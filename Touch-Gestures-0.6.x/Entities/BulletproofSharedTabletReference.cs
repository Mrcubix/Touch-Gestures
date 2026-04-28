using OpenTabletDriver.Desktop.Reflection;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Entities
{
    public class BulletproofSharedTabletReference : SharedTabletReference
    {
        public BulletproofSharedTabletReference() : base()
        {
            ServiceProvider = new ServiceManager();
        }

        public BulletproofSharedTabletReference(string name, SharedTabletDigitizer digitizer,
                                                SharedTabletDigitizer touchDigitizer, IServiceManager serviceProvider)
            : base(name, digitizer, touchDigitizer)
        {
            ServiceProvider = serviceProvider;
        }

        public BulletproofSharedTabletReference(string name, SharedTabletDigitizer digitizer,
                                                SharedTabletDigitizer touchDigitizer, SharedDeviceIdentifier deviceIdentifier,
                                                IServiceManager serviceProvider)
            : base(name, digitizer, touchDigitizer, deviceIdentifier)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceManager ServiceProvider { set; get; }
    }
}