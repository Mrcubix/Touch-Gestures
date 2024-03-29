using System.Numerics;
using OpenTabletDriver.Plugin;
using OTD.EnhancedOutputMode.Tool;

namespace TouchGestures.Extensions
{
    public static class IDriverExtensions
    {
        public static Vector2 GetLPMM(this IDriver driver)
        {
            if (driver == null || driver.Tablet == null)
                return Vector2.One;

            var digitizer = driver.Tablet.Digitizer;

            return new Vector2(digitizer.MaxX / digitizer.Width, digitizer.MaxY / digitizer.Height);
        }

        public static Vector2 GetTouchLPMM(this IDriver driver)
        {
            // the driver or tablet is not available, default to 1 Line per mm for default behavior
            if (driver == null || driver.Tablet == null)
                return Vector2.One;

            var digitizer = driver.Tablet.Digitizer;

            return new Vector2(TouchSettings.maxX / digitizer.Width, TouchSettings.maxY / digitizer.Height);
        }
    }
}