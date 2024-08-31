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
    public class BindableHoldGesture : HoldGesture, IBindable
    {
        #region Constructors

        #region Base Constructors

        public BindableHoldGesture() : base()
        {
        }

        public BindableHoldGesture(Rectangle bounds) : base(bounds)
        {
        }

        public BindableHoldGesture(Rectangle bounds, double deadline) : base(bounds, deadline)
        {
        }

        public BindableHoldGesture(SharedArea? sharedArea, double deadline, int requiredTouchesCount) : this()
        {
            Bounds = sharedArea;
            Deadline = deadline;
            RequiredTouchesCount = requiredTouchesCount;
        }

        #endregion

        public BindableHoldGesture(SerializableHoldGesture tapGesture) 
            : this(tapGesture.Bounds, tapGesture.Deadline, tapGesture.RequiredTouchesCount) 
        {
            Threshold = tapGesture.Threshold;
        }

        public BindableHoldGesture(Rectangle bounds, double deadline, ISharedBinding binding) : this(bounds, deadline)
        {
            Binding = binding;
        }

        public BindableHoldGesture(Rectangle bounds, double deadline, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
        }

        public BindableHoldGesture(Rectangle bounds, ISharedBinding binding, int requiredTouchesCount) : this(bounds)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        public BindableHoldGesture(Rectangle bounds, double deadline, ISharedBinding binding, int requiredTouchesCount) : this(bounds, deadline)
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

        protected override void Press()
        {
            base.Press();

            Binding?.Press();
        }

        protected override void Release()
        {
            Binding?.Release();

            base.Release();
        }

        #endregion

        #region static Methods

        public static BindableHoldGesture? FromSerializable<T>(SerializableHoldGesture? holdGesture, Dictionary<int, TypeInfo> identifierToPlugin, SharedTabletReference? tablet)
            where T : BindingSettingStore, new()
        {
            if (holdGesture == null)
                return null;

            if (holdGesture.PluginProperty == null)
                return null;

            if (!identifierToPlugin.TryGetValue(holdGesture.PluginProperty.Identifier, out var plugin))
                return null;

            var store = new T();
            store.SetTypeInfo(plugin);

            // Set the values of the plugin property
            if (store.SetValue(plugin, holdGesture.PluginProperty.Value) == false)
                return null;

            return new BindableHoldGesture(holdGesture)
            {
                Store = store,
                Binding = BindingBuilder.Current?.Build(store, tablet)
            };
        }

        public static SerializableHoldGesture? ToSerializable(BindableHoldGesture? holdGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (holdGesture == null)
                return null;

            var store = holdGesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.GetValue(identifier.Value);

            return new SerializableHoldGesture(holdGesture)
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