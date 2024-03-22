using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Serializables.Gestures;

namespace TouchGestures.Entities.Gestures
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

        public BindablePanGesture(SerializablePanGesture swipeGesture) : base(swipeGesture.Threshold, swipeGesture.Deadline, swipeGesture.Direction, swipeGesture.Bounds)
        {
        }

        public BindablePanGesture(Vector2 threshold) : base(threshold)
        {
        }

        public BindablePanGesture(double deadline) : base(deadline)
        {
        }

        public BindablePanGesture(Vector2 threshold, double deadline) : base(threshold, deadline)
        {
        }

        public BindablePanGesture(SwipeDirection direction) : base(direction)
        {
        }

        public BindablePanGesture(Vector2 threshold, double deadline, IBinding binding) : base(threshold, deadline)
        {
            Binding = binding;
        }

        public BindablePanGesture(Vector2 threshold, double deadline, SwipeDirection direction) : base(threshold, deadline, direction)
        {
        }

        public BindablePanGesture(Vector2 threshold, double deadline, SwipeDirection direction, IBinding binding) : base(threshold, deadline, direction)
        {
            Binding = binding;
        }

        #endregion

        #region Properties

        public override float LinesPerMM => Info.Driver.Tablet.Digitizer.MaxX / Info.Driver.Tablet.Digitizer.Width;

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

        public static BindablePanGesture? FromSerializable(SerializablePanGesture? swipeGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (swipeGesture == null)
                return null;

            if (swipeGesture.PluginProperty == null)
                return null;

            if (!identifierToPlugin.TryGetValue(swipeGesture.PluginProperty.Identifier, out var plugin))
                return null;

            var store = new PluginSettingStore(plugin);

            // Set the values of the plugin property
            store.Settings.Single(s => s.Property == "Property").SetValue(swipeGesture.PluginProperty.Value!);

            return new BindablePanGesture(swipeGesture)
            {
                Store = store,
                Bounds = swipeGesture.Bounds,
                Binding = store?.Construct<IBinding>()
            };
        }

        public static SerializablePanGesture? ToSerializable(BindableSwipeGesture? swipeGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (swipeGesture == null)
                return null;

            var store = swipeGesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.Settings.FirstOrDefault(x => x.Property == "Property");

            return new SerializablePanGesture(swipeGesture)
            {
                PluginProperty = new SerializablePluginSettings()
                {
                    Identifier = identifier.Key,
                    Value = value?.GetValue<string?>()
                }
            };
        }

        #endregion
    }
}