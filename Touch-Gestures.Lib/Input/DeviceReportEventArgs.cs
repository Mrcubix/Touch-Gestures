using System;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Lib.Input
{
    public class DeviceReportEventArgs : EventArgs
    {
        public DeviceReportEventArgs(string name, ITouchReport report)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Report = report ?? throw new ArgumentNullException(nameof(report));
        }

        /// <summary>
        ///   The name of the device
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   The emitted or emulated touch report 
        /// </summary>
        public ITouchReport Report { get; set; }
    }
}