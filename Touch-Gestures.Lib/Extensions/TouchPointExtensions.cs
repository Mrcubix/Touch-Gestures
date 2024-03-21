using System.Drawing;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Entities;

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

        /// <summary>
        ///   Check whether a point is inside an Area.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="area">The area to check.</param>
        /// <returns>True if the point is inside the area, false otherwise.</returns>
        public static bool IsInside(this TouchPoint point, Area area)
        {
            var position = point.Position;

            var left = area.Position.X - area.Width / 2;
            var right = area.Position.X + area.Width / 2;
            var top = area.Position.Y - area.Height / 2;
            var bottom = area.Position.Y + area.Height / 2;

            return position.X >= left && position.X <= right &&
                   position.Y >= top && position.Y <= bottom;
        }

        /// <summary>
        ///   Check whether a point is inside a SharedArea.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="area">The area to check.</param>
        /// <returns>True if the point is inside the area, false otherwise.</returns>
        public static bool IsInside(this TouchPoint point, SharedArea area)
        {
            var position = point.Position;

            var left = area.Position.X - area.Width / 2;
            var right = area.Position.X + area.Width / 2;
            var top = area.Position.Y - area.Height / 2;
            var bottom = area.Position.Y + area.Height / 2;

            return position.X >= left && position.X <= right &&
                   position.Y >= top && position.Y <= bottom;
        }

        #endregion
    }
}