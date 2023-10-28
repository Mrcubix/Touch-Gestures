using System;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Input;


namespace TouchGestures.Lib.Interfaces
{
    public interface IGesture
    {
        #region Events

        /// <summary>
        ///   Invoked when the gesture starts.
        /// </summary>
        public event EventHandler<GestureStartedEventArgs> GestureStarted;

        /// <summary>
        ///   Invoked when the gesture ends.
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureEnded;

        /// <summary>
        ///   Invoked when the gesture is complete.
        /// </summary>
        public event EventHandler<GestureEventArgs> GestureCompleted;

        #endregion
        
        #region Properties

        public GestureType GestureType => throw new NotImplementedException();

        /// <summary>
        ///   Whether the gesture has started.
        /// </summary>
        public virtual bool HasStarted
        {
            get => throw new NotImplementedException();
            protected set => throw new NotImplementedException();
        }

        /// <summary>
        ///   Whether the gesture has ended.
        /// </summary>
        public virtual bool HasEnded
        {
            get => throw new NotImplementedException();
            protected set => throw new NotImplementedException();
        }

        /// <summary>
        ///   Whether the gesture has completed.
        /// </summary>
        public virtual bool HasCompleted
        {
            get => throw new NotImplementedException();
            protected set => throw new NotImplementedException();
        }

        /// <summary>
        ///   The binding associated with the gesture.
        /// </summary>
        public virtual IBinding? Binding
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
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
        }

        /// <summary>
        ///   Called when a touch input is received.
        /// </summary>
        /// <param name="points">The touch points.</param>
        /// <remarks>
        ///   This method is only called if the plugin is enabled &#38; the RPC server is ready.
        /// </remarks>
        public virtual void OnInput(TouchPoint[] points)
        {
        }

        #endregion
    }
}