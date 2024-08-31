using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Serializables.Gestures;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Reflection;

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

        public BindablePinchGesture(SharedArea? bounds, ISharedBinding binding) : base(bounds)
        {
            Binding = binding;
        }

        public BindablePinchGesture(Rectangle bounds, ISharedBinding binding) : base(bounds)
        {
            Binding = binding;
        }

        public BindablePinchGesture(double distanceThreshold, double angleThreshold, bool isInner, bool isClockwise,
                                    SharedArea? bounds, ISharedBinding binding)
            : base(distanceThreshold, angleThreshold, isInner, isClockwise, bounds)
        {
            Binding = binding;
        }

        public BindablePinchGesture(double distanceThreshold, double angleThreshold, bool isInner, bool isClockwise,
                                    Rectangle bounds, ISharedBinding binding)
            : base(distanceThreshold, angleThreshold, isInner, isClockwise, bounds)
        {
            Binding = binding;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        [JsonProperty]
        public BindingSettingStore? Store { get; set; }

        /// <inheritdoc/>
        public virtual ISharedBinding? Binding { get; set; }

        #endregion

        #region Methods

        protected override void CompleteGesture()
        {
            base.CompleteGesture();
            
            if (Binding != null)
            {
                _ = Task.Run(async () =>
                {
                    Binding.Press();
                    await Task.Delay(15);
                    Binding.Release();
                });
            }
        }

        #endregion

        #region static Methods

        public static BindablePinchGesture? FromSerializable<T>(SerializablePinchGesture? pinchGesture, Dictionary<int, TypeInfo> identifierToPlugin, SharedTabletReference? tablet)
            where T : BindingSettingStore, new()
        {
            if (pinchGesture == null)
                return null;

            if (pinchGesture.PluginProperty == null)
                return null;

            if (!identifierToPlugin.TryGetValue(pinchGesture.PluginProperty.Identifier, out var plugin))
                return null;

            var store = new T();
            store.SetTypeInfo(plugin);

            // Set the values of the plugin property
            if (store.SetValue(plugin, pinchGesture.PluginProperty.Value) == false)
                return null;

            return new BindablePinchGesture(pinchGesture)
            {
                Store = store,
                Binding = BindingBuilder.Current?.Build(store, tablet)
            };
        }

        public static SerializablePinchGesture? ToSerializable(BindablePinchGesture? gesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (gesture == null)
                return null;

            var store = gesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.GetValue(identifier.Value);

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