using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Extensions;
using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Serializables.Gestures;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a swipe gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BindablePanGesture : PanGesture, IBindable
    {
        #region Constructors

        public BindablePanGesture() : base()
        {
        }

        public BindablePanGesture(SerializablePanGesture swipeGesture)
            : base(swipeGesture.Threshold, swipeGesture.Deadline, swipeGesture.Direction, swipeGesture.Bounds)
        {
        }

        public BindablePanGesture(Vector2 threshold) : base(threshold)
        {
        }

        public BindablePanGesture(double deadline) : base(deadline)
        {
        }

        public BindablePanGesture(SwipeDirection direction) : base(direction)
        {
        }

        public BindablePanGesture(Vector2 threshold, double deadline) : base(threshold, deadline)
        {
        }

        public BindablePanGesture(Vector2 threshold, double deadline, SwipeDirection direction) : base(threshold, deadline, direction)
        {
        }

        public BindablePanGesture(Vector2 threshold, double deadline, IBinding binding) : this(threshold, deadline)
        {
            Binding = binding;
        }

        public BindablePanGesture(Vector2 threshold, double deadline, SwipeDirection direction, IBinding binding) : this(threshold, deadline, direction)
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

        public static BindablePanGesture? FromSerializable(SerializablePanGesture? swipeGesture, Dictionary<int, TypeInfo> identifierToPlugin, SharedTabletReference? tablet)
        {
            if (swipeGesture == null)
                return null;

            if (swipeGesture.PluginProperty == null)
                return null;

            if (!identifierToPlugin.TryGetValue(swipeGesture.PluginProperty.Identifier, out var plugin))
                return null;

            var store = new PluginSettingStore(plugin);

            // Set the values of the plugin property
            if (store.SetBindingValue(plugin, swipeGesture.PluginProperty.Value) == false)
                return null;

            return new BindablePanGesture(swipeGesture)
            {
                Store = store,
                Binding = BindingBuilder.Build(store, tablet) as IBinding
            };
        }

        public static SerializablePanGesture? ToSerializable(BindablePanGesture? swipeGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (swipeGesture == null)
                return null;

            var store = swipeGesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.GetBindingValue(identifier.Value);

            return new SerializablePanGesture(swipeGesture)
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