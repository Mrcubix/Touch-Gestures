using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Extensions;
using TouchGestures.Lib.Serializables.Gestures;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a swipe gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BindablePinchGesture : PinchGesture, IBindable
    {
        #region Constructors

        public BindablePinchGesture() : base()
        {
        }

        public BindablePinchGesture(SharedArea? bounds) : base(bounds)
        {
        }

        public BindablePinchGesture(Rectangle bounds) : base(bounds)
        {
        }

        public BindablePinchGesture(SerializablePinchGesture pinchGesture) : this(pinchGesture.Bounds)
        {
            IsInner = pinchGesture.IsInner;
            IsClockwise = pinchGesture.IsClockwise;

            DistanceThreshold = pinchGesture.DistanceThreshold;
            AngleThreshold = pinchGesture.AngleThreshold;
        }

        public BindablePinchGesture(SharedArea? bounds, IBinding binding) : base(bounds)
        {
            Binding = binding;
        }

        public BindablePinchGesture(Rectangle bounds, IBinding binding) : base(bounds)
        {
            Binding = binding;
        }

        public BindablePinchGesture(double distanceThreshold, double angleThreshold, bool isInner, bool isClockwise,
                                    SharedArea? bounds, IBinding binding)
            : base(distanceThreshold, angleThreshold, isInner, isClockwise, bounds)
        {
            Binding = binding;
        }

        public BindablePinchGesture(double distanceThreshold, double angleThreshold, bool isInner, bool isClockwise,
                                    Rectangle bounds, IBinding binding)
            : base(distanceThreshold, angleThreshold, isInner, isClockwise, bounds)
        {
            Binding = binding;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        [JsonProperty]
        public PluginSettingStore? Store { get; set; }

        /// <inheritdoc/>
        public virtual IBinding? Binding { get; set; }

        #endregion

        #region Methods

        protected override void CompleteGesture()
        {
            base.CompleteGesture();

            if (Binding != null)
            {
                Binding.Press();
                Binding.Release();
            }
        }

        #endregion

        #region static Methods

        public static BindablePinchGesture? FromSerializable(SerializablePinchGesture? pinchGesture, Dictionary<int, TypeInfo> identifierToPlugin, SharedTabletReference? tablet)
        {
            if (pinchGesture == null)
                return null;

            if (pinchGesture.PluginProperty == null)
                return null;

            if (!identifierToPlugin.TryGetValue(pinchGesture.PluginProperty.Identifier, out var plugin))
                return null;

            var store = new PluginSettingStore(plugin);

            // Set the values of the plugin property
            if (store.SetBindingValue(plugin, pinchGesture.PluginProperty.Value) == false)
                return null;

            return new BindablePinchGesture(pinchGesture)
            {
                Store = store,
                Binding = BindingBuilder.Build(store, tablet) as IBinding
            };
        }

        public static SerializablePinchGesture? ToSerializable(BindablePinchGesture? gesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (gesture == null)
                return null;

            var store = gesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.GetBindingValue(identifier.Value);

            return new SerializablePinchGesture(gesture)
            {
                PluginProperty = new SerializablePluginSettings()
                {
                    Identifier = identifier.Key,
                    Value = value
                }
            };
        }

        #endregion
    }
}