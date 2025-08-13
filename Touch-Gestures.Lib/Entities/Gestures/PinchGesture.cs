using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    ///   Represents a pinch gesture. <br/>
    ///   A pinch gesture is a two-finger gesture where the fingers move towards or away from each other.
    ///   A rotation can also be detected if the fingers move in a circular motion.
    ///   The gesture can be inner or outer (Simple Pinch), clockwise or counter-clockwise (Rotation). 
    /// </summary>
    public class PinchGesture : Gesture
    {
        private const double RadToDeg = 180 / Math.PI;
        private const int REQUIRED_TOUCHES_COUNT = 2;

        #region Fields

        protected bool _hasStarted = false;
        protected bool _hasActivated = false;
        protected bool _hasEnded = false;
        protected bool _hasCompleted = false;

        protected List<TouchPoint> _currentPoints = null!;
        protected TouchPoint[] _activatingPoints = null!;
        protected bool[] _releasedPoints = null!;
        protected int _previousReleasedCount = 0;
        protected int _releasedCount = 0;

        private double _previousDistance;
        private double _previousAngle;
        private double _deltaDistance;
        private double _deltaAngle;

        #endregion

        #region Constructors

        public PinchGesture()
        {
            GestureStarted += (_, args) => OnGestureStart(args);
            GestureActivated += (_, args) => OnGestureActive(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);

            _currentPoints = new List<TouchPoint>(REQUIRED_TOUCHES_COUNT);
            _activatingPoints = new TouchPoint[REQUIRED_TOUCHES_COUNT];
            _releasedPoints = new bool[REQUIRED_TOUCHES_COUNT];

            IsRestrained = true;
        }

        public PinchGesture(SharedArea? bounds) : this()
        {
            Bounds = bounds;
        }

        public PinchGesture(Rectangle bounds) : this()
        {
            Bounds = new SharedArea(bounds.Width, bounds.Height, new Vector2(bounds.X, bounds.Y), 0);
        }

        public PinchGesture(double distanceThreshold, double angleThreshold, SharedArea? bounds) : this(bounds)
        {
            IsInner = false;
            IsClockwise = false;

            DistanceThreshold = distanceThreshold;
            AngleThreshold = angleThreshold;
        }

        public PinchGesture(double distanceThreshold, double angleThreshold, Rectangle bounds) : this(bounds)
        {
            IsInner = false;
            IsClockwise = false;

            DistanceThreshold = distanceThreshold;
            AngleThreshold = angleThreshold;
        }

        public PinchGesture(double distanceThreshold, double angleThreshold, bool isInner, bool isClockwise, SharedArea? bounds)
            : this(bounds)
        {
            IsInner = isInner;
            IsClockwise = isClockwise;

            DistanceThreshold = distanceThreshold;
            AngleThreshold = angleThreshold;
        }

        public PinchGesture(double distanceThreshold, double angleThreshold, bool isInner, bool isClockwise, Rectangle bounds)
            : this(bounds)
        {
            IsInner = isInner;
            IsClockwise = isClockwise;

            DistanceThreshold = distanceThreshold;
            AngleThreshold = angleThreshold;
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
                    if (_currentPoints.Count == REQUIRED_TOUCHES_COUNT)
                        GestureStarted?.Invoke(this, new GestureStartedEventArgs(value, _hasActivated, _hasEnded, _hasCompleted, _currentPoints[0].Position));
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

                if (value && previous == false)
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

                if (value && previous == false)
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

                if (value && previous == false)
                    GestureCompleted?.Invoke(this, new GestureEventArgs(_hasStarted, _hasActivated, _hasEnded, value));
            }
        }

        [JsonProperty]
        public override GestureType Type => DistanceThreshold > 0 ? GestureType.Pinch : GestureType.Rotate;

        #endregion

        #region Dependencies

        /// <summary>
        ///   Specifies whether the pinch is inner or outer.
        /// </summary>
        [JsonProperty]
        public bool IsInner { get; set; }

        /// <summary>
        ///   Specifies whether the pinch is clockwise or counter-clockwise.
        /// </summary>
        [JsonProperty]
        public bool IsClockwise { get; set; }

        /// <summary>
        ///   The distance threshold for the gesture to be completed.
        /// </summary>
        [JsonProperty]
        public double DistanceThreshold { get; set; }

        /// <summary>
        ///   The angle threshold for the gesture to be completed.
        /// </summary>
        [JsonProperty]
        public double AngleThreshold { get; set; }

        #endregion

        /// <summary>
        ///   Indicates whether the gesture was invalidated after any checks. <br/>
        /// </summary>
        public bool IsInvalidState { get; protected set; }

        /// <summary>
        ///   Origin of the gesture.
        /// </summary>
        public Vector2 Origin { get; protected set; }

        /// <summary>
        ///   The current angle of the gesture.
        /// </summary>
        public double CurrentAngle { get; protected set; }

        #endregion

        #region Methods

        protected virtual void CompleteGesture()
        {
            HasCompleted = true;
        }

        #region Gesture Specific Methods

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
                    if (currentIndex < REQUIRED_TOUCHES_COUNT)
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
        ///   Checks the delta values of the gesture.
        /// </summary>
        protected virtual /*(double, double, double, double)*/ void CheckDelta()
        {
            if (_currentPoints.Count != REQUIRED_TOUCHES_COUNT)
                return;
            //return (0, 0, 0, 0);

            var first = _currentPoints[0];
            var second = _currentPoints[1];

            var distance = GetDistance(first.Position, second.Position);
            //var distanceDiff = distance - InitialDistance;
            _deltaDistance += distance - _previousDistance;

            CurrentAngle = GetAngleDegreeFromPoints(first.Position, second.Position);
            //var angleDiff = CurrentAngle - InitialAngle;
            _deltaAngle += CurrentAngle - _previousAngle;

            _previousDistance = distance;
            _previousAngle = CurrentAngle;

            //return (distanceDiff, angleDiff, distance, CurrentAngle);
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

                TouchPoint ap = enumerator.Current;
                int indexOf = _currentPoints.FindIndex(p => p.TouchID == ap.TouchID);

                // 3.1. Check if the point is still active
                if (indexOf != -1)
                {
                    var point = _currentPoints[indexOf];

                    // 3.1.1. If Relative mode, check if the point is inside the bounds
                    if (IsRestrained == false)
                        HandleRelativeMode(point);

                    _releasedPoints[currentIndex] = false;

                    // 3.1.3. Remove the point from the current points array
                    _currentPoints.RemoveAt(indexOf);
                }
                else
                {
                    // The point has been released, this invalidates the gesture
                    _releasedPoints[currentIndex] = true;
                    _releasedCount++;

                    IsInvalidState = true;
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
            if (point != null && _bounds != null && !_bounds.IsZero() && !point.IsInside(_bounds))
            {
                IsInvalidState = true;
                return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region Event Handlers

        /// <inheritdoc/>
        protected override void OnGestureComplete(GestureEventArgs e)
        {
        }

        /// <inheritdoc/>
        public override void OnInput(TouchPoint[] points)
        {
            if (points.Length == 0)
                return;

            if (CheckActivePoints(points, out int currentIndex) == false)
                return;

            if (HasStarted == false)
            {
                if (currentIndex == REQUIRED_TOUCHES_COUNT)
                {
                    if (IsRestrained == false && _bounds != null && _bounds.IsZero() == false)
                        foreach (var point in _currentPoints)
                            if (!point.IsInside(_bounds))
                                return;

                    IsInvalidState = false;

                    _activatingPoints = _currentPoints.ToArray();
                    Origin = _currentPoints[0].Position;

                    //InitialDistance = GetDistance(_currentPoints[0].Position, _currentPoints[1].Position);
                    //InitialAngle = PreviousAngle = GetAngleDegreeFromPoints(_currentPoints[0].Position, _currentPoints[1].Position);

                    _previousDistance = GetDistance(_currentPoints[0].Position, _currentPoints[1].Position);
                    _previousAngle = GetAngleDegreeFromPoints(_currentPoints[0].Position, _currentPoints[1].Position);

                    _deltaDistance = 0;
                    _deltaAngle = 0;

                    HasStarted = true;
                }
            }
            else
                OnInputCore(points);
        }

        /// <summary>
        ///   The core of the input handling.
        /// </summary>
        /// <param name="points">The points to handle</param>
        protected virtual void OnInputCore(TouchPoint[] points)
        {
            //(double distanceDiff, double angleDiff, double distance, double angle) = CheckDelta();
            CheckDelta();

            CheckReleasedPoints();

            // If there are still points in the current points, 
            // that means other touch points were pressed after the gesture started, 
            // the state is invalid.
            if (_currentPoints.Count > 0)
                IsInvalidState = true;

            // An activating point was released but then it was pressed again
            if (_previousReleasedCount > _releasedCount)
                IsInvalidState = true;

            // Checking that the gesture isn't in an invalid state to complete it at least once
            if (!IsInvalidState)
            {
                bool conditionsMet;

                // Check if pinch is not a rotation
                if (DistanceThreshold > 0)
                    conditionsMet = IsInner ? _deltaDistance <= (-DistanceThreshold) : _deltaDistance >= DistanceThreshold;
                else
                    conditionsMet = IsClockwise ? _deltaAngle >= AngleThreshold : _deltaAngle <= (-AngleThreshold);

                if (conditionsMet)
                {
                    CompleteGesture();

                    // Reset the delta values
                    //_deltaDistance += IsInner ? DistanceThreshold : -DistanceThreshold;
                    //_deltaAngle += IsClockwise ? -AngleThreshold : AngleThreshold;
                    _deltaDistance = 0;
                    _deltaAngle = 0;
                }
            }

            // Only end the gesture when all points are released
            if (_releasedPoints.All(released => released))
                if (_currentPoints.Count == 0)
                    HasEnded = true;
        }

        #endregion

        #region static Methods

        /// <summary>
        ///   Gets the distance between two points.
        /// </summary>
        /// <param name="a">The origin point</param>
        /// <param name="b">The target point</param>
        /// <returns>The distance between the two points</returns>
        private static double GetDistance(Vector2 a, Vector2 b)
        {
            return (a - b).Length();
        }

        /// <summary>
        ///   Gets the angle between two points in degrees.
        /// </summary>
        /// <param name="a">The first point</param>
        /// <param name="b">The second point</param>
        /// <returns>The angle between the two points in degrees</returns>
        /// <remarks>
        ///   Stolen from Avalonia's PinchGestureRecognizer
        /// </remarks>
        private static double GetAngleDegreeFromPoints(Vector2 a, Vector2 b)
        {
            // https://stackoverflow.com/a/15994225/20894223

            var deltaX = a.X - b.X;
            var deltaY = -(a.Y - b.Y);                           // I reverse the sign, because on the screen the Y axes
                                                                 // are reversed with respect to the Cartesian plane.
            var rad = Math.Atan2(deltaX, deltaY);                // radians from -π to +π
            var degree = (rad * RadToDeg) + 180;                 // Atan2 returns a radian value between -π to +π, in degrees -180 to +180.
                                                                 // To get the angle between 0 and 360 degrees you need to add 180 degrees.
            return degree;
        }

        #endregion
    }
}