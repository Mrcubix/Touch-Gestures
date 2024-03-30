using System;
using OpenTabletDriver.Plugin.Tablet;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Extensions
{
    public static class SharedTabletReferenceExtension
    {
        public static TabletState ToState(this SharedTabletReference tablet)
        {
            var properties = new TabletConfiguration()
            {
                Name = tablet.Name
            };

            DigitizerIdentifier digitizer = null!;

            if (tablet.PenDigitizer != null)
            {
                digitizer = new DigitizerIdentifier()
                {
                    Width = tablet.PenDigitizer.Width,
                    Height = tablet.PenDigitizer.Height,
                    MaxX = tablet.PenDigitizer.MaxX,
                    MaxY = tablet.PenDigitizer.MaxY
                };
            }

            DeviceIdentifier identifier = null!;

            if (tablet.DeviceIdentifier != null)
            {
                identifier = new DeviceIdentifier()
                {
                    VendorID = tablet.DeviceIdentifier.VendorID,
                    ProductID = tablet.DeviceIdentifier.ProductID,
                    InputReportLength = tablet.DeviceIdentifier.InputReportLength,
                    OutputReportLength = tablet.DeviceIdentifier.OutputReportLength,
                    ReportParser = tablet.DeviceIdentifier.ReportParser,
                    FeatureInitReport = tablet.DeviceIdentifier.FeatureInitReport?[0] ?? Array.Empty<byte>(),
                    OutputInitReport = tablet.DeviceIdentifier.OutputInitReport?[0] ?? Array.Empty<byte>(),
                    DeviceStrings = tablet.DeviceIdentifier.DeviceStrings,
                    InitializationStrings = tablet.DeviceIdentifier.InitializationStrings
                };
            }

            return new TabletState(properties, digitizer, identifier);
        }
    }
}