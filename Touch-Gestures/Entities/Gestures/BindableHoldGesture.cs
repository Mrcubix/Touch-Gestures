using OpenTabletDriver.Plugin;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TouchGestures.Lib.Interfaces;
using OpenTabletDriver.Desktop.Reflection;
using TouchGestures.Lib.Serializables.Gestures;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using OpenTabletDriver.External.Common.Serializables;
using System.Drawing;

namespace TouchGestures.Entities.Gestures
{
    /// <summary>
    ///   Represent a 1-finger tap gesture.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BindableHoldGesture : HoldGesture, IBindable
    {
        #region Constructors

        public BindableHoldGesture() : base()
        {
        }

        public BindableHoldGesture(SerializableHoldGesture tapGesture) : base(tapGesture.Bounds, tapGesture.Deadline, tapGesture.RequiredTouchesCount)
        {
        }

        public BindableHoldGesture(Rectangle bounds) : base(bounds)
        {
        }

        public BindableHoldGesture(Rectangle bounds, double deadline) : base(bounds, deadline)
        {
        }

        public BindableHoldGesture(Rectangle bounds, double deadline, IBinding binding) : this(bounds, deadline)
        {
            Binding = binding;
        }

        public BindableHoldGesture(Rectangle bounds, double deadline, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
        }

        public BindableHoldGesture(Rectangle bounds, IBinding binding, int requiredTouchesCount) : this(bounds)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        public BindableHoldGesture(Rectangle bounds, double deadline, IBinding binding, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        #endregion

        #region Properties

        public override float LinesPerMM => Info.Driver.Tablet.Digitizer.MaxX / Info.Driver.Tablet.Digitizer.Width;

        /// <inheritdoc/>
        [JsonProperty]
        public PluginSettingStore? Store { get; set; }

        /// <inheritdoc/>
        public IBinding? Binding { get; set; }

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

        public static BindableHoldGesture? FromSerializable(SerializableHoldGesture? tapGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (tapGesture == null)
                return null;

            if (tapGesture.PluginProperty == null)
                return null;

            if (!identifierToPlugin.TryGetValue(tapGesture.PluginProperty.Identifier, out var plugin))
                return null;

            var store = new PluginSettingStore(plugin);

            // Set the values of the plugin property
            store.Settings.Single(s => s.Property == "Property").SetValue(tapGesture.PluginProperty.Value!);

            return new BindableHoldGesture(tapGesture)
            {
                Store = store,
                Binding = store?.Construct<IBinding>()
            };
        }

        public static SerializableHoldGesture? ToSerializable(BindableHoldGesture? tapGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (tapGesture == null)
                return null;

            var store = tapGesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.Settings.FirstOrDefault(x => x.Property == "Property");

            return new SerializableHoldGesture(tapGesture)
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