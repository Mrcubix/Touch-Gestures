using System;
using System.Drawing;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Extensions;

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

        #endregion

        #region Properties

        [JsonProperty]
        public override GestureType Type => GestureType.Tap;

        #endregion

        #region Methods

        protected override void CompleteGesture()
        {
            _delta = Vector2.Zero;
            //HasCompleted = true;
        }

        #endregion

        #region Event Handlers

        /// <inheritdoc/>
        //protected override void OnGestureComplete(GestureEventArgs e)
        //{
            //base.OnGestureComplete(e);
            
        //}

        /// <inheritdoc/>
        public override void OnInput(TouchPoint[] points)
        {
            if (points.Length > 0)
            {
                var point = points[0];

                if (point != null)
                {
                    if (!HasStarted)
                    {
                        if (IsRestrained && _bounds != null && !_bounds.IsZero() && !point.IsInside(_bounds))
                            return;

                        StartPosition = point.Position;
                        IsInvalidState = false;
                        HasStarted = true;
                    }
                    else
                    {
                        if (Deadline != 0 && (DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
                            IsInvalidState = true;

                        if (IsRestrained && _bounds != null && !_bounds.IsZero() && !point.IsInside(_bounds))
                            IsInvalidState = true;

                        if (IsInvalidState)
                        {
                            HasEnded = true;
                            return;
                        }

                        _delta = point.Position - StartPosition;
                        OnDelta();
                    }
                }
                else
                {
                    // finger may have been lifted while the gesture was active
                    if (HasActivated)
                        HasCompleted = true;
                    else if (HasStarted)
                        HasEnded = true;
                }
            }
        }

        #endregion
    }
}