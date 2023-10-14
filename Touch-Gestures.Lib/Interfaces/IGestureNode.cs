using System.Numerics;
using TouchGestures.Lib.Enums;

namespace TouchGestures.Lib.Interfaces
{
    public interface IGestureNode
    {
        public int Index { get; set; }
        public GestureNodeShape Shape { get; init; }
        public bool IsGestureStart { get; set; }
        public bool IsGestureEnd { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public float Timestamp { get; set; }
        public float TimestampTolerance { get; set; }
        public bool IsHold { get; set; }
        public float HoldDuration { get; set; }

        public bool IsWithinNode(Vector2 position, float timestamp);

        // A node may be dragged
        // A node may be resized
    }
}