using System.Drawing;

namespace TouchGestures.Lib.Interfaces
{
    public interface IAbsolutePositionable
    {
        #region Properties

        /// <summary>
        ///    Check whether a situation needs to start in a specific area.
        /// </summary>
        bool IsAbsolute { get; }

        /// <summary>
        ///   The absolute bounds of the situation.
        /// </summary>
        Rectangle Bounds { get; }

        #endregion
    }
}