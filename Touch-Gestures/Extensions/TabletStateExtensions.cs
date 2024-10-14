using System;
using System.Collections.Generic;
using OpenTabletDriver.Plugin.Tablet;
using OTD.EnhancedOutputMode.Lib.Tools;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Extensions
{
    public static class TabletStateExtensions
    {
        public static SharedTabletReference ToShared(this TabletState tablet, TouchSettings touchSettings)
        {
            if (tablet == null || tablet.Digitizer == null)
                throw new ArgumentNullException(nameof(tablet));

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
                MaxX = touchSettings.MaxX,
                MaxY = touchSettings.MaxY
            };

            var featureInitReport = tablet.Digitizer.FeatureInitReport == null
            ? new List<byte[]>()
            : new List<byte[]> { tablet.Digitizer.FeatureInitReport };

            var outputInitReport = tablet.Digitizer.OutputInitReport == null
                ? new List<byte[]>()
                : new List<byte[]> { tablet.Digitizer.OutputInitReport };

            var identifier = new SharedDeviceIdentifier
            {
                VendorID = tablet.Digitizer.VendorID,
                ProductID = tablet.Digitizer.ProductID,
                InputReportLength = tablet.Digitizer.InputReportLength,
                OutputReportLength = tablet.Digitizer.OutputReportLength,
                ReportParser = tablet.Digitizer.ReportParser,
                FeatureInitReport = featureInitReport,
                OutputInitReport = outputInitReport,
                DeviceStrings = tablet.Digitizer.DeviceStrings,
                InitializationStrings = tablet.Digitizer.InitializationStrings
            };

            return new SharedTabletReference(tablet.TabletProperties.Name, digitizer, touchDigitizer, identifier);
        }
    }
}