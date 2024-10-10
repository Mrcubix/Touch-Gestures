using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Tests.Tablet
{
    public class GenericTouchReport : ITouchReport
    {
        public byte[] Raw { set; get; } = new byte[0];
        public TouchPoint[] Touches { get; init; } = new TouchPoint[0];
    }
}