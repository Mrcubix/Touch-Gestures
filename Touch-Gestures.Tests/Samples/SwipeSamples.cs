using System.Numerics;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Tests.Samples
{
    public static class SwipeSamples
    {
        private const int MAX_TOUCHES = 1;
        
        #region Sample Data

        public static readonly TouchPoint[] OriginSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(0, 0),
            },
        };

        public static readonly TouchPoint[] OriginAbsoluteFaultyData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(-201, -201),
            },
        };

        public static readonly TouchPoint[] LeftSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(-30, 0),
            },
        };

        public static readonly TouchPoint[] UpSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(0, -30),
            },
        };

        public static readonly TouchPoint[] RightSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(30, 0),
            },
        };

        public static readonly TouchPoint[] DownSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(0, 30),
            },
        };

        public static readonly TouchPoint[] UpLeftSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(-30, -30),
            },
        };

        public static readonly TouchPoint[] UpRightSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(30, -30),
            },
        };

        public static readonly TouchPoint[] DownLeftSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(-30, 30),
            },
        };

        public static readonly TouchPoint[] DownRightSwipeSampleData = new TouchPoint[MAX_TOUCHES]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(30, 30),
            },
        };

        public static readonly TouchPoint[] ReleasedSampleData = new TouchPoint[MAX_TOUCHES]
        {
            null!,
        };

        #endregion
    }
}