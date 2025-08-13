using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Extensions;
using TouchGestures.Lib.Input;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a swipe gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SwipeGesture : MixedBasedGesture, ITouchesCountDependant
    {
        #region Fields

        protected bool _hasStarted = false;
        protected bool _hasActivated = false;
        protected bool _hasEnded = false;
        protected bool _hasCompleted = false;

        protected int _requiredTouchesCount;

        protected List<TouchPoint> _currentPoints = null!;
        protected TouchPoint[] _activatingPoints = null!;
        protected bool[] _releasedPoints = null!;
        protected int _previousReleasedCount = 0;
        protected int _releasedCount = 0;


        protected Vector2 _delta = Vector2.Zero;
        protected Vector2 _lastPosition = Vector2.Zero;
        protected bool _isInvalidState = false;

        #endregion

        #region Constructors

        public SwipeGesture() : base(1000)
        {
            IsRestrained = true;

            GestureStarted += (_, args) => OnGestureStart(args);
            GestureActivated += (_, args) => OnGestureActive(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);

            RequiredTouchesCount = 1;
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

        public SwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction, Rectangle bounds, int requiredTouches) : this(threshold, deadline, direction, bounds)
        {
            RequiredTouchesCount = requiredTouches;
        }

        public SwipeGesture(Vector2 threshold, double deadline, SwipeDirection direction, SharedArea? bounds, int requiredTouches) : this(threshold, deadline, direction, bounds)
        {
            RequiredTouchesCount = requiredTouches;
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
        public bool IsInvalidState
        {
            get => _isInvalidState;
            set => _isInvalidState = value;
        }

        /// <inheritdoc/>
        [JsonProperty]
        public int RequiredTouchesCount
        {
            get => _requiredTouchesCount;
            set
            {
                var finalCount = value;

                if (value < 1)
                {
                    Log.Write("Touch Gestures", "The number of required touches cannot be less than 1, setting to 1.", LogLevel.Warning);
                    finalCount = 1;
                }

                _requiredTouchesCount = finalCount;

                _currentPoints = new List<TouchPoint>(finalCount);
                _activatingPoints = new TouchPoint[finalCount];
                _releasedPoints = new bool[finalCount];
            }
        }

        #endregion

        #region Methods

        protected virtual void CompleteGesture()
        {
            HasActivated = true;
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
                // 1. Check the currently active points, gesture may have been invalidated
                _currentPoints = GestureUtilities.GetActivePoints(points, RequiredTouchesCount, ref _isInvalidState, out int currentIndex);

                if (_isInvalidState)
                    return;

                // 2. Has the gesture started ? Is the required number of touches active ?
                if (HasStarted == false)
                {
                    if (currentIndex == _requiredTouchesCount)
                    {
                        StartPosition = Vector2.Zero;

                        // Check if the gesture is relative, in which case, the points must be inside the bounds
                        if (IsRestrained && _bounds != null && !_bounds.IsZero())
                            foreach (var point in _currentPoints)
                            {
                                if (point.IsInside(_bounds))
                                    StartPosition += point.Position;
                                else
                                    return;
                            }

                        StartPosition /= RequiredTouchesCount;

                        // Set the activating points
                        _activatingPoints = _currentPoints.ToArray();

                        IsInvalidState = false;
                        HasStarted = true;
                    }
                }
                else
                {
                    // 3. Start by iterating over the activating points & check if they are still active
                    CheckReleasedPoints();

                    // 4. Check if the gesture is valid
                    IsInvalidState |= Validate() == false;

                    if (IsInvalidState == false && currentIndex == RequiredTouchesCount)
                    {
                        // 5. Calculate the average position
                        var avgPosition = Vector2.Zero;

                        foreach (var point in points)
                            if (point != null)
                                avgPosition += point.Position;

                        avgPosition /= RequiredTouchesCount;
                        _lastPosition = avgPosition;

                        // 6. Calculate the delta

                        _delta = avgPosition - StartPosition;
                    }

                    OnInputCore();
                }
            }
        }

        protected virtual void OnInputCore()
        {
            if (_releasedPoints.All(released => released) && _currentPoints.Count == 0)
            {
                if (IsInvalidState == false)
                    OnDelta();

                HasEnded = true;
            }
        }

        #region Checks

        /// <summary>
        ///   Checks the currently active points and sets the released points array.
        ///   Also removes the points from the current points array if they have been released.
        /// </summary>
        protected virtual void CheckReleasedPoints()
        {
            if (_currentPoints.Count == 0)
                Array.Fill(_releasedPoints, true);

            _releasedCount = 0;

            var enumerator = _activatingPoints.AsEnumerable().GetEnumerator();
            var currentIndex = -1;

            // We enumerate over activating points instead of current points for efficiency
            while (IsInvalidState == false && enumerator.MoveNext())
            {
                currentIndex++;

                TouchPoint ap = enumerator.Current;
                int indexOf = _currentPoints.FindIndex(p => p.TouchID == ap.TouchID);

                // 3.1. Check if the point is still active
                if (indexOf != -1)
                {
                    var point = _currentPoints[indexOf];

                    // 3.1.1. If Relative mode, check if the point is inside the bounds
                    if (IsRestrained == false)
                        if (point != null && _bounds != null && !_bounds.IsZero() && !point.IsInside(_bounds))
                            IsInvalidState = true;

                    // 3.1.2. The point has not been released, set it in the released points array
                    _releasedPoints[currentIndex] = false;

                    // 3.1.3. Remove the point from the current points array
                    _currentPoints.RemoveAt(indexOf);
                }
                else
                {
                    // 3.2. the point has been released, set it in the released points array
                    _releasedPoints[currentIndex] = true;
                    _releasedCount++;
                }
            }
        }

        /// <summary>
        ///   Checks if the gesture is still valid
        /// </summary>
        protected virtual bool Validate()
        {
            // If there are still points in the current points, 
            // that means other touch points were pressed after the gesture started, 
            // the state is invalid.
            if (_currentPoints.Count > 0)
                return false;

            // An activating point was released but then it was pressed again
            if (_previousReleasedCount > _releasedCount)
                return false;

            // Check if the deadline has been reached after the gesture has started
            if (Deadline != 0 && (DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
                return false;

            // Check if the points are still inside the bounds
            if (IsRestrained && _bounds != null && !_bounds.IsZero())
                foreach (var point in _currentPoints)
                    if (!point.IsInside(_bounds))
                        return false;

            return true;
        }

        #endregion

        /// <summary>
        ///   Called
        /// </summary>
        protected virtual void OnDelta()
        {
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