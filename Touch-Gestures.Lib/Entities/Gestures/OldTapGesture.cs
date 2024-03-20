using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Input;
using Newtonsoft.Json;
using TouchGestures.Lib;
using TouchGestures.Lib.Entities.Gestures.Bases;

namespace TouchGestures.Entities.Gestures
{
    /// <summary>
    ///   Represent a 1-finger tap gesture.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class OldTapGesture : MixedBasedGesture
    {
        #region Fields

        private bool _hasStarted = false;
        private bool _hasEnded = false;
        private bool _hasCompleted = false;

        private int _requiredTouchesCount = 1;

        private int[] _invokingTouchesIndices = null!;
        private bool[] _releasedTouches = null!;
        private TouchPoint[] _previousPoints = null!;

        #endregion

        #region Constructors

        public OldTapGesture() : base(1000)
        {
            GestureStarted += (_, args) => OnGestureStart(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);

            RequiredTouchesCount = 1;
        }

        public OldTapGesture(Vector2 threshold) : this()
        {
            Threshold = threshold;
        }

        public OldTapGesture(double deadline) : this()
        {
            Deadline = deadline;
        }

        public OldTapGesture(Vector2 threshold, double deadline) : this(threshold)
        {
            Deadline = deadline;
        }

        public OldTapGesture(Vector2 threshold, double deadline, int requiredTouchesCount) : this(threshold, deadline)
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
                    GestureCompleted?.Invoke(this, new GestureEventArgs(_hasStarted, _hasEnded, value));
            }
        }

        /// <inheritdoc/>
        public override bool IsRestrained { get; }

        /// <inheritdoc/>
        [JsonProperty]
        public override Vector2 Threshold { get; set; }

        /// <inheritdoc/>
        [JsonProperty]
        public override double Deadline { get; set; }

        /// <summary>
        ///   Indicates whether the gesture was invalidated after any checks. <br/>
        /// </summary>
        public bool IsInvalidState { get; private set; }

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
                _invokingTouchesIndices = new int[value];
                _releasedTouches = new bool[value];

                Array.Fill(_invokingTouchesIndices, -1);

                //_currentTouches = new List<int>(value);
            }
        }

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
                if (_previousPoints == null || _previousPoints.Length != points.Length)
                    _previousPoints = new TouchPoint[points.Length];

                List<int> currentPointsIndices = new(points.Length);

                for (int i = 0; i < points.Length; i++)
                    if (points[i] != null)
                        currentPointsIndices.Add(i);

                // Create List of TouchPoints from the indices
                List<TouchPoint> currentPoints = new(currentPointsIndices.Count);
                int currentPointsCount = currentPointsIndices.Count;

                // Fill the list
                foreach (int index in currentPointsIndices)
                    currentPoints.Add(points[index]);

                if (!HasStarted)
                {
                    // We start the gesture when the required touches are present, no more, no less
                    if (currentPointsCount == RequiredTouchesCount)
                    {
                        HasStarted = true;

                        // get the position of the first non-null point
                        StartPosition = points[currentPointsIndices[0]].Position;

                        TimeStarted = DateTime.Now;

                        IsInvalidState = false;

                        // no index or ids are stored within the points
                        _invokingTouchesIndices = currentPointsIndices.ToArray();
                    }
                }
                else
                {
                    // check if the current points are the same as the invoking points
                    if (currentPointsCount == 0)
                        Array.Fill(_releasedTouches, true);

                    var enumerator = _invokingTouchesIndices.AsEnumerable().GetEnumerator();
                    var currentIndex = -1;

                    // we check if the invoking touches have been released or not
                    while (!IsInvalidState && enumerator.MoveNext())
                    {
                        currentIndex++;

                        int index = enumerator.Current;
                        int indexOf = currentPointsIndices.IndexOf(index);

                        // if an invoking touch is not in the current points, 
                        if (indexOf != -1)
                        {
                            // check if the touch is outside the threshold
                            var point = currentPoints[indexOf];

                            // check if point is active & within the set threshold
                            if (point != null)
                                if (Math.Abs(point.Position.X - StartPosition.X) > Threshold.X || Math.Abs(point.Position.Y - StartPosition.Y) > Threshold.Y)
                                    IsInvalidState = true;

                            _releasedTouches[currentIndex] = false;

                            // remove the point from the list
                            currentPointsIndices.RemoveAt(indexOf);
                        }
                        else
                        {
                            _releasedTouches[currentIndex] = true;
                        }
                    }

                    // if there are still points in the current points, the state is invalid
                    if (currentPointsIndices.Count > 0)
                        IsInvalidState = true;

                    /* 
                      Used to check if the any current points were not in the invoking points, 
                      but that was slower than now just removing invoking points upon earlier discovery
                    */
                    /*foreach (int index in currentPointsIndices)
                        if (!_invokingTouchesIndices.Contains(index))
                            IsInvalidState = true;*/


                    // check if the deadline has been reached
                    if ((DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
                        HasEnded = true;
                    else
                    {
                        // check if all touches have been released
                        if (_releasedTouches.All(released => released))
                        {
                            if (!IsInvalidState)
                                CompleteGesture();
                            else
                            {
                                // Wait for all touches to be released, or else, it will just start again on the next input and complete on the next release
                                if (currentPointsCount == 0)
                                    HasEnded = true;
                            }
                        }
                    }
                }
                _previousPoints = points;
            }
        }

        #endregion
    }
}