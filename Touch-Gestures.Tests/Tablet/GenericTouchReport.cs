using System;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Tests.Tablet
{
    public class GenericTouchReport : ITouchReport
    {
        public byte[] Raw { set; get; } = Array.Empty<byte>();
        public TouchPoint[] Touches { get; init; } = Array.Empty<TouchPoint>();
    }
}