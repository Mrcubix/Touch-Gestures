using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Tests.Samples
{
    public static class TapSamples
    {
        private const int MAX_TOUCHES = 3;

        #region Sample Data

        // This is a simple one touch point, as if the user was dragging or tapping.
        public static readonly TouchPoint[] NonGestureTouchPoints = new TouchPoint[MAX_TOUCHES]
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
        public static readonly TouchPoint[] ValidGestureTouchPoints = new TouchPoint[MAX_TOUCHES]
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
        public static readonly TouchPoint[] ValidGestureTouchPointsIntermediaryState = new TouchPoint[MAX_TOUCHES]
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
        public static readonly TouchPoint[] ValidGestureTouchPointsFinalState = new TouchPoint[MAX_TOUCHES]
        {
            null!,
            null!,
            null!,
        };

        // The gesture should be ended when passing such touch points, considered as invalid.
        public static readonly TouchPoint[] InvalidationGestureTouchPoints = new TouchPoint[MAX_TOUCHES]
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
        public static readonly TouchPoint[] OutOfBoundsTouchPoints = new TouchPoint[MAX_TOUCHES]
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
        public static readonly TouchPoint[] OutOfBoundsTouchPointsIntermediaryState = new TouchPoint[MAX_TOUCHES]
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
    }
}