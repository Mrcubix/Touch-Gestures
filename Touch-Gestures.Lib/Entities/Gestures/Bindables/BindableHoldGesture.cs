using OpenTabletDriver.Plugin;
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
using TouchGestures.Lib.Entities.Tablet;

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

        /// <inheritdoc/>
        [JsonProperty]
        public PluginSettingStore? Store { get; set; }

        /// <inheritdoc/>
        public IBinding? Binding { get; set; }

        #endregion

        #region Methods

        protected override void Press()
        {
            base.Press();
            Binding?.Press();
        }

        protected override void Release()
        {
            if (IsPressing)
                Binding?.Release();

            base.Release();
        }

        #endregion

        #region static Methods

        public static BindableHoldGesture? FromSerializable(SerializableHoldGesture? holdGesture, Dictionary<int, TypeInfo> identifierToPlugin, SharedTabletReference? tablet)
        {
            if (holdGesture == null)
                return null;

            if (holdGesture.PluginProperty == null)
                return null;

            if (!identifierToPlugin.TryGetValue(holdGesture.PluginProperty.Identifier, out var plugin))
                return null;

            var store = new PluginSettingStore(plugin);

            // Set the values of the plugin property
            if (store.SetBindingValue(plugin, holdGesture.PluginProperty.Value) == false)
                return null;

            return new BindableHoldGesture(holdGesture)
            {
                Store = store,
                Binding = BindingBuilder.Build(store, tablet) as IBinding
            };
        }

        public static SerializableHoldGesture? ToSerializable(BindableHoldGesture? holdGesture, Dictionary<int, TypeInfo> identifierToPlugin)
        {
            if (holdGesture == null)
                return null;

            var store = holdGesture.Store;

            var identifier = identifierToPlugin.FirstOrDefault(x => x.Value.FullName == store?.Path);
            var value = store?.GetBindingValue(identifier.Value);

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