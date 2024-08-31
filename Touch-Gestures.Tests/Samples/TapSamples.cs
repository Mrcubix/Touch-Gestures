using System;
using TouchGestures.Lib.Entities.Tablet.Touch;

namespace TouchGestures.Tests.Samples
{
    public static class TapSamples
    {
        private const int MAX_TOUCHES = 10;

        static TapSamples()
        {
            NonGestureTouchPoints[0] = new TouchPoint
            {
                TouchID = 0,
                Position = new(1, 1)
            };

            // Invalid touch points
            InvalidationGestureTouchPoints[0] = new TouchPoint
            {
                TouchID = 0,
                Position = new(1, 1)
            };
            InvalidationGestureTouchPoints[1] = new TouchPoint
            {
                TouchID = 1,
                Position = new(2, 2)
            };
            InvalidationGestureTouchPoints[2] = new TouchPoint
            {
                TouchID = 2,
                Position = new(3, 3)
            };

            // Out of bounds touch points
            OutOfBoundsTouchPoints[0] = new TouchPoint
            {
                TouchID = 0,
                Position = new(1, 1)
            };
            OutOfBoundsTouchPoints[1] = new TouchPoint
            {
                TouchID = 1,
                Position = new(100, 100)
            };

            // Out of bounds touch points during intermediary state
            OutOfBoundsTouchPointsIntermediaryState[1] = new TouchPoint
            {
                TouchID = 1,
                Position = new(100, 1)
            };
        }

        #region Sample Data

        // This is a simple one touch point, as if the user was dragging or tapping.
        public static readonly TouchPoint[] NonGestureTouchPoints = new TouchPoint[MAX_TOUCHES];

        // The gesture should be ended when passing such touch points, considered as invalid.
        public static readonly TouchPoint[] InvalidationGestureTouchPoints = new TouchPoint[MAX_TOUCHES];

        // The gesture should be ended when passing such touch points, considered as invalid since a touch point is out of bounds.
        public static readonly TouchPoint[] OutOfBoundsTouchPoints = new TouchPoint[MAX_TOUCHES];

        // the gesture should be ended when passing such touch points, considered as invalid since the touch point is out of bounds during the intermediary state.
        public static readonly TouchPoint[] OutOfBoundsTouchPointsIntermediaryState = new TouchPoint[MAX_TOUCHES];

        // The gesture should be completed when passing such touch points.
        public static readonly TouchPoint[] ReleasedTouchPoints = new TouchPoint[MAX_TOUCHES];

        #endregion

        /// <summary>
        ///   Generate a valid starting sample for the given count of touch points.
        /// </summary>
        /// <param name="count">The count of touch points to generate.</param>
        /// <returns>The generated touch points.</returns>
        public static TouchPoint[] GenerateValidSample(int count)
        {
            count = Math.Min(count, MAX_TOUCHES);

            var touchPoints = new TouchPoint[MAX_TOUCHES];

            for (byte i = 0; i < count; i++)
            {
                touchPoints[i] = new TouchPoint
                {
                    TouchID = i,
                    Position = new(i, i)
                };
            }

            return touchPoints;
        }

        /// <summary>
        ///   Generate Intermediary samples for the given count of touch points. <br/>
        ///   One more touch point is added for each touch point in the sample.
        /// </summary>
        /// <param name="count">The count of touch points to generate.</param>
        /// <returns>The generated touch points.</returns>
        public static TouchPoint[][] GenerateIntermediarySamples(int count)
        {
            count = Math.Min(count, MAX_TOUCHES) - 1;

            var samples = new TouchPoint[count][];

            // generate count samples
            for (byte i = 0; i < count; i++)
            {
                samples[i] = new TouchPoint[MAX_TOUCHES];

                // for 2 fingers, it would be something like
                // 1 0 0 0 0
                // 1 1 0 0 0
                for (byte j = 0; j <= i; j++)
                {
                    samples[i][j] = new TouchPoint
                    {
                        TouchID = j,
                        Position = new(j, j)
                    };
                }
            }

            return samples;
        }
    }
}