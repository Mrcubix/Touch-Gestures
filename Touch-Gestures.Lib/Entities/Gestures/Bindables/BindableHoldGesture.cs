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
            LinesPerMM = Info.Driver.GetTouchLPMM();
        }

        public BindableHoldGesture(Rectangle bounds) : base(bounds)
        {
            LinesPerMM = Info.Driver.GetTouchLPMM();
        }

        public BindableHoldGesture(Rectangle bounds, double deadline) : base(bounds, deadline)
        {
            LinesPerMM = Info.Driver.GetTouchLPMM();
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

#if !OTD06
        /// <inheritdoc/>
        public IBinding? Binding { get; set; }
#else
        /// <inheritdoc/>
        public IStateBinding? Binding { get; set; }
#endif

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