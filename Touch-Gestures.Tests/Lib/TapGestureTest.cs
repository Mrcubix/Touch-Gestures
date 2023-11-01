using System.Numerics;
using System.Threading;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Entities.Gestures;
using Xunit;
using Xunit.Abstractions;

namespace TouchGestures.Tests.Lib
{
    public class TapGestureTest
    {
        #region Constants & Readonly fields

        private const int MAX_TOUCHES = 3;
        private const double DEADLINE = 500;
        private const int TESTED_TOUCHES = 2;

        private readonly Vector2 THRESHOLD = new(30, 30);

        #endregion

        #region Constructor

        private readonly ITestOutputHelper _output;

        public TapGestureTest(ITestOutputHelper output)
        {
            _output = output;
        }

        #endregion

        #region Test samples

        // This is a simple one touch point, as if the user was dragging or tapping.
        public TouchPoint[] NonGestureTouchPoints = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new(1, 1)
            },
            null!,
            null!,
        };

        // The gesture should be started when passing such touch points.
        public TouchPoint[] ValidGestureTouchPoints = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new(1, 1)
            },
            null!,
            new()
            {
                TouchID = 1,
                Position = new(2, 2)
            }
        };

        // One of the touch is released which should trigger the deadline timer but not the gesture.
        public TouchPoint[] ValidGestureTouchPointsIntermediaryState = new TouchPoint[MAX_TOUCHES]
        {
            null!,
            null!,
            new()
            {
                TouchID = 1,
                Position = new(2, 2)
            },
        };

        // The gesture should be completed when passing such touch points.
        public TouchPoint[] ValidGestureTouchPointsFinalState = new TouchPoint[MAX_TOUCHES]
        {
            null!,
            null!,
            null!,
        };

        // The gesture should be ended when passing such touch points, considered as invalid.
        public TouchPoint[] InvalidationGestureTouchPoints = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new(1, 1)
            },
            new()
            {
                TouchID = 1,
                Position = new(2, 2)
            },
            new()
            {
                TouchID = 2,
                Position = new(3, 3)
            },
        };

        // The gesture should be ended when passing such touch points, considered as invalid since a touch point is out of bounds.
        public TouchPoint[] OutOfBoundsTouchPoints = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new(1, 1)
            },
            new()
            {
                TouchID = 1,
                Position = new(100, 100)
            },
            null!
        };

        // the gesture should be ended when passing such touch points, considered as invalid since the touch point is out of bounds during the intermediary state.
        public TouchPoint[] OutOfBoundsTouchPointsIntermediaryState = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new(100, 1)
            },
            null!,
            null!
        };

        #endregion

        #region Non-gesture test

        /// <summary>
        ///   A test where a non-gesture touch point is passed.
        /// </summary>
        /// <remarks>
        ///    The gesture should not be started when passing such touch points.
        /// </remarks>
        [Fact]
        public void NonGestureTest()
        {
            TapGesture gesture = new(THRESHOLD, DEADLINE, TESTED_TOUCHES);

            // we pass a non-gesture touch point
            gesture.OnInput(NonGestureTouchPoints);

            Assert.False(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture has not started as expected");
        }

        #endregion

        #region Valid gesture test

        /// <summary>
        ///   A test where a series of touch points are passed, which should trigger the gesture.
        /// </summary>
        /// <remarks>
        ///   The gesture should be started at first and should have ended upon completion.
        /// </remarks>
        [Fact]
        public void ValidGestureTest()
        {
            TapGesture gesture = IntermidiaryTestPart();

            FinalizeGesture(gesture);

            _output.WriteLine("Gesture was completed & ended successfully");
        }

        #endregion

        #region Valid gesture but a released touch is detected again

        /// <summary>
        ///   A test where a series of touch points are passed, which should trigger the gesture.
        ///   However, the released touch point is sent again, but it should still be considered as valid.
        /// </summary>
        /// <remarks>
        ///   The gesture should be started at first and should have ended upon completion.
        /// </remarks>
        [Fact]
        public void ValidGestureReleasedDetected()
        {
            TapGesture gesture = IntermidiaryTestPart();

            // we pass an intermediary state touch point
            gesture.OnInput(ValidGestureTouchPoints);

            Assert.False(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture has not ended abruptly");

            // we pass the intermediary state touch point again
            gesture.OnInput(ValidGestureTouchPointsIntermediaryState);

            Assert.False(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture has not ended abruptly");

            FinalizeGesture(gesture);

            _output.WriteLine("Gesture was completed & ended successfully");
            
        }

        #endregion

        #region Mid-gesture invalidation test

        /// <summary>
        ///   A test where the initial and invoking touch points were active, <br/>
        ///   but a touch point other of the invoking touch points is active upon the third report,
        ///   which should invalidate the gesture.
        /// </summary>
        /// <remarks>
        ///   The gesture should be started at first and should have ended abruptly. <br/>
        ///   The gesture should not be completed.
        /// </remarks>
        [Fact]
        public void MidGestureInvalidation()
        {
            TapGesture gesture = IntermidiaryTestPart();

            // we pass an invalidation touch point
            gesture.OnInput(InvalidationGestureTouchPoints);

            // the gesture should not be started
            Assert.False(gesture.HasStarted);

            // the gesture should have ended abruptly
            Assert.True(gesture.HasEnded);

            // the gesture should not be completed
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture ended abruptly as expected");
        }

        #endregion

        #region Invoking touch point goes out of bounds

        /// <summary>
        ///   A test where the initial and invoking touch points were active, <br/>
        ///   but one of the invoking touch points goes out of bounds upon the second report,
        ///   which should invalidate the gesture.
        /// </summary>
        /// <remarks>
        ///   The gesture should be started at first and should have ended abruptly. <br/>
        ///   The gesture should not be completed.
        /// </remarks>
        [Fact]
        public void InvokingTouchPointOutOfBounds()
        {
            TapGesture gesture = new(THRESHOLD, DEADLINE, TESTED_TOUCHES);

            // we pass a valid gesture touch point
            gesture.OnInput(ValidGestureTouchPoints);

            // the gesture should be started
            Assert.True(gesture.HasStarted);

            _output.WriteLine("Gesture started successfully");

            // we pass an out of bounds touch point
            gesture.OnInput(OutOfBoundsTouchPoints);

            // the gesture should have ended due to the out of bounds touch point
            Assert.True(gesture.HasEnded);

            // the gesture should not be completed
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture ended abruptly as expected");
        }

        #endregion

        #region Gesture is valid but deadline was passed

        /// <summary>
        ///   A test where the initial and invoking touch points were active, <br/>
        ///   but the deadline was passed upon the last report,
        ///   which should end the gesture.
        /// </summary>
        /// <remarks>
        ///   The gesture should be started at first and should have ended abruptly. <br/>
        ///   The gesture should not be completed.
        /// </remarks>
        [Fact]
        public void DeadlinePassed()
        {
            TapGesture gesture = IntermidiaryTestPart();

            Thread.Sleep((int)DEADLINE + 50);

            // we pass an end state touch point
            gesture.OnInput(ValidGestureTouchPointsFinalState);

            Assert.True(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture ended abruptly as expected");
        }

        #endregion

        #region Shared test parts

        private TapGesture IntermidiaryTestPart()
        {
            TapGesture gesture = new(THRESHOLD, DEADLINE, TESTED_TOUCHES);

            // we pass a valid gesture touch point
            gesture.OnInput(ValidGestureTouchPoints);

            // the gesture should be started
            Assert.True(gesture.HasStarted);

            _output.WriteLine("Gesture started successfully");

            // we pass an intermediary state touch point
            gesture.OnInput(ValidGestureTouchPointsIntermediaryState);

            // the gesture should not have ended
            Assert.False(gesture.HasEnded);

            // the gesture should not be completed
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture has not ended abruptly");

            return gesture;
        }

        private void FinalizeGesture(TapGesture gesture)
        {
            // we pass an end state touch point
            gesture.OnInput(ValidGestureTouchPointsFinalState);

            // the gesture should be completed
            Assert.True(gesture.HasCompleted);

            // the gesture should have ended
            Assert.True(gesture.HasEnded);

            // the gesture has ended, therefore it isn't started anymore
            Assert.False(gesture.HasStarted);
        }

        #endregion
    }
}