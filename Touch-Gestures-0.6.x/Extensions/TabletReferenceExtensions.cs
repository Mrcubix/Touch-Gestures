using System.Numerics;
using OpenTabletDriver.Plugin.Tablet;
using OTD.EnhancedOutputMode.Lib.Tools;
using TouchGestures.Entities;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Extensions
{
    public static class TabletReferenceExtensions
    {
        public static SharedTabletReference ToShared(this TabletReference tablet, TouchSettings touchSettings)
        {
            var digitizer = tablet.Properties.Specifications.Digitizer;

            var penDigitizer = new SharedTabletDigitizer
            {
                Width = digitizer.Width,
                Height = digitizer.Height,
                MaxX = digitizer.MaxX,
                MaxY = digitizer.MaxY
            };

            var touchDigitizer = new SharedTabletDigitizer
            {
                Width = digitizer.Width,
                Height = digitizer.Height,
                MaxX = touchSettings.MaxX,
                MaxY = touchSettings.MaxY
            };

            return new BulletproofSharedTabletReference(tablet.Properties.Name, penDigitizer, touchDigitizer, new PointerManager());
        }

        public static Vector2 GetLPMM(this TabletReference tabletReference)
        {
            var digitizer = tabletReference.Properties.Specifications.Digitizer;

            return new Vector2(digitizer.MaxX / digitizer.Width, digitizer.MaxY / digitizer.Height);
        }

        public static Vector2 GetTouchLPMM(this TabletReference tabletReference, TouchSettings touchSettings)
        {
            var digitizer = tabletReference.Properties.Specifications.Digitizer;

            return new Vector2(touchSettings.MaxX / digitizer.Width, touchSettings.MaxY / digitizer.Height);
        }
    }
}