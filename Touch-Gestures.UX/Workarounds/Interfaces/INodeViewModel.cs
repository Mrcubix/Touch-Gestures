//using Avalonia;
using TouchGestures.Lib.Enums;

namespace TouchGestures.UX.Workarounds.Interfaces
{
    public interface INodeViewModel
    {
        public int Index { get; set; }
        public GestureNodeShape Shape { get; init; }
        public bool IsGestureStart { get; set; }
        public bool IsGestureEnd { get; set; }
        /*
        public Point Position { get; set; }
        public Size Size { get; set; }
        */
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public float Timestamp { get; set; }
        public float TimestampTolerance { get; set; }
        public bool IsHold { get; set; }
        public float HoldDuration { get; set; }
    }
}