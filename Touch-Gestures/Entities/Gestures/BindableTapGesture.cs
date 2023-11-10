using System.Numerics;
using OpenTabletDriver.Plugin;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TouchGestures.Lib.Interfaces;
using OpenTabletDriver.Desktop.Reflection;
using TouchGestures.Lib.Serializables.Gestures;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace TouchGestures.Entities.Gestures
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

        public BindableTapGesture(SerializableTapGesture tapGesture) : base(tapGesture.Threshold, tapGesture.Deadline, tapGesture.RequiredTouchesCount)
        {
        }

        public BindableTapGesture(Vector2 threshold) : base(threshold)
        {
        }

        public BindableTapGesture(Vector2 threshold, double deadline) : base(threshold, deadline)
        {
        }

        public BindableTapGesture(Vector2 threshold, double deadline, IBinding binding) : this(threshold, deadline)
        {
            Binding = binding;
        }

        public BindableTapGesture(Vector2 threshold, double deadline, int requiredTouchesCount) : this(threshold, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
        }

        public BindableTapGesture(Vector2 threshold, IBinding binding, int requiredTouchesCount) : this(threshold)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        public BindableTapGesture(Vector2 threshold, double deadline, IBinding binding, int requiredTouchesCount) : this(threshold, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
            Binding = binding;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        [JsonProperty("Store")]
        public PluginSettingStore? PluginProperty { get; set; }

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

            var Store = new PluginSettingStore(plugin);

            // Set the values of the plugin property
            Store.Settings.Single(s => s.Property == "Property").SetValue(tapGesture.PluginProperty.Value!);

            return new BindableTapGesture(tapGesture.Threshold, tapGesture.Deadline, tapGesture.RequiredTouchesCount)
            {
                PluginProperty = Store,
                Binding = Store?.Construct<IBinding>()
            };
        }

        #endregion
    }
}