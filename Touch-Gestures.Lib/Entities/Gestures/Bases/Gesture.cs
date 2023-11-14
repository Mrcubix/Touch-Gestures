using System;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Input;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Gestures.Bases
{

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Gesture : IGesture
    {
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
        [JsonProperty]
        public abstract GestureKind GestureKind { get; }

        /// <inheritdoc />
        public abstract bool HasStarted { get; protected set; }

        /// <inheritdoc />
        public abstract bool HasEnded { get; protected set; }

        /// <inheritdoc />
        public abstract bool HasCompleted { get; protected set; }

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