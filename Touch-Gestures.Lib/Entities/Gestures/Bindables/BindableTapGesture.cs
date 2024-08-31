using System.Threading.Tasks;
using Newtonsoft.Json;
using TouchGestures.Lib.Interfaces;
using TouchGestures.Lib.Serializables.Gestures;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using OpenTabletDriver.External.Common.Serializables;
using System.Drawing;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Reflection;

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
            
        public BindableTapGesture(Rectangle bounds, double deadline, ISharedBinding binding) : this(bounds, deadline)
        {
            Binding = binding;
        }

        public BindableTapGesture(Rectangle bounds, double deadline, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
        }

        public BindableTapGesture(Rectangle bounds, ISharedBinding binding, int requiredTouchesCount) : this(bounds)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        public BindableTapGesture(Rectangle bounds, double deadline, ISharedBinding binding, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        [JsonProperty]
        public BindingSettingStore? Store { get; set; }

        /// <inheritdoc/>
        public ISharedBinding? Binding { get; set; }

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

        public static BindableTapGesture? FromSerializable<T>(SerializableTapGesture? tapGesture, Dictionary<int, TypeInfo> identifierToPlugin, SharedTabletReference? tablet)
            where T : BindingSettingStore, new()
        {
            if (tapGesture == null)
                return null;

            if (tapGesture.PluginProperty == null)
                return null;

            if (!identifierToPlugin.TryGetValue(tapGesture.PluginProperty.Identifier, out var plugin))
                return null;

            var store = new T();
            store.SetTypeInfo(plugin);

            if (store.SetValue(plugin, tapGesture.PluginProperty.Value) == false)
                return null;

            return new BindableTapGesture(tapGesture)
            {
                Store = store,
                Binding = BindingBuilder.Current?.Build(store, tablet)
            };
        }

        public static SerializableTapGesture? ToSerializable(BindableTapGesture? tapGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (tapGesture == null)
                return null;

            var store = tapGesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.GetValue(identifier.Value);

            return new SerializableTapGesture(tapGesture)
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