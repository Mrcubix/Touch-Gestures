using System;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Lib.Input
{
    public class DeviceReportEventArgs(string name, ITouchReport report) : EventArgs
    {

        /// <summary>
        ///   The name of the device
        /// </summary>
        public string Name { get; set; } = name ?? throw new ArgumentNullException(nameof(name));

        /// <summary>
        ///   The emitted or emulated touch report 
        /// </summary>
        public ITouchReport Report { get; set; } = report ?? throw new ArgumentNullException(nameof(report));
    }
}