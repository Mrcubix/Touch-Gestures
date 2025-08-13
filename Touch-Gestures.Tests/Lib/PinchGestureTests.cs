using System.Drawing;
using System.Numerics;
using TouchGestures.Lib.Entities.Gestures;
using Xunit;
using Xunit.Abstractions;

namespace TouchGestures.Tests.Lib
{
    using static TouchGestures.Tests.Samples.PinchSamples;

    [Collection("Pinch Gesture Tests")]
    public class PinchGestureTest
    {
        #region Constants & Readonly fields

        public const double DISTANCE_THRESHOLD = 10;

        protected const double DEADLINE = 60;
        protected const int TESTED_TOUCHES = 2;

        protected readonly Rectangle BOUNDS = new(0, 0, 100, 100);

        #endregion

        #region Constructor

        protected readonly ITestOutputHelper _output;

        public PinchGestureTest(ITestOutputHelper output)
        {
            _output = output;
        }

        #endregion

        #region Tests

        [Fact]
        public void TestInnerPinchGesture()
        {
            // Given
            var gesture = new PinchGesture(DISTANCE_THRESHOLD, 0, true, false, BOUNDS);
            var samples = GeneratePinchReportsTowardsTarget(50, InnerPinchInitialSample, new Vector2[] { new(50, 50), new(50, 50) });

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
                
                // Distance delta is -11.313705444335938 at the 5th sample
                if (i > 4)
                {
                    Assert.True(gesture.HasStarted);
                    Assert.False(gesture.HasEnded);
                    Assert.True(gesture.HasCompleted);
                }
            }

            // Release the touch points
            TestReleasedTouches(gesture);

            _output.WriteLine("Inner Pinch Gesture: Passed");
        }

        [Fact]
        public void TestOuterPinchGesture()
        {
            // Given
            var gesture = new PinchGesture(DISTANCE_THRESHOLD, 0, false, false, BOUNDS);
            var samples = GeneratePinchReportsTowardsTarget(50, OuterPinchInitialSample, new Vector2[] { new(0, 0), new(100, 100) });

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
                
                // Distance delta is 11.313705444335938 at the 5th sample
                if (i > 4)
                {
                    Assert.True(gesture.HasStarted);
                    Assert.False(gesture.HasEnded);
                    Assert.True(gesture.HasCompleted);
                }
            }

            // Release the touch points
            TestReleasedTouches(gesture);

            _output.WriteLine("Outer Pinch Gesture: Passed");
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