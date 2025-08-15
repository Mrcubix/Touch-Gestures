using System;
using OpenTabletDriver.Plugin.Tablet.Touch;

#nullable disable

namespace TouchGestures.Lib.Entities.Tablet
{
    public class TouchReport : ITouchReport
    {
        public TouchReport() { }

        public TouchReport(byte count = 0)
        {
            Touches = new TouchPoint[count];

            for (int i = 0; i < count; i++)
                Touches[i] = new TouchPoint();
        }

        public TouchPoint[] Touches { get; set; } = Array.Empty<TouchPoint>();
        public byte[] Raw { get; set; }
    }
}