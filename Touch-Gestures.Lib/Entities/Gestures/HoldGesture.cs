using System;
using System.Numerics;
using TouchGestures.Lib.Input;
using Newtonsoft.Json;
using System.Linq;
using System.Drawing;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a x-finger tap gesture.
    /// </summary>
    /// <remarks>
    ///   A tap gesture is triggered when a <see cref="RequiredTouchesCount"/> number of fingers are pressed and released within a specified deadline.
    /// </remarks>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class HoldGesture : TapGesture
    {
        //private bool _deadlineStarted = false;

        #region Constructors

        public HoldGesture() : base(1000)
        {
            GestureStarted += (_, args) => OnGestureStart(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);

            UseThreshold = true;

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

        public HoldGesture(Rectangle bounds, Vector2 threshold, double deadline, int requiredTouchesCount) : this(bounds, deadline, requiredTouchesCount)
        {
            Threshold = threshold;
        }

        public HoldGesture(SharedArea? bounds, Vector2 threshold, double deadline, int requiredTouchesCount) : this(bounds, deadline, requiredTouchesCount)
        {
            Threshold = threshold;
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

        /// <summary>
        ///   The amount of time the user has to keep the touch points pressed to trigger the hold.
        /// </summary>
        [JsonProperty]
        public override double Deadline { get; set; }

        /// <inheritdoc/>
        /// <remarks>
        ///   Used for HoldGesture to check if the touch point is within the threshold.
        /// </remarks>
        [JsonProperty]
        public override Vector2 Threshold { get; set; }

        #endregion

        public bool IsPressing { get; private set; }

        #endregion

        #region Methods

        protected virtual void Press() 
        {
            IsPressing = true;
        }

        protected virtual void Release() 
        {
            IsPressing = false;

            CompleteGesture();
        }

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

            // 4.1 Check if the user has held the touch point for the required amount of time
            if (!IsInvalidState && !IsPressing && (DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
                Press();

            // 4.2 // 4.1.2 Wait for all touches to be released, or else, it will just start again on the next input and complete on the next release
            if (_releasedPoints.All(released => released))
                if (_currentPoints.Count == 0)
                    Release();
        }

        /// <inheritdoc/>
        protected override void HandleThreshold(TouchPoint point, TouchPoint activePoint)
        {
            // 2. Check if the touch point is within the threshold
            if (Vector2.Distance(point.Position, activePoint.Position) > Threshold.X)
                IsInvalidState = true;
        }

        #endregion
    }
}