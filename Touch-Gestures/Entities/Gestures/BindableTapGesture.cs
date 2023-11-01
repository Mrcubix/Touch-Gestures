using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Input;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Interfaces;
using OpenTabletDriver.Desktop.Reflection;

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
    }
}