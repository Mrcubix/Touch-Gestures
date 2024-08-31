using System.Drawing;
using System.Numerics;

namespace TouchGestures.Lib.Entities.Tablet.Touch
{
    public class TouchPoint
    {
        public TouchPoint(byte touchID, Vector2 position)
        {
            TouchID = touchID;
            Position = position;
        }

        public TouchPoint() { }

        public byte TouchID { get; set; }
        public Vector2 Position { get; set; }

        /// <summary>
        ///   Check whether a point is inside a rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="rectangle">The rectangle to check.</param>
        /// <returns>True if the point is inside the rectangle, false otherwise.</returns>
        public bool IsInside(Rectangle rectangle)
        {
            return Position.X >= rectangle.Left && Position.X <= rectangle.Right &&
                   Position.Y >= rectangle.Top && Position.Y <= rectangle.Bottom;
        }

        /// <summary>
        ///   Check whether a point is inside a SharedArea.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="area">The area to check.</param>
        /// <returns>True if the point is inside the area, false otherwise.</returns>
        public bool IsInside(SharedArea area)
        {
            var left = area.Position.X - (area.Width / 2);
            var right = left + area.Width;
            var top = area.Position.Y - (area.Height / 2);
            var bottom = top + area.Height;

            return Position.X >= left && Position.X <= right &&
                   Position.Y >= top && Position.Y <= bottom;
        }

        public override string ToString()
        {
            return $"Point #{TouchID}: {Position};";
        }
    }
}
