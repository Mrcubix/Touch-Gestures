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
using TouchGestures.Lib.Extensions;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a 1-finger tap gesture.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BindableTapGesture : TapGesture, IBindable
    {
        #region Constructors

        public BindableTapGesture() : base()
        {
        }

        public BindableTapGesture(Rectangle bounds) : base(bounds)
        {
        }

        public BindableTapGesture(SerializableTapGesture tapGesture) : this()
        {
            Bounds = tapGesture.Bounds;
            Deadline = tapGesture.Deadline;
            RequiredTouchesCount = tapGesture.RequiredTouchesCount;
        }

        public BindableTapGesture(Rectangle bounds, double deadline) : this(bounds)
        {
            Deadline = deadline;
        }
            
        public BindableTapGesture(Rectangle bounds, double deadline, IBinding binding) : this(bounds, deadline)
        {
            Binding = binding;
        }

        public BindableTapGesture(Rectangle bounds, double deadline, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
        }

        public BindableTapGesture(Rectangle bounds, IBinding binding, int requiredTouchesCount) : this(bounds)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        public BindableTapGesture(Rectangle bounds, double deadline, IBinding binding, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        #endregion

        #region Properties

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

        public static BindableTapGesture? FromSerializable(SerializableTapGesture? tapGesture, Dictionary<int, TypeInfo> identifierToPlugin)
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

            return new BindableTapGesture(tapGesture)
            {
                Store = store,
                Binding = store?.Construct<IBinding>()
            };
        }

        public static SerializableTapGesture? ToSerializable(BindableTapGesture? tapGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (tapGesture == null)
                return null;

            var store = tapGesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.Settings.FirstOrDefault(x => x.Property == "Property");

            return new SerializableTapGesture(tapGesture)
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