using TouchGestures.Lib.Entities;

namespace TouchGestures.Lib.Interfaces
{
    public interface IAbsolutePositionable
    {
        #region Properties

        /// <summary>
        ///    Check whether a situation needs to start in a specific area.
        /// </summary>
        bool IsRestrained { get; }

        /// <summary>
        ///   The absolute bounds of the situation.
        /// </summary>
        SharedArea? Bounds { get; }

        #endregion
    }
}