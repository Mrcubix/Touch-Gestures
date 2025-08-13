using System.Drawing;
using System.Numerics;
using System.Threading;
using TouchGestures.Lib.Entities.Gestures;
using Xunit;
using Xunit.Abstractions;

namespace TouchGestures.Tests.Lib
{
    using static TouchGestures.Tests.Samples.TapSamples;

    [Collection("Hold Gesture Tests")]
    public class HoldGestureTest
    {
        #region Constants & Readonly fields

        protected const int MAX_TOUCHES = 3;
        protected const double DEADLINE = 60;
        protected const int TESTED_TOUCHES = 2;

        protected readonly Rectangle BOUNDS = new(0, 0, 30, 30);
        protected readonly Vector2 THRESHOLD = new(30, 30);

        #endregion

        #region Constructor

        protected readonly ITestOutputHelper _output;

        public HoldGestureTest(ITestOutputHelper output)
        {
            _output = output;
        }

        #endregion

        #region Tests

        /// <summary>
        ///   Test the gesture with a deadline. <br/>
        ///   The gesture should be completed.
        /// </summary>
        [Fact]
        public void OneToTenTapTest()
        {
            _output.WriteLine("HoldGestureTest.OneToTenTapTest: Testing the gesture with a deadline.");

            for (int i = 1; i <= 10; i++)
            {
                // Given
                var gesture = new HoldGesture(BOUNDS, DEADLINE, i);
                var sample = GenerateValidSample(i);
                
                TestIntermediarySamples(gesture, i);

                gesture.OnInput(sample);

                Assert.True(gesture.HasStarted);
                Assert.False(gesture.HasCompleted);
                Assert.False(gesture.HasCompleted);

                // Wait for the deadline to pass
                Thread.Sleep((int)DEADLINE + 15);

                // Pass a similar sample with the same touch points active
                gesture.OnInput(sample);

                Assert.True(gesture.HasActivated);
                Assert.False(gesture.HasEnded);
                Assert.False(gesture.HasCompleted);

                TestReleasedTouches(gesture);

                _output.WriteLine($"HoldGestureTest.OneToTenTapTest: {i} touch points passed.");
            }
        }

        /// <summary>
        ///   Test the gesture without waiting for the deadline to pass. <br/>
        ///   The gesture should not be completed.
        /// </summary>
        [Fact]
        public void OneToTenButNotWaitingTest()
        {
            _output.WriteLine("HoldGestureTest.OneToTenButNotWaitingTest: Testing the gesture without waiting for the deadline to pass.");

            for (int i = 1; i <= 10; i++)
            {
                // Given
                var gesture = new HoldGesture(BOUNDS, DEADLINE, i);
                var sample = GenerateValidSample(i);

                TestIntermediarySamples(gesture, i);

                gesture.OnInput(sample);

                Assert.True(gesture.HasStarted);
                Assert.False(gesture.HasCompleted);
                Assert.False(gesture.HasCompleted);

                // Pass a similar sample with the same touch points active
                gesture.OnInput(sample);

                Assert.False(gesture.HasActivated);
                Assert.False(gesture.HasEnded);
                Assert.False(gesture.HasCompleted);

                TestReleasedTouches(gesture);

                _output.WriteLine($"HoldGestureTest.OneToTenButNotWaitingTest: {i} touch points passed.");
            }
        }

        /// <summary>
        ///   Test the gesture when one of the touch points go out of bounds. <br/>
        ///   The gesture should not be completed.
        /// </summary>
        [Fact]
        public void GoesOutOfRangeTest()
        {
            _output.WriteLine("HoldGestureTest.GoesOutOfRangeTest: Testing the gesture when one of the touch points go out of bounds.");

            var gesture = new HoldGesture(BOUNDS, DEADLINE, TESTED_TOUCHES);
            var validSample = GenerateValidSample(TESTED_TOUCHES);
            var invalidSample = OutOfBoundsTouchPoints;

            gesture.OnInput(validSample);

            Assert.True(gesture.HasStarted);
            Assert.False(gesture.HasCompleted);
            Assert.False(gesture.HasCompleted);

            // wait for the deadline to pass
            Thread.Sleep((int)DEADLINE + 15);

            gesture.OnInput(invalidSample);
            
            Assert.False(gesture.HasActivated);
            Assert.False(gesture.HasEnded);
            Assert.False(gesture.HasCompleted);

            TestReleasedTouches(gesture);

            _output.WriteLine("HoldGestureTest.GoesOutOfRangeTest: Test passed.");
        }

        #endregion

        #region Shared

        private static void TestReleasedTouches(HoldGesture gesture)
        {
            // Given
            var sample = ReleasedTouchPoints;

            // When
            gesture.OnInput(sample);

            // Then
            Assert.False(gesture.HasActivated);
            Assert.False(gesture.HasStarted);
            Assert.True(gesture.HasEnded);
            Assert.True(gesture.HasCompleted);
        }

        private static void TestIntermediarySamples(HoldGesture gesture, int i)
        {
            var intermediarySamples = GenerateIntermediarySamples(i);

            foreach (var isample in intermediarySamples)
            {
                gesture.OnInput(isample);

                Assert.False(gesture.HasStarted);
                Assert.False(gesture.HasEnded);
                Assert.False(gesture.HasCompleted);
            }
        }

        #endregion
    }
}