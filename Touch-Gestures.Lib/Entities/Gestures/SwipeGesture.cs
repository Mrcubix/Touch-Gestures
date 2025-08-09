using System;
using System.Drawing;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Extensions;
using TouchGestures.Lib.Input;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a swipe gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SwipeGesture : MixedBasedGesture
    {
        #region Fields

        protected bool _hasStarted = false;
        protected bool _hasActivated = false;
        protected bool _hasEnded = false;
        protected bool _hasCompleted = false;

        protected Vector2 _delta = Vector2.Zero;

        #endregion

        #region Constructors

        public SwipeGesture() : base(1000)
        {
            IsRestrained = true;

            GestureStarted += (_, args) => OnGestureStart(args);
            GestureActivated += (_, args) => OnGestureActive(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);
        }

        public SwipeGesture(Vector2 threshold) : this()
        {
            Threshold = threshold;
        }

        public SwipeGesture(double deadline) : this()
        {
            Deadline = deadline;
        }

        public SwipeGesture(Vector2 threshold, double deadline) : this()
        {
            Threshold = threshold;
            Deadline = deadline;
        }

        public SwipeGesture(SwipeDirection direction) : this()
        {
            Direction = direction;
        }

        public SwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction) : this(threshold, deadline)
        {
            Direction = direction;
        }

        public SwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction, Rectangle bounds) : this(threshold, deadline, direction)
        {
            IsRestrained = true;
            Bounds = new SharedArea(bounds.Width, bounds.Height, new Vector2(bounds.X + (bounds.Width / 2), bounds.Y + (bounds.Height / 2)), 0);
        }

        public SwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction, SharedArea? bounds) : this(threshold, deadline, direction)
        {
            IsRestrained = true;
            Bounds = bounds;
        }

        #endregion

        #region Events

        /// <inheritdoc/>
        public override event EventHandler<GestureStartedEventArgs>? GestureStarted;

        /// <inheritdoc/>
        public override event EventHandler<GestureEventArgs>? GestureActivated;

        /// <inheritdoc/>
        public override event EventHandler<GestureEventArgs>? GestureEnded;

        /// <inheritdoc/>
        public override event EventHandler<GestureEventArgs>? GestureCompleted;

        #endregion

        #region Properties

        #region Parents Implementations

        /// <inheritdoc/>
        public override bool HasStarted
        {
            get => _hasStarted;
            protected set
            {
                var previous = _hasStarted;

                _hasStarted = value;

                if (value && !previous)
                    GestureStarted?.Invoke(this, new GestureStartedEventArgs(value, _hasActivated, _hasEnded, _hasCompleted, StartPosition));
            }
        }

        /// <inheritdoc/>
        public override bool HasActivated
        {
            get => _hasActivated;
            protected set
            {
                var previous = _hasActivated;

                _hasActivated = value;

                if (value && !previous)
                    GestureActivated?.Invoke(this, new GestureEventArgs(_hasStarted, value, _hasEnded, _hasCompleted));
            }
        }

        /// <inheritdoc/>
        public override bool HasEnded
        {
            get => _hasEnded;
            protected set
            {
                var previous = _hasEnded;

                _hasEnded = value;

                if (value && !previous)
                    GestureEnded?.Invoke(this, new GestureEventArgs(_hasStarted, _hasActivated, value, _hasCompleted));
            }
        }

        /// <inheritdoc/>
        public override bool HasCompleted
        {
            get => _hasCompleted;
            protected set
            {
                var previous = _hasCompleted;

                _hasCompleted = value;

                if (value && !previous)
                    GestureCompleted?.Invoke(this, new SwipeGestureEventArgs(_hasStarted, _hasActivated, _hasEnded, value, Direction));
            }
        }

        [JsonProperty]
        public override GestureType Type => GestureType.Swipe;

        /// <inheritdoc/>
        [JsonProperty]
        public override Vector2 Threshold { get; set; }

        /// <inheritdoc/>
        [JsonProperty]
        public override double Deadline { get; set; }

        #endregion

        /// <summary>
        ///   The direction of the swipe.
        /// </summary>
        [JsonProperty]
        public SwipeDirection Direction { get; set; }

        /// <summary>
        ///   Whether or not the gesture has been invalidated at some point through the process.
        /// </summary>
        public bool IsInvalidState { get; set; }

        #endregion

        #region Methods

        protected virtual void CompleteGesture()
        {
            HasCompleted = true;
        }

        #endregion

        #region Events handlers

        /// <inheritdoc/>
        protected override void OnGestureStart(GestureStartedEventArgs e)
        {
            base.OnGestureStart(e);

            TimeStarted = DateTime.Now;
        }

        /// <inheritdoc/>
        protected override void OnGestureEnd(GestureEventArgs e)
        {
            // reset the gesture
            base.OnGestureEnd(e);
            StartPosition = Vector2.Zero;
            _delta = Vector2.Zero;
        }

        /// <inheritdoc/>
        public override void OnInput(TouchPoint[] points)
        {
            if (points.Length > 0)
            {
                var point = points[0];

                if (point != null)
                {
                    // TODO: Swipes are stealing each others turn, an up swipe could mistakenly be started by a down swipe.
                    if (!HasStarted)
                    {
                        // 1. Check if the point is inside the bounds
                        if (IsRestrained && _bounds != null && !_bounds.IsZero() && !point.IsInside(_bounds))
                            return;

                        StartPosition = point.Position;
                        IsInvalidState = false;
                        HasStarted = true;
                    }
                    else
                    {
                        // 2. Check if the deadline has been reached after the gesture has started
                        if (Deadline != 0 && (DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
                            IsInvalidState = true;

                        // 3. Check if the point is still inside the bounds
                        if (IsRestrained && _bounds != null && !_bounds.IsZero() && !point.IsInside(_bounds))
                            IsInvalidState = true;

                        // 4. End it early if the gesture is invalid
                        if (IsInvalidState)
                        {
                            HasEnded = true;
                            return;
                        }

                        // 5. Calculate the delta
                        _delta = point.Position - StartPosition;
                    }
                }
                else
                {
                    // 6. On release, possibly complete the gesture
                    if (HasStarted)
                    {
                        OnDelta();

                        // Completed or not, the gesture has ended
                        HasEnded = true;
                    }
                }
            }
        }

        /// <summary>
        ///   Called when the delta is calculated.
        /// </summary>
        protected virtual void OnDelta()
        {
            HasActivated = true;

            switch (Direction)
            {
                case SwipeDirection.Up:
                    if (_delta.Y <= -Threshold.Y)
                        CompleteGesture();
                    break;
                case SwipeDirection.Down:
                    if (_delta.Y >= Threshold.Y)
                        CompleteGesture();
                    break;
                case SwipeDirection.Left:
                    if (_delta.X <= -Threshold.X)
                        CompleteGesture();
                    break;
                case SwipeDirection.Right:
                    if (_delta.X >= Threshold.X)
                        CompleteGesture();
                    break;
                case SwipeDirection.UpLeft:
                    if (_delta.Y <= -Threshold.Y && _delta.X <= -Threshold.X)
                        CompleteGesture();
                    break;
                case SwipeDirection.UpRight:
                    if (_delta.Y <= -Threshold.Y && _delta.X >= Threshold.X)
                        CompleteGesture();
                    break;
                case SwipeDirection.DownLeft:
                    if (_delta.Y >= Threshold.Y && _delta.X <= -Threshold.X)
                        CompleteGesture();
                    break;
                case SwipeDirection.DownRight:
                    if (_delta.Y >= Threshold.Y && _delta.X >= Threshold.X)
                        CompleteGesture();
                    break;
            }
        }

        #endregion
    }
}