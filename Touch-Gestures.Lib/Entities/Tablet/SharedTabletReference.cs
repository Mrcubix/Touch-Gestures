namespace TouchGestures.Lib.Entities.Tablet
{
    public class SharedTabletReference
    {
        public SharedTabletReference()
        {
        }

        public SharedTabletReference(string name, SharedTabletDigitizer digitizer, SharedTabletDigitizer touchDigitizer)
        {
            Name = name;
            PenDigitizer = digitizer;
            TouchDigitizer = touchDigitizer;
        }

        public SharedTabletReference(string name, SharedTabletDigitizer digitizer, SharedTabletDigitizer touchDigitizer, SharedDeviceIdentifier deviceIdentifier)
        {
            Name = name;
            PenDigitizer = digitizer;
            TouchDigitizer = touchDigitizer;
            DeviceIdentifier = deviceIdentifier;
        }

        /// <summary>
        ///   The name of the tablet.
        /// </summary>
        public string Name { set; get; } = string.Empty;

        /// <summary>
        ///   The device identifier of the tablet.
        /// </summary>
        public SharedDeviceIdentifier? DeviceIdentifier { set; get; } = null;

        /// <summary>
        ///   The Pen digitizer specifications of the tablet.
        /// </summary>
        public SharedTabletDigitizer? PenDigitizer { set; get; } = null;

        /// <summary>
        ///   The Touch digitizer specifications of the tablet.
        /// </summary>
        public SharedTabletDigitizer? TouchDigitizer { set; get; } = null;
    }
}