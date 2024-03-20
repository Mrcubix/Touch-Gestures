using System;
using System.Drawing;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Extensions;
using TouchGestures.Lib.Input;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Entities.Gestures
{
    /// <summary>
    ///   Represent a swipe gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SwipeGesture : MixedBasedGesture, IAbsolutePositionable
    {
        #region Fields

        private bool _hasStarted = false;
        private bool _hasEnded = false;
        private bool _hasCompleted = false;

        #endregion

        #region Constructors

        public SwipeGesture() : base(1000)
        {
            GestureStarted += (_, args) => OnGestureStart(args);
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
            Bounds = new Area(bounds.Width, bounds.Height, new Vector2(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2), 0);
        }   

        public SwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction, Area? bounds) : this(threshold, deadline, direction)
        {
            IsRestrained = true;
            Bounds = bounds;
        }

        #endregion

        #region Events

        /// <inheritdoc/>
        public override event EventHandler<GestureStartedEventArgs>? GestureStarted;

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
                    GestureStarted?.Invoke(this, new GestureStartedEventArgs(value, _hasEnded, _hasCompleted, StartPosition));
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
                    GestureEnded?.Invoke(this, new GestureEventArgs(_hasStarted, value, _hasCompleted));
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
                    GestureCompleted?.Invoke(this, new SwipeGestureEventArgs(_hasStarted, _hasEnded, value, Direction));
            }
        }

        /// <inheritdoc/>
        [JsonProperty]
        public override bool IsRestrained { get; }

        /// <inheritdoc/>
        [JsonProperty]
        public override Vector2 Threshold { get; set; }

        /// <inheritdoc/>
        [JsonProperty]
        public override double Deadline { get; set; }

        /// <inheritdoc/>
        [JsonProperty]
        public Area? Bounds { get; set; } = AreaExtensions.Zero;

        #endregion

        /// <summary>
        ///   The direction of the swipe.
        /// </summary>
        [JsonProperty]
        public SwipeDirection Direction { get; set; }

        #endregion

        #region Methods

        protected virtual void CompleteGesture()
        {
            HasCompleted = true;
            HasEnded = true;
        }

        #endregion

        #region Events handlers

        /// <inheritdoc/>
        protected override void OnGestureStart(GestureStartedEventArgs e)
        {
            HasEnded = false;
            HasCompleted = false;

            TimeStarted = DateTime.Now;
        }

        /// <inheritdoc/>
        protected override void OnGestureEnd(GestureEventArgs e)
        {
            // reset the gesture
            HasStarted = false;
        }

        /// <inheritdoc/>
        protected override void OnGestureComplete(GestureEventArgs e)
        {
            HasStarted = false;
            StartPosition = Vector2.Zero;
        }

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
                        if (IsRestrained && Bounds != null && !point.IsInside(Bounds))
                            return;

                        StartPosition = point.Position;
                        HasStarted = true;
                    }
                    else
                    {
                        if (Deadline != 0 && (DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
                        {
                            HasEnded = true;
                            return;
                        }

                        var delta = point.Position - StartPosition;

                        switch(Direction)
                        {
                            case SwipeDirection.Up:
                                if (delta.Y <= -Threshold.Y)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.Down:
                                if (delta.Y >= Threshold.Y)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.Left:
                                if (delta.X <= -Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.Right:
                                if (delta.X >= Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.UpLeft:
                                if (delta.Y <= -Threshold.Y && delta.X <= -Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.UpRight:
                                if (delta.Y <= -Threshold.Y && delta.X >= Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.DownLeft:
                                if (delta.Y >= Threshold.Y && delta.X <= -Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.DownRight:
                                if (delta.Y >= Threshold.Y && delta.X >= Threshold.X)
                                    CompleteGesture();
                                break;
                        }
                    }
                }
                else
                {
                    if (HasStarted)
                        HasEnded = true;
                }
            }
        }

        #endregion
    }
}