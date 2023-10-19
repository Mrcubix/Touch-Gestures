using System;
using System.Numerics;
using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Input;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a swipe gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    public class SwipeGesture : IMixedBasedGesture
    {
        #region Fields

        private bool _hasStarted = false;
        private bool _hasEnded = false;
        private bool _hasCompleted = false;

        #endregion

        #region Constructors

        public SwipeGesture()
        {
            GestureStarted += (_, args) => OnGestureStart(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);
        }

        public SwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction) : this()
        {
            Threshold = threshold;
            Deadline = deadline;
            Direction = direction;
        }

        public SwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction, IBinding binding) : this(threshold, deadline, direction)
        {
            Binding = binding;
        }

        #endregion

        #region Events

        /// <inheritdoc/>
        public event EventHandler<GestureStartedEventArgs> GestureStarted = null!;

        /// <inheritdoc/>
        public event EventHandler<GestureEventArgs> GestureEnded = null!;

        /// <inheritdoc/>
        public event EventHandler<GestureEventArgs> GestureCompleted = null!;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual bool HasStarted
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
        public virtual bool HasEnded
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
        public virtual bool HasCompleted
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
        public virtual IBinding? Binding { get; set; }

        /// <inheritdoc/>
        public Vector2 StartPosition { get; protected set; }

        /// <inheritdoc/>
        public Vector2 Threshold { get; protected set; }

        /// <inheritdoc/>
        public DateTime TimeStarted { get; protected set; }

        /// <inheritdoc/>
        public double Deadline { get; protected set; } = 1000;

        /// <summary>
        ///   The direction of the swipe.
        /// </summary>
        public SwipeDirection Direction { get; set; }

        #endregion

        #region Methods

        protected void CompleteGesture()
        {
            HasCompleted = true;
            HasEnded = true;
            
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

        #region Events handlers

        /// <inheritdoc/>
        protected virtual void OnGestureStart(GestureEventArgs e)
        {
            HasEnded = false;
            HasCompleted = false;

            TimeStarted = DateTime.Now;
        }

        /// <inheritdoc/>
        protected virtual void OnGestureEnd(GestureEventArgs e)
        {
            // reset the gesture
            HasStarted = false;
        }

        /// <inheritdoc/>
        protected virtual void OnGestureComplete(GestureEventArgs e)
        {
            HasStarted = false;
            StartPosition = Vector2.Zero;
        }

        /// <inheritdoc/>
        public virtual void OnInput(TouchPoint[] points)
        {
            if (points.Length > 0)
            {
                var point = points[0];

                if (point != null)
                {
                    if (!HasStarted)
                    {
                        StartPosition = point.Position;
                        HasStarted = true;
                    }
                    else
                    {
                        if ((DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
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