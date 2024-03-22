using System;
using System.Numerics;
using TouchGestures.Lib.Input;
using Newtonsoft.Json;
using TouchGestures.Lib;
using System.Linq;
using System.Drawing;
using TouchGestures.Lib.Entities;

namespace TouchGestures.Entities.Gestures
{
    /// <summary>
    ///   Represent a x-finger tap gesture.
    /// </summary>
    /// <remarks>
    ///   A tap gesture is triggered when a <see cref="RequiredTouchesCount"/> number of fingers are pressed and released within a specified deadline.
    /// </remarks>
    [JsonObject(MemberSerialization.OptIn)]
    public class HoldGesture : TapGesture
    {
        private bool _deadlineStarted = false;

        #region Constructors

        public HoldGesture() : base(1000)
        {
            GestureStarted += (_, args) => OnGestureStart(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);

            RequiredTouchesCount = 1;
        }

        public HoldGesture(Rectangle bounds) : this()
        {
            Bounds = new SharedArea(bounds.Width, bounds.Height, new Vector2(bounds.X + (bounds.Width / 2), bounds.Y + (bounds.Height / 2)), 0);
        }

        public HoldGesture(SharedArea? bounds) : this()
        {
            Bounds = bounds;
        }

        public HoldGesture(double deadline) : this()
        {
            Deadline = deadline;
        }

        public HoldGesture(Rectangle bounds, double deadline) : this(bounds)
        {
            Deadline = deadline;
        }

        public HoldGesture(SharedArea? bounds, double deadline) : this()
        {
            Bounds = bounds;
            Deadline = deadline;
        }

        public HoldGesture(Rectangle bounds, double deadline, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
        }

        public HoldGesture(SharedArea? bounds, double deadline, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
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

        #region Parents Implementation

        /// <inheritdoc/>
        public override bool HasStarted
        {
            get => _hasStarted;
            protected set
            {
                var previous = _hasStarted;

                _hasStarted = value;

                if (value && previous == false)
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

                if (value && previous == false)
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

                if (value && previous == false)
                    GestureCompleted?.Invoke(this, new GestureEventArgs(_hasStarted, _hasEnded, value));
            }
        }

        /// <inheritdoc/>
        [JsonProperty]
        public override bool IsRestrained { get; }

        /// <summary>
        ///   The deadline within which the gesture must be completed from the moment any activating points are released.
        /// </summary>
        [JsonProperty]
        public override double Deadline { get; set; }

        /// <summary>
        ///   Time at which the first touch point was released.
        /// </summary>
        public DateTime TimeFirstReleased { get; private set; }

        /// <summary>
        ///   The amount of time the user must keep the touch points pressed to complete the gesture.
        /// </summary>
        [JsonProperty]
        public double Threshold { get; set; }

        #endregion

        #endregion

        #region Event Handlers

        /// <summary>
        ///   Handles the core of the input for the gesture once it has started.
        /// </summary>
        protected override void OnInputCore()
        {
            // 3. Start by iterating over the activating points & check if they are still active
            CheckReleasedPoints();

            // If there are still points in the current points, 
            // that means other touch points were pressed after the gesture started, 
            // the state is invalid.
            if (_currentPoints.Count > 0)
                IsInvalidState = true;

            // An activating point was released but then it was pressed again
            if (_previousReleasedCount > _releasedCount)
                IsInvalidState = true;

            // 4. Deadline & Release check

            // 4.1 Check if the user has released any touch points, at which point they will have to release all of them within the deadline
            if (_releasedCount > 0 && _deadlineStarted == false)
            {
                _deadlineStarted = true;
                TimeFirstReleased = DateTime.Now;
            }

            // 4.2 Check if the user has not released the touch points within the deadline from the first release
            if (_deadlineStarted && (DateTime.Now - TimeFirstReleased).TotalMilliseconds >= Deadline)
                IsInvalidState = true;

            // 4.3 Check if all points have been released
            if (_releasedPoints.All(released => released))
            {
                // 4.3.1 Check if the user has held the touch point for the required amount of time
                if (!IsInvalidState && (DateTime.Now - TimeStarted).TotalMilliseconds >= Threshold)
                    CompleteGesture();
                else
                {
                    // 4.3.2 Wait for all touches to be released, or else, it will just start again on the next input and complete on the next release
                    if (_currentPoints.Count == 0)
                        HasEnded = true;
                }
            }
        }

        #endregion
    }
}