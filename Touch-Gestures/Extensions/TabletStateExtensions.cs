using System.Collections.Generic;
using OpenTabletDriver.Plugin.Tablet;
using OTD.EnhancedOutputMode.Lib.Tools;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Extensions
{
    public static class TabletStateExtensions
    {
        public static SharedTabletReference ToShared(this TabletState tablet)
        {
            var digitizer = new SharedTabletDigitizer
            {
                Width = tablet.Digitizer.Width,
                Height = tablet.Digitizer.Height,
                MaxX = tablet.Digitizer.MaxX,
                MaxY = tablet.Digitizer.MaxY
            };

            var touchDigitizer = new SharedTabletDigitizer
            {
                Width = digitizer.Width,
                Height = digitizer.Height,
                MaxX = TouchSettings.maxX,
                MaxY = TouchSettings.maxY
            };

            var featureInitReport = tablet.Auxiliary.FeatureInitReport == null
                ? new List<byte[]>()
                : new List<byte[]> { tablet.Auxiliary.FeatureInitReport };

            var outputInitReport = tablet.Auxiliary.OutputInitReport == null
                ? new List<byte[]>()
                : new List<byte[]> { tablet.Auxiliary.OutputInitReport };

            var identifier = new SharedDeviceIdentifier
            {
                VendorID = tablet.Auxiliary.VendorID,
                ProductID = tablet.Auxiliary.ProductID,
                InputReportLength = tablet.Auxiliary.InputReportLength,
                OutputReportLength = tablet.Auxiliary.OutputReportLength,
                ReportParser = tablet.Auxiliary.ReportParser,
                FeatureInitReport = featureInitReport,
                OutputInitReport = outputInitReport,
                DeviceStrings = tablet.Auxiliary.DeviceStrings,
                InitializationStrings = tablet.Auxiliary.InitializationStrings
            };

            return new SharedTabletReference(tablet.TabletProperties.Name, digitizer, touchDigitizer, identifier);
        }
    }
}