using System.Drawing;
using System.Numerics;
using TouchGestures.Lib.Entities.Gestures;
using Xunit;
using Xunit.Abstractions;

namespace TouchGestures.Tests.Lib
{
    using static TouchGestures.Tests.Samples.PinchSamples;

    public class RotateGestureTest
    {
        #region Constants & Readonly fields

        public const double ANGLE_THRESHOLD = 10;

        public const double DEADLINE = 60;
        public const int TESTED_TOUCHES = 2;

        public static readonly Rectangle BOUNDS = new(0, 0, 100, 100);
        public static readonly Vector2 ORIGIN = new(50, 50);

        #endregion

        #region Constructor

        protected readonly ITestOutputHelper _output;

        public RotateGestureTest(ITestOutputHelper output)
        {
            _output = output;
        }

        #endregion

        #region Tests

        [Fact]
        public void TestClockwiseRotation()
        {
            // Given
            var gesture = new PinchGesture(0, ANGLE_THRESHOLD, false, true, BOUNDS);
            var samples = GenerateRotateReportsTowardsTarget(50, ORIGIN, true);

            // When
            for (int i = 0; i < samples.Length; i++)
            {
                gesture.OnInput(samples[i]);

                // Then
                if (i == 0)
                {
                    Assert.True(gesture.HasStarted);
                    Assert.False(gesture.HasEnded);
                    Assert.False(gesture.HasCompleted);
                }
                
                // Distance delta is -10.799997977871385 at the 6th sample
                if (i > 6)
                {
                    Assert.True(gesture.HasStarted);
                    Assert.False(gesture.HasEnded);
                    Assert.True(gesture.HasCompleted);
                }
            }

            // Release the touch points
            TestReleasedTouches(gesture);

            _output.WriteLine("Clockwise Rotate Gesture: Passed");
        }

        [Fact]
        public void TestCounterClockwiseRotation()
        {
            // Given
            var gesture = new PinchGesture(0, ANGLE_THRESHOLD, false, false, BOUNDS);
            var samples = GenerateRotateReportsTowardsTarget(50, ORIGIN, false);

            // When
            for (int i = 0; i < samples.Length; i++)
            {
                gesture.OnInput(samples[i]);

                // Then
                if (i == 0)
                {
                    Assert.True(gesture.HasStarted);
                    Assert.False(gesture.HasEnded);
                    Assert.False(gesture.HasCompleted);
                }
                
                // Distance delta is 10.799997977871385 at the 6th sample
                if (i > 5)
                {
                    Assert.True(gesture.HasStarted);
                    Assert.False(gesture.HasEnded);
                    Assert.True(gesture.HasCompleted);
                }
            }

            // Release the touch points
            TestReleasedTouches(gesture);

            _output.WriteLine("Counter-Clockwise Rotate Gesture: Passed");
        }

        #endregion

        #region Shared

        private static void TestReleasedTouches(PinchGesture gesture)
        {
            // Given
            var sample = NoTouchSample;

            // When
            gesture.OnInput(sample);

            // Then
            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);
        }

        #endregion
    }
}