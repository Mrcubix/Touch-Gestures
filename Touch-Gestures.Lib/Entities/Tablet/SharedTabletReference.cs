using System.Numerics;
using Newtonsoft.Json;

namespace TouchGestures.Lib.Entities.Tablet
{
    [JsonObject(MemberSerialization.OptIn)]
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
        [JsonProperty]
        public string Name { set; get; } = string.Empty;

        /// <summary>
        ///   The device identifier of the tablet.
        /// </summary>
        [JsonProperty]
        public SharedDeviceIdentifier? DeviceIdentifier { set; get; } = null;

        /// <summary>
        ///   The Pen digitizer specifications of the tablet.
        /// </summary>
        [JsonProperty]
        public SharedTabletDigitizer? PenDigitizer { set; get; } = null;

        /// <summary>
        ///   The Touch digitizer specifications of the tablet.
        /// </summary>
        [JsonProperty]
        public SharedTabletDigitizer? TouchDigitizer { set; get; } = null;

        public Vector2 Size => new(PenDigitizer!.Width, PenDigitizer!.Height);
    }
}