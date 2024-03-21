using System;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Input;
using Newtonsoft.Json;
using TouchGestures.Lib;
using TouchGestures.Lib.Entities.Gestures.Bases;
using System.Linq;
using System.Drawing;
using TouchGestures.Lib.Extensions;
using System.Collections.Generic;
using TouchGestures.Lib.Interfaces;
using Microsoft;
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
    public class TapGesture : TimeBasedGesture, IAbsolutePositionable
    {
        #region Fields

        protected bool _hasStarted = false;
        protected bool _hasEnded = false;
        protected bool _hasCompleted = false;

        protected int _requiredTouchesCount = 1;

        protected List<TouchPoint> _currentPoints = null!;
        protected byte[] _activatingPoints = null!;
        protected bool[] _releasedPoints = null!;
        protected int _previousReleasedCount = 0;
        protected int _releasedCount = 0;

        #endregion

        #region Constructors

        public TapGesture()
        {
            GestureStarted += (_, args) => OnGestureStart(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);

            Deadline = 1000;

            RequiredTouchesCount = 1;
        }

        public TapGesture(Rectangle bounds) : this()
        {
            Bounds = new SharedArea(bounds.Width, bounds.Height, new Vector2(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2), 0);
        }

        public TapGesture(SharedArea? bounds) : this()
        {
            Bounds = bounds;
        }

        public TapGesture(double deadline) : this()
        {
            Deadline = deadline;
        }

        public TapGesture(Rectangle bounds, double deadline) : this(bounds)
        {
            Deadline = deadline;
        }

        public TapGesture(SharedArea? bounds, double deadline) : this()
        {
            Bounds = bounds;
            Deadline = deadline;
        }

        public TapGesture(Rectangle bounds, double deadline, int requiredTouchesCount) : this(bounds, deadline)
        {
            RequiredTouchesCount = requiredTouchesCount;
        }

        public TapGesture(SharedArea? bounds, double deadline, int requiredTouchesCount) : this(bounds, deadline)
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

        /// <inheritdoc/>
        [JsonProperty]
        public override double Deadline { get; set; }

        #endregion

        /// <summary>
        ///   Indicates whether the gesture was invalidated after any checks. <br/>
        /// </summary>
        public bool IsInvalidState { get; protected set; }

        /// <summary>
        ///   The number of touches required to trigger the gesture. <br/>
        ///   Defaults to 1.
        /// </summary>
        [JsonProperty]
        public virtual int RequiredTouchesCount
        {
            get => _requiredTouchesCount;
            set
            {
                if (value < 1)
                    Log.Write("Touch Gestures", "The number of required touches cannot be less than 1, setting to 1.", LogLevel.Warning);

                _requiredTouchesCount = value;

                _currentPoints = new List<TouchPoint>(value);
                _activatingPoints = new byte[value];
                _releasedPoints = new bool[value];

                //_currentTouches = new List<int>(value);
            }
        }

        /// <inheritdoc/>
        [JsonProperty]
        public SharedArea? Bounds
        {
            get => _bounds?.Divide(LinesPerMM);
            set => _bounds = value?.Multiply(LinesPerMM);
        }

        /// <summary>
        ///   The position where the gesture started.
        /// </summary>
        public Vector2 StartPosition { get; protected set; }

        #endregion

        #region Methods

        protected virtual void CompleteGesture()
        {
            HasCompleted = true;
            HasEnded = true;
        }

        #endregion

        #region Event Handlers

        /// <inheritdoc/>
        protected override void OnGestureStart(GestureStartedEventArgs e)
        {
            HasEnded = false;
            HasCompleted = false;
        }

        /// <inheritdoc/>
        protected override void OnGestureEnd(GestureEventArgs e)
        {
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
            // a tap is only triggered on release
            if (points.Length > 0)
            {
                // 1. Check the currently active points
                bool res = CheckActivePoints(points, out int currentIndex);

                // Gesture has already been invalidated
                if (res == false)
                    return;

                // 2. Has the gesture started?
                if (HasStarted == false)
                {
                    // 2.1 Check if the required touches are active
                    if (currentIndex == _requiredTouchesCount)
                    {
                        // Check if the gesture is relative, in which case, the points must be inside the bounds
                        if (IsRestrained == false && _bounds != null && _bounds.IsZero() == false)
                            foreach (var point in _currentPoints)
                                if (!point.IsInside(_bounds))
                                    return;

                        TimeStarted = DateTime.Now;
                        IsInvalidState = false;

                        // Set the activating points
                        _activatingPoints = _currentPoints.Select(p => p.TouchID).ToArray();

                        // Set the gesture as started
                        HasStarted = true;
                    }
                }
                else
                {
                    // 2.2 Proceed with further handling of the input
                    OnInputCore();
                }
            }
        }

        /// <summary>
        ///   Checks the currently active points and sets the current points array.
        /// </summary>
        /// <param name="points">The points to check</param>
        /// <param name="currentIndex">The current index of the points array</param>
        /// <returns>True if the points are valid, false otherwise</returns>
        protected bool CheckActivePoints(TouchPoint[] points, out int currentIndex)
        {
            currentIndex = 0;

            _currentPoints.Clear();

            for (int i = 0; i < points.Length; i++)
            {
                TouchPoint point = points[i];

                // point is active
                if (point != null)
                {
                    // check if currentIndex is less than the required touches count
                    if (currentIndex < _requiredTouchesCount)
                    {
                        // add the point to the current points array
                        _currentPoints.Add(point);
                        currentIndex++;
                    }
                    else
                    {
                        // if currentIndex is greater than the required touches count, then the gesture is invalid
                        IsInvalidState = true;
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///   Handles the core of the input for the gesture once it has started.
        /// </summary>
        protected virtual void OnInputCore()
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
            if (RequiredTouchesCount == 1)
                Log.Write("Touch Gestures", $"Deadline: {Deadline}, Time: {(DateTime.Now - TimeStarted).TotalMilliseconds}");

            // 4.1 Check if the deadline has been reached
            if ((DateTime.Now - TimeStarted).TotalMilliseconds > Deadline)
                HasEnded = true;
            else
            {
                // 4.2 Check if all points have been released
                if (_releasedPoints.All(released => released))
                {
                    if (!IsInvalidState)
                        CompleteGesture();
                    else
                    {
                        // Wait for all touches to be released, or else, it will just start again on the next input and complete on the next release
                        if (_currentPoints.Count == 0)
                            HasEnded = true;
                    }
                }
            }
        }

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

                byte id = enumerator.Current;
                int indexOf = _currentPoints.FindIndex(p => p.TouchID == id);

                // 3.1. Check if the point is still active
                if (indexOf != -1)
                {
                    var point = _currentPoints[indexOf];

                    // 3.1.1. If Relative mode, check if the point is inside the bounds
                    if (IsRestrained == false)
                        HandleRelativeMode(point);

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
        ///   Handles the relative mode of the gesture.
        /// </summary>
        /// <remarks>
        ///   The point must be non-null & outside the bounds for the gesture to be invalid
        /// </remarks>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is valid, false otherwise</returns>
        protected bool HandleRelativeMode(TouchPoint point)
        {
            if (_bounds != SharedArea.Zero && point != null && _bounds != null && !point.IsInside(_bounds))
            {
                IsInvalidState = true;
                return false;
            }

            return true;
        }

        #endregion
    }
}