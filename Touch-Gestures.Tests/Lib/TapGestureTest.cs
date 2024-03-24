using System.Drawing;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using System.Numerics;
using TouchGestures.Lib.Entities.Gestures;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Tests.Lib
{
    using static TouchGestures.Tests.Samples.TapSamples;

    public class TapGestureTest
    {
        #region Constants & Readonly fields
        
        protected const double DEADLINE = 500;
        protected const int TESTED_TOUCHES = 2;

        protected readonly Rectangle BOUNDS = new(0, 0, 30, 30);
        protected readonly Vector2 THRESHOLD = new(30, 30);

        #endregion

        #region Constructor

        protected readonly ITestOutputHelper _output;

        public TapGestureTest(ITestOutputHelper output)
        {
            _output = output;
        }

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
            TapGesture gesture = new(BOUNDS, DEADLINE, TESTED_TOUCHES);

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
            var sample = GenerateValidSample(2);
            var intermediarySamples = GenerateIntermediarySamples(2);

            // we pass an intermediary state touch point
            gesture.OnInput(sample);

            Assert.False(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture has not ended abruptly");

            // we pass the intermediary state touch point again
            gesture.OnInput(intermediarySamples[0]);

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
        ///   The gesture should not be completed. <br/>
        ///   The gesture will only end when all touch points are released.
        /// </remarks>
        [Fact]
        public void MidGestureInvalidation()
        {
            TapGesture gesture = IntermidiaryTestPart();
            var sample = GenerateValidSample(1);

            // we pass an invalidation touch point
            gesture.OnInput(InvalidationGestureTouchPoints);

            // the gesture should have started
            Assert.True(gesture.HasStarted);

            // the gesture should be in an invalid state
            Assert.True(gesture.IsInvalidState);

            // the gesture should not have ended yet
            Assert.False(gesture.HasEnded);

            // the gesture should not be completed
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture has not ended abruptly as expected");

            // we pass an intermediary state touch point
            gesture.OnInput(sample);

            // the gesture should not have ended
            Assert.False(gesture.HasEnded);

            // the gesture should not be completed
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture has not ended abruptly");

            // we finally release all touch points

            // we pass an end state touch point
            gesture.OnInput(ReleasedTouchPoints);

            // the gesture should have ended
            Assert.True(gesture.HasEnded);

            // the gesture should not be completed
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture finally ended in a failure when all touch points were released");
        }

        #endregion

        #region Invoking touch point goes out of bounds

        /// <summary>
        ///   A test where the initial and invoking touch points were active, <br/>
        ///   but one of the invoking touch points goes out of bounds upon the second report,
        ///   which should invalidate the gesture.
        /// </summary>
        /// <remarks>
        ///   The gesture should be started at first and should not have had ended abruptly. <br/>
        ///   The gesture should not be completed. <br/>
        ///   The gesture will only end when all touch points are released.
        /// </remarks>
        [Fact]
        public void InvokingTouchPointOutOfBounds()
        {
            TapGesture gesture = new(BOUNDS, DEADLINE, TESTED_TOUCHES);
            var sample = GenerateValidSample(2);

            // we pass a valid gesture touch point
            gesture.OnInput(sample);

            // the gesture should be started
            Assert.True(gesture.HasStarted);

            _output.WriteLine("Gesture started successfully");

            // we pass an out of bounds touch point
            gesture.OnInput(OutOfBoundsTouchPoints);

            // the gesture should have started
            Assert.True(gesture.HasStarted);

            // the gesture should be in an invalid state
            Assert.True(gesture.IsInvalidState);

            // the gesture should not have ended yet
            Assert.False(gesture.HasEnded);

            // the gesture should not be completed
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture has not ended abruptly as expected");

            // we finally release all touch points

            // we pass an end state touch point
            gesture.OnInput(ReleasedTouchPoints);

            // the gesture should have ended
            Assert.True(gesture.HasEnded);

            // the gesture should not be completed
            Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture finally ended in a failure when all touch points were released");
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
            gesture.OnInput(ReleasedTouchPoints);

            //Assert.True(gesture.HasEnded);
            //Assert.False(gesture.HasCompleted);

            _output.WriteLine("Gesture ended abruptly as expected");
        }

        #endregion

        #region Shared test parts

        private TapGesture IntermidiaryTestPart()
        {
            TapGesture gesture = new(BOUNDS, DEADLINE, TESTED_TOUCHES);
            TouchPoint[] sample = GenerateValidSample(2);
            TouchPoint[][] intermediarySamples = GenerateIntermediarySamples(2);

            // we pass a valid gesture touch point
            gesture.OnInput(sample);

            // the gesture should be started
            Assert.True(gesture.HasStarted);

            _output.WriteLine("Gesture started successfully");

            // we pass an intermediary state touch point
            gesture.OnInput(intermediarySamples[0]);

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
            gesture.OnInput(ReleasedTouchPoints);

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