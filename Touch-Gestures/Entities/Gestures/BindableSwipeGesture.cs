using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Entities.Gestures
{
    /// <summary>
    ///   Represent a swipe gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BindableSwipeGesture : SwipeGesture, IBindable
    {
        #region Constructors

        public BindableSwipeGesture() : base()
        {
        }

        public BindableSwipeGesture(Vector2 threshold) : base(threshold)
        {
        }

        public BindableSwipeGesture(double deadline) : base(deadline)
        {
        }

        public BindableSwipeGesture(Vector2 threshold, double deadline) : base(threshold, deadline)
        {
        }

        public BindableSwipeGesture(SwipeDirection direction) : base(direction)
        {
        }

        public BindableSwipeGesture(Vector2 threshold, double deadline, IBinding binding) : base(threshold, deadline)
        {
            Binding = binding;
        }

        public BindableSwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction) : base(threshold, deadline, direction)
        {
        }

        public BindableSwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction, IBinding binding) : base(threshold, deadline, direction)
        {
            Binding = binding;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        [JsonProperty("Store")]
        public PluginSettingStore? PluginProperty { get; set; }

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
    }
}