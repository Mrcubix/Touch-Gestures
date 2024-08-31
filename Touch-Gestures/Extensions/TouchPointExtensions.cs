using TouchGestures.Lib.Entities.Tablet.Touch;
using OTDTouchPoint = OpenTabletDriver.Plugin.Tablet.Touch.TouchPoint;

namespace TouchGestures.Extensions
{
    public static class TouchPointExtensions
    {
        public static TouchPoint FromOTD(this OTDTouchPoint point)
        {
            return new TouchPoint(point.TouchID, point.Position);
        }

        public static OTDTouchPoint ToOTD(this TouchPoint point)
        {
            return new OTDTouchPoint
            {
                TouchID = point.TouchID,
                Position = point.Position
            };
        }

        public static TouchPoint[] FromOTD(this OTDTouchPoint[] points)
        {
            TouchPoint[] result = new TouchPoint[points.Length];

            for (int i = 0; i < points.Length; i++)
                result[i] = points[i].FromOTD();

            return result;
        }

        public static OTDTouchPoint[] ToOTD(this TouchPoint[] points)
        {
            OTDTouchPoint[] result = new OTDTouchPoint[points.Length];

            for (int i = 0; i < points.Length; i++)
                result[i] = points[i].ToOTD();
                
            return result;
        }
    }
}