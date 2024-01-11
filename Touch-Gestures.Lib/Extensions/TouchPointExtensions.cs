using System.Drawing;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Lib.Extensions
{
    public static class TouchPointExtensions
    {
        #region Methods

        /// <summary>
        ///   Check whether a point is inside a rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="rectangle">The rectangle to check.</param>
        /// <returns>True if the point is inside the rectangle, false otherwise.</returns>
        public static bool IsInside(this TouchPoint point, Rectangle rectangle)
        {
            var position = point.Position;

            return position.X >= rectangle.Left && position.X <= rectangle.Right &&
                   position.Y >= rectangle.Top && position.Y <= rectangle.Bottom;
        }

        #endregion
    }
}