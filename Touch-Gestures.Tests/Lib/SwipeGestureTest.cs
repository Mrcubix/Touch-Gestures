using System;
using System.Numerics;
using System.Threading;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Input;
using Xunit;
using Xunit.Abstractions;

namespace TouchGestures.Tests.Lib
{
    public class SwipeGestureTest
    {
        /*
          the following elements are to be tested:
            - All 8 directions swipes using 2 reports each,
            - The gesture should not trigger is the threshold is not reached & the invoking touch point is released,
            - The gesture should not trigger if the threshold is reached but the invoking touch point is released after the deadline passed
            - ?
        */

        #region Constants & readonly fields

        private const int MAX_TOUCHES = 1;
        private const double DEADLINE = 500;

        private readonly Vector2 THRESHOLD = new(30, 30);
        private readonly IBinding BINDING = null!;
        private readonly Action SubscribeAction = null!;

        #endregion

        #region Constructor

        private readonly ITestOutputHelper _output;

        public SwipeGestureTest(ITestOutputHelper output)
        {
            _output = output;
        }

        #endregion

        #region Sample Data

        public TouchPoint[] OriginSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(0, 0),
            },
        };

        public TouchPoint[] LeftSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(-30, 0),
            },
        };

        public TouchPoint[] UpSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(0, -30),
            },
        };

        public TouchPoint[] RightSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(30, 0),
            },
        };

        public TouchPoint[] DownSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(0, 30),
            },
        };

        public TouchPoint[] UpLeftSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(-30, -30),
            },
        };

        public TouchPoint[] UpRightSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(30, -30),
            },
        };

        public TouchPoint[] DownLeftSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(-30, 30),
            },
        };

        public TouchPoint[] DownRightSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(30, 30),
            },
        };
        
        public TouchPoint[] ReleasedSampleData = new TouchPoint[MAX_TOUCHES]
        {
            null!,
        };

        #endregion

        #region 

        [Fact]
        public void LeftSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.Left, BINDING);

            SubscribeToCompletion(gesture, SwipeDirection.Left);

            TestOriginSampleData(gesture);

            gesture.OnInput(LeftSwipeSampleData);

            _output.WriteLine($"Left Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.Left);

            _output.WriteLine($"Left Swipe Gesture Completed");
        }

        [Fact]
        public void UpSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.Up, BINDING);

            SubscribeToCompletion(gesture, SwipeDirection.Up);

            TestOriginSampleData(gesture);

            gesture.OnInput(UpSwipeSampleData);

            _output.WriteLine($"Up Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.Up);

            _output.WriteLine($"Up Swipe Gesture Completed");
        }

        [Fact]
        public void RightSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.Right, BINDING);

            SubscribeToCompletion(gesture, SwipeDirection.Right);

            TestOriginSampleData(gesture);

            gesture.OnInput(RightSwipeSampleData);

            _output.WriteLine($"Right Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.Right);

            _output.WriteLine($"Right Swipe Gesture Completed");
        }

        [Fact]
        public void DownSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.Down, BINDING);

            SubscribeToCompletion(gesture, SwipeDirection.Down);

            TestOriginSampleData(gesture);

            gesture.OnInput(DownSwipeSampleData);

            _output.WriteLine($"Down Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.Down);

            _output.WriteLine($"Down Swipe Gesture Completed");
        }

        [Fact]
        public void LeftUpSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.UpLeft, BINDING);

            SubscribeToCompletion(gesture, SwipeDirection.UpLeft);

            TestOriginSampleData(gesture);

            gesture.OnInput(UpLeftSwipeSampleData);

            _output.WriteLine($"Left Up Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.UpLeft);

            _output.WriteLine($"Left Up Swipe Gesture Completed");
        }

        [Fact]
        public void RightUpSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.UpRight, BINDING);

            SubscribeToCompletion(gesture, SwipeDirection.UpRight);

            TestOriginSampleData(gesture);

            gesture.OnInput(UpRightSwipeSampleData);

            _output.WriteLine($"Right Up Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.UpRight);

            _output.WriteLine($"Right Up Swipe Gesture Completed");
        }

        [Fact]
        public void LeftDownSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.DownLeft, BINDING);

            SubscribeToCompletion(gesture, SwipeDirection.DownLeft);

            TestOriginSampleData(gesture);

            gesture.OnInput(DownLeftSwipeSampleData);

            _output.WriteLine($"Left Down Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.DownLeft);

            _output.WriteLine($"Left Down Swipe Gesture Completed");
        }

        [Fact]
        public void RightDownSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.DownRight, BINDING);

            SubscribeToCompletion(gesture, SwipeDirection.DownRight);

            TestOriginSampleData(gesture);

            gesture.OnInput(DownRightSwipeSampleData);

            _output.WriteLine($"Right Down Swipe Sample Data passed to OnInput");

            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);

            UnsubscribeToCompletion(gesture, SwipeDirection.DownRight);

            _output.WriteLine($"Right Down Swipe Gesture Completed");
        }

        [Fact]
        public void ReleasedSwipeGesture()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.DownRight, BINDING);

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

        [Fact]
        public void CompletedAfterDeadline()
        {
            var gesture = new SwipeGesture(THRESHOLD, DEADLINE, SwipeDirection.DownRight, BINDING);

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

        #region Shared

        private void TestOriginSampleData(SwipeGesture gesture)
        {
            gesture.OnInput(OriginSampleData);

            _output.WriteLine($"Origin Sample Data passed to OnInput");

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);
        }

        private void SubscribeToCompletion(SwipeGesture gesture, SwipeDirection direction)
        {
            gesture.GestureCompleted += (_, e) => OnCompletion((SwipeGestureEventArgs)e, direction);
        }

        private void UnsubscribeToCompletion(SwipeGesture gesture, SwipeDirection direction)
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