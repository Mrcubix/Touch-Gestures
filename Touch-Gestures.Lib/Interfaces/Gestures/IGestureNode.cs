using System.Numerics;
using TouchGestures.Lib.Enums;

namespace TouchGestures.Lib.Interfaces
{
    public interface IGestureNode
    {
        int Index { get; }
        GestureNodeShape Shape { get; init; }
        bool IsGestureStart { get; }
        bool IsGestureEnd { get; }
        Vector2 Position { get; }
        Vector2 Size { get; }
        float Timestamp { get; }
        float TimestampTolerance { get; }
        bool IsHold { get; }
        float HoldDuration { get; }

        bool IsWithinNode(Vector2 position, float timestamp);
    }
}