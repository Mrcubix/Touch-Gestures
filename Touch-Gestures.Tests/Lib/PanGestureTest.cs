using System.Drawing;
using System.Numerics;
using System.Threading;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Input;
using Xunit;
using Xunit.Abstractions;

namespace TouchGestures.Tests.Lib
{
    using static TouchGestures.Tests.Samples.SwipeSamples;

    public class PanGestureTest
    {
        /*
          the following elements are to be tested:
            - All 8 directions swipes using 2 reports each,
            - The gesture should not trigger is the threshold is not reached
            - Absolute positionable gestures should only trigger if the starting touch point is within the bounds
        */

        #region Constants & readonly fields

        private const double DEADLINE = 50;

        private readonly Vector2 THRESHOLD = new(30, 30);

        private readonly Rectangle EXAMPLE_BOUNDS = new(0, 0, 400, 400);

        #endregion

        #region Constructor

        private readonly ITestOutputHelper _output;

        public PanGestureTest(ITestOutputHelper output)
        {
            _output = output;
        }

        #endregion

        #region Tests

        #region Directional Swipes

        /// <summary>
        ///   This test aims to check if the gesture is completed after performing a swipe in the left direction properly.
        /// </summary>
        [Fact]
        public void LeftPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.Left);

            SubscribeToCompletion(gesture, SwipeDirection.Left);

            TestOriginSampleData(gesture);

            gesture.OnInput(LeftSwipeSampleData);

            _output.WriteLine($"Left Pan Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            TestReleasedSampleData(gesture);

            UnsubscribeToCompletion(gesture, SwipeDirection.Left);

            _output.WriteLine($"Left Swipe Gesture Completed");
        }

        /// <summary>
        ///   This test aims to check if the gesture is completed after performing a swipe in the up direction properly.
        /// </summary>
        [Fact]
        public void UpPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.Up);

            SubscribeToCompletion(gesture, SwipeDirection.Up);

            TestOriginSampleData(gesture);

            gesture.OnInput(UpSwipeSampleData);

            _output.WriteLine($"Up Swipe Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            TestReleasedSampleData(gesture);

            UnsubscribeToCompletion(gesture, SwipeDirection.Up);

            _output.WriteLine($"Up Swipe Gesture Completed");
        }

        /// <summary>
        ///   This test aims to check if the gesture is completed after performing a swipe in the right direction properly.
        /// </summary>
        [Fact]
        public void RightPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.Right);

            SubscribeToCompletion(gesture, SwipeDirection.Right);

            TestOriginSampleData(gesture);

            gesture.OnInput(RightSwipeSampleData);

            _output.WriteLine($"Right Swipe Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            TestReleasedSampleData(gesture);

            UnsubscribeToCompletion(gesture, SwipeDirection.Right);

            _output.WriteLine($"Right Swipe Gesture Completed");
        }

        /// <summary>
        ///   This test aims to check if the gesture is completed after performing a swipe in the down direction properly.
        /// </summary>
        [Fact]
        public void DownPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.Down);

            SubscribeToCompletion(gesture, SwipeDirection.Down);

            TestOriginSampleData(gesture);

            gesture.OnInput(DownSwipeSampleData);

            _output.WriteLine($"Down Swipe Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            TestReleasedSampleData(gesture);

            UnsubscribeToCompletion(gesture, SwipeDirection.Down);

            _output.WriteLine($"Down Swipe Gesture Completed");
        }

        /// <summary>
        ///   This test aims to check if the gesture is completed after performing a swipe in the up-left direction properly.
        /// </summary>
        [Fact]
        public void LeftUpPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.UpLeft);

            SubscribeToCompletion(gesture, SwipeDirection.UpLeft);

            TestOriginSampleData(gesture);

            gesture.OnInput(UpLeftSwipeSampleData);

            _output.WriteLine($"Left Up Swipe Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            TestReleasedSampleData(gesture);

            UnsubscribeToCompletion(gesture, SwipeDirection.UpLeft);

            _output.WriteLine($"Left Up Swipe Gesture Completed");
        }

        /// <summary>
        ///   This test aims to check if the gesture is completed after performing a swipe in the up-right direction properly.
        /// </summary>
        [Fact]
        public void RightUpPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.UpRight);

            SubscribeToCompletion(gesture, SwipeDirection.UpRight);

            TestOriginSampleData(gesture);

            gesture.OnInput(UpRightSwipeSampleData);

            _output.WriteLine($"Right Up Swipe Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            TestReleasedSampleData(gesture);

            UnsubscribeToCompletion(gesture, SwipeDirection.UpRight);

            _output.WriteLine($"Right Up Swipe Gesture Completed");
        }

        /// <summary>
        ///   This test aims to check if the gesture is completed after performing a swipe in the down-left direction properly.
        /// </summary>
        [Fact]
        public void LeftDownPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.DownLeft);

            SubscribeToCompletion(gesture, SwipeDirection.DownLeft);

            TestOriginSampleData(gesture);

            gesture.OnInput(DownLeftSwipeSampleData);

            _output.WriteLine($"Left Down Swipe Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            TestReleasedSampleData(gesture);

            UnsubscribeToCompletion(gesture, SwipeDirection.DownLeft);

            _output.WriteLine($"Left Down Swipe Gesture Completed");
        }

        /// <summary>
        ///   This test aims to check if the gesture is completed after performing a swipe in the down-right direction properly.
        /// </summary>
        [Fact]
        public void RightDownPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.DownRight);

            SubscribeToCompletion(gesture, SwipeDirection.DownRight);

            TestOriginSampleData(gesture);

            gesture.OnInput(DownRightSwipeSampleData);

            _output.WriteLine($"Right Down Swipe Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            TestReleasedSampleData(gesture);

            UnsubscribeToCompletion(gesture, SwipeDirection.DownRight);

            _output.WriteLine($"Right Down Swipe Gesture Completed");
        }

        #endregion

        #region Absolute

        // It does not matter if the gesture is completed outside the bounnds, the boundaries are only used to start the gesture.

        /// <summary>
        ///   This test aims to check if the gesture is started after performing a specified swipe within the provided bounds.
        /// </summary>
        [Fact]
        public void AbsoluteGestureStartedWithinBounds()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.DownRight, EXAMPLE_BOUNDS);

            TestOriginSampleData(gesture);

            _output.WriteLine($"Origin Sample Data passed to OnInput, Assertion seems to have succeeded as expected");
        }

        /// <summary>
        ///   This test aims to check if the gesture is not started after performing a specified swipe outside the provided bounds.
        /// </summary>
        [Fact]
        public void AbsoluteGestureNotStartedOutsideBounds()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.DownRight, EXAMPLE_BOUNDS);

            //TestOriginSampleData(gesture);
            gesture.OnInput(OriginAbsoluteFaultyData);

            _output.WriteLine($"Faulty Origin Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            _output.WriteLine($"Gesture did not start as expected");
        }

        #endregion

        #region Intended Failures Cases

        /// <summary>
        ///   This test aims to check if the gesture is not completed after performing a pan in the left direction but not reaching the threshold.
        /// </summary>
        [Fact]
        public void ReleasedPanGesture()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.Left);

            SubscribeToCompletion(gesture, SwipeDirection.DownRight);

            TestOriginSampleData(gesture);

            gesture.OnInput(ReleasedSampleData);

            _output.WriteLine($"Released Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.DownRight);

            _output.WriteLine($"Releasing the active touch point ended the gesture as expected");
        }

        /// <summary>
        ///   This test aims to check if the gesture is not completed after performing a swipe in the left direction but is out of time.
        /// </summary>
        [Fact]
        public void CompletedAfterDeadline()
        {
            var gesture = new PanGesture(THRESHOLD, DEADLINE, SwipeDirection.DownRight);

            SubscribeToCompletion(gesture, SwipeDirection.DownRight);

            TestOriginSampleData(gesture);

            Thread.Sleep((int)DEADLINE + 50);

            gesture.OnInput(DownRightSwipeSampleData);

            _output.WriteLine($"Right Down Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.DownRight);

            _output.WriteLine($"Late Completion resulted in the gesture being ended as expected");
        }

        #endregion

        #endregion

        #region Shared

        private void TestOriginSampleData(PanGesture gesture)
        {
            gesture.OnInput(OriginSampleData);

            _output.WriteLine($"Origin Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);
        }

        private void TestReleasedSampleData(PanGesture gesture)
        {
            gesture.OnInput(ReleasedSampleData);

            _output.WriteLine($"Released Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);
        }

        private void SubscribeToCompletion(PanGesture gesture, SwipeDirection direction)
        {
            gesture.GestureCompleted += (_, e) => OnCompletion((SwipeGestureEventArgs)e, direction);
        }

        private void UnsubscribeToCompletion(PanGesture gesture, SwipeDirection direction)
        {
            gesture.GestureCompleted -= (_, e) => OnCompletion((SwipeGestureEventArgs)e, direction);
        }

        private void OnCompletion(SwipeGestureEventArgs e, SwipeDirection comparison)
        {
            Assert.Equal(comparison, e.Direction);
        }

        #endregion
    }
}