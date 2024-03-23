using OpenTabletDriver.Plugin;
using OTD.EnhancedOutputMode.Tool;

namespace TouchGestures.Extensions
{
    public static class IDriverExtensions
    {
        public static float GetLPMM(this IDriver driver)
        {
            return driver?.Tablet?.Digitizer.MaxX / driver?.Tablet?.Digitizer.Width ?? 1;
        }

        public static float GetTouchLPMM(this IDriver driver)
        {
            // the driver or tablet is not available, default to 1 Line per mm for default behavior
            if (driver == null || driver.Tablet == null)
                return 1;

            return TouchSettings.maxX / driver.Tablet.Digitizer.Width;
        }
    }
}