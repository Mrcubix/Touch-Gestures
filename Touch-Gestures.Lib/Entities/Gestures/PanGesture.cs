using System.Drawing;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using TouchGestures.Lib.Enums;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a pan gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PanGesture : SwipeGesture
    {
        #region Constructors

        public PanGesture() : base() { }

        public PanGesture(Vector2 threshold) : base(threshold) { }

        public PanGesture(double deadline) : base(deadline) { }

        public PanGesture(Vector2 threshold, double deadline) : base(threshold, deadline) { }

        public PanGesture(SwipeDirection direction) : base(direction) { }

        public PanGesture(Vector2 threshold, double deadline, SwipeDirection direction)
            : base(threshold, deadline, direction) { }

        public PanGesture(Vector2 threshold, double deadline, SwipeDirection direction, Rectangle bounds)
            : base(threshold, deadline, direction, bounds) { }

        public PanGesture(Vector2 threshold, double deadline, SwipeDirection direction, SharedArea? bounds)
            : base(threshold, deadline, direction, bounds) { }

        public PanGesture(Vector2 threshold, double deadline, SwipeDirection direction, Rectangle bounds, int requiredTouchesCount)
            : base(threshold, deadline, direction, bounds, requiredTouchesCount) { }

        public PanGesture(Vector2 threshold, double deadline, SwipeDirection direction, SharedArea? bounds, int requiredTouchesCount)
            : base(threshold, deadline, direction, bounds, requiredTouchesCount) { }

        #endregion

        #region Properties

        [JsonProperty]
        public override GestureType Type => GestureType.Pan;

        public override string DisplayName => $"{RequiredTouchesCount}-Touch {Direction} Pan";

        #endregion

        #region Methods

        protected override void CompleteGesture()
        {
            HasActivated = true;
            StartPosition = _lastPosition;

            if (Binding != null)
            {
                Binding.Press(null!);
                Binding.Release(null!);
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnInputCore()
        {
            if (IsInvalidState == false)
                OnDelta();

            // Wait for all touches to be released, or else, it will just start again on the next input and complete on the next release
            if (_releasedPoints.All(released => released) && _currentPoints.Count == 0)
                if (HasActivated)
                    HasCompleted = true;
                else
                    HasEnded = true;
        }

        #endregion
    }
}