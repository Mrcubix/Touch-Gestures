using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Lib.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IGesture
    {
        #region Properties

        /// <summary>
        ///   Whether the gesture has started.
        /// </summary>
        bool HasStarted { get; }

        /// <summary>
        ///   Whether the gesture has ended.
        /// </summary>
        bool HasEnded { get; }

        /// <summary>
        ///   Whether the gesture has completed.
        /// </summary>
        bool HasCompleted { get; }

        /// <summary>
        ///   Whether the gesture is relative or absolute.
        /// </summary>
        bool IsRestrained { get; }

        /// <summary>
        ///   The number of lines per millimeter.
        /// </summary>
        float LinesPerMM { get; }

        #endregion

        #region Methods

        /// <summary>
        ///   Ends the gesture prematurely.
        /// </summary>
        void End();

        #endregion

        #region Event Handlers

        /// <summary>
        ///   Called when a touch input is received.
        /// </summary>
        /// <param name="points">The touch points.</param>
        /// <remarks>
        ///   This method is only called if the plugin is enabled &#38; the RPC server is ready.
        /// </remarks>
        void OnInput(TouchPoint[] points);

        #endregion
    }
}