using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Reflection;

namespace TouchGestures.Entities
{
    public class BulletproofSharedTabletReference : SharedTabletReference
    {
        public BulletproofSharedTabletReference() : base()
        {
            ServiceProvider = new PointerManager();
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