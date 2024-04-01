using System.Collections.Generic;
using OpenTabletDriver.Plugin.Tablet;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Extensions
{
    public static class SharedTabletReferenceExtension
    {
        public static TabletReference ToState(this SharedTabletReference tablet)
        {
            var properties = new TabletConfiguration()
            {
                Name = tablet.Name
            };

            if (tablet.PenDigitizer != null)
            {
                properties.Specifications = new TabletSpecifications()
                {
                    Digitizer = new DigitizerSpecifications()
                    {
                        Width = tablet.PenDigitizer.Width,
                        Height = tablet.PenDigitizer.Height,
                        MaxX = tablet.PenDigitizer.MaxX,
                        MaxY = tablet.PenDigitizer.MaxY
                    }
                };
            }

            List<DeviceIdentifier> identifiers = new List<DeviceIdentifier>();

            if (tablet.DeviceIdentifier != null)
            {
                identifiers.Add(new DeviceIdentifier()
                {
                    VendorID = tablet.DeviceIdentifier.VendorID,
                    ProductID = tablet.DeviceIdentifier.ProductID,
                    InputReportLength = tablet.DeviceIdentifier.InputReportLength,
                    OutputReportLength = tablet.DeviceIdentifier.OutputReportLength,
                    ReportParser = tablet.DeviceIdentifier.ReportParser,
                    FeatureInitReport = tablet.DeviceIdentifier.FeatureInitReport,
                    OutputInitReport = tablet.DeviceIdentifier.OutputInitReport,
                    DeviceStrings = tablet.DeviceIdentifier.DeviceStrings,
                    InitializationStrings = tablet.DeviceIdentifier.InitializationStrings
                });
            }

            return new TabletReference(properties, identifiers);
        }
    }
}