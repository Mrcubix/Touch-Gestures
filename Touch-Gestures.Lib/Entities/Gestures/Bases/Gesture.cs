using System;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Input;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Gesture : IGesture
    {
        #region Fields

        protected SharedArea? _bounds = SharedArea.Zero;
        protected Vector2 _linesPerMM = Vector2.One;

        #endregion

        #region Events

        /// <summary>
        ///   Invoked when the gesture starts.
        /// </summary>
        public abstract event EventHandler<GestureStartedEventArgs>? GestureStarted;

        /// <summary>
        ///   Invoked when the gesture ends.
        /// </summary>
        public abstract event EventHandler<GestureEventArgs>? GestureEnded;

        /// <summary>
        ///   Invoked when the gesture is complete.
        /// </summary>
        public abstract event EventHandler<GestureEventArgs>? GestureCompleted;

        #endregion

        #region Properties

        /// <inheritdoc />
        public abstract bool HasStarted { get; protected set; }

        /// <inheritdoc />
        public abstract bool HasEnded { get; protected set; }

        /// <inheritdoc />
        public abstract bool HasCompleted { get; protected set; }

        /// <summary>
        ///    Check whether a situation needs to start in a specific area.
        /// </summary>
        public virtual bool IsRestrained { get; protected set; }

        /// <summary>
        ///   The absolute bounds of the situation.
        /// </summary>
        [JsonProperty]
        public virtual SharedArea? Bounds
        {
            get => _bounds?.Divide(LinesPerMM);
            set => _bounds = value?.Multiply(LinesPerMM);
        }

        /// <inheritdoc />
        public virtual Vector2 LinesPerMM
        {
            get => _linesPerMM;
            set
            {
                var oldBounds = Bounds;
                _linesPerMM = value;

                // Update the bounds
                if (_linesPerMM != Vector2.Zero)
                    Bounds = oldBounds;
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public virtual void End()
        {
            HasEnded = true;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///   Called when the gesture starts.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        ///   This happen when the gesture is started.
        /// </remarks>
        protected virtual void OnGestureStart(GestureStartedEventArgs e)
        {
            HasEnded = false;
            HasStarted = true;
        }

        /// <summary>
        ///   Called when the gesture ends.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        ///   This happen either when the gesture is complete or when the gesture is cancelled.
        /// </remarks>
        protected virtual void OnGestureEnd(GestureEventArgs e)
        {
            HasStarted = false;
        }

        /// <summary>
        ///   Called when the gesture is complete.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        ///   This happen when the gesture is completed.
        /// </remarks>
        protected virtual void OnGestureComplete(GestureEventArgs e)
        {
            HasStarted = false;
        }

        /// <inheritdoc />
        public abstract void OnInput(TouchPoint[] points);

        #endregion
    }
}