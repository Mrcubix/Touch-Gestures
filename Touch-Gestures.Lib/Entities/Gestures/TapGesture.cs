using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Input;
using TouchGestures.Lib.Interfaces;
using System.Threading.Tasks;

namespace TouchGestures.Lib.Entities.Gestures
{
    /// <summary>
    ///   Represent a 1-finger tap gesture.
    /// </summary>
    public class TapGesture : IMixedBasedGesture
    {
        private bool _hasStarted = false;
        private bool _hasEnded = false;
        private bool _hasCompleted = false;

        private int _requiredTouchesCount = 1;
        private int[] _invokingTouchesIndices = null!;
        private bool[] _releasedTouches = null!;
        private TouchPoint[] _invokingTouches = null!;

        private TouchPoint[] _previousPoints = null!;

        /*private List<int> _currentTouches = null!;
        private bool[] _previousActiveTouches = null!;
        private bool[] _activeTouchesOutsideThreshold = null!;
        private readonly List<int> _invokerTouches = new();*/



        #region Constructors

        public TapGesture()
        {
            GestureStarted += (_, args) => OnGestureStart(args);
            GestureEnded += (_, args) => OnGestureEnd(args);
            GestureCompleted += (_, args) => OnGestureComplete(args);
        }

        public TapGesture(Vector2 threshold) : this()
        {
            Threshold = threshold;
        }

        public TapGesture(Vector2 threshold, double deadline) : this(threshold)
        {
            Deadline = deadline;
        }

        public TapGesture(Vector2 threshold, double deadline, IBinding binding) : this(threshold, deadline)
        {
            Binding = binding;
        }

        public TapGesture(Vector2 threshold, IBinding binding) : this(threshold)
        {
            Binding = binding;
        }

        public TapGesture(Vector2 threshold, IBinding binding, int requiredTouchesCount) : this(threshold, binding)
        {
            RequiredTouchesCount = requiredTouchesCount;
        }

        public TapGesture(Vector2 threshold, double deadline, IBinding binding, int requiredTouchesCount) : this(threshold, deadline, binding)
        {
            RequiredTouchesCount = requiredTouchesCount;
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
                    GestureCompleted?.Invoke(this, new GestureEventArgs(_hasStarted, _hasEnded, value));
            }
        }

        /// <inheritdoc/>
        public virtual Vector2 StartPosition { get; protected set; }

        /// <inheritdoc/>
        public virtual Vector2 Threshold { get; protected set; }

        /// <inheritdoc/>
        public virtual DateTime TimeStarted { get; protected set; }

        /// <inheritdoc/>
        public virtual double Deadline { get; protected set; } = 200;

        /// <inheritdoc/>
        public virtual IBinding? Binding { get; set; }

        /// <summary>
        ///   The number of touches required to trigger the gesture. <br/>
        ///   Defaults to 1.
        /// </summary>
        public virtual int RequiredTouchesCount
        {
            get => _requiredTouchesCount;
            set
            {
                if (value < 1)
                    Log.Write("Touch Gestures", "The number of required touches cannot be less than 1, setting to 1.", LogLevel.Warning);

                _requiredTouchesCount = value;
                _invokingTouchesIndices = new int[value];
                _invokingTouches = new TouchPoint[value];
                _releasedTouches = new bool[value];

                Array.Fill(_invokingTouchesIndices, -1);

                //_currentTouches = new List<int>(value);
            }
        }

        #endregion

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

        #region Event Handlers

        /// <inheritdoc/>
        protected virtual void OnGestureStart(GestureStartedEventArgs e)
        {
            HasEnded = false;
            HasCompleted = false;
        }

        /// <inheritdoc/>
        protected virtual void OnGestureEnd(GestureEventArgs e)
        {
            HasStarted = false;
        }

        /// <inheritdoc/>
        protected virtual void OnGestureComplete(GestureEventArgs e)
        {
            // Reset the gesture.
            HasStarted = false;

            StartPosition = Vector2.Zero;
        }

        /// <inheritdoc/>
        public virtual void OnInput(TouchPoint[] points)
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
                    if (currentPointsCount == RequiredTouchesCount)
                    {
                        HasStarted = true;
                        StartPosition = points[0].Position;
                        TimeStarted = DateTime.Now;

                        // no index or ids are stored within the points
                        _invokingTouchesIndices = currentPointsIndices.ToArray();
                    }
                }
                else
                {
                    // check if the current points are the same as the invoking points
                    bool IsInvalidState = false;

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

                            // check if point is active
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

                    if (IsInvalidState)
                    {
                        HasEnded = true;
                    }
                    else
                    {
                        // check if the deadline has been reached
                        if ((DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
                            HasEnded = true;
                        else
                        {
                            // check if all touches have been released
                            if (_releasedTouches.All(released => released))
                                CompleteGesture();
                        }
                    }
                }
                _previousPoints = points;
            }
        }

        #endregion
    }
}