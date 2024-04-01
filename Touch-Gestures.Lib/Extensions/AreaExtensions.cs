using System.Numerics;
using OpenTabletDriver.Plugin;

namespace TouchGestures.Lib.Extensions
{
    public static class AreaExtensions
    {
        public static Area Zero => new();

        public static bool IsZero(this Area? area)
        {
            if (area == null)
                return true;

            return area.Position == Vector2.Zero && area.Width == 0 && area.Height == 0 && area.Rotation == 0;
        }

        public static Area Add(this Area area, Area other)
        {
            return new Area
            {
                Position = area.Position + other.Position,
                Width = area.Width + other.Width,
                Height = area.Height + other.Height,
                Rotation = area.Rotation + other.Rotation
            };
        }

        public static Area Subtract(this Area area, Area other)
        {
            return new Area
            {
                Position = area.Position - other.Position,
                Width = area.Width - other.Width,
                Height = area.Height - other.Height,
                Rotation = area.Rotation - other.Rotation
            };
        }

        public static Area Multiply(this Area area, float factor)
        {
            return new Area
            {
                Position = area.Position * factor,
                Width = area.Width * factor,
                Height = area.Height * factor,
                Rotation = area.Rotation
            };
        }

        public static Area Divide(this Area area, float factor)
        {
            if (factor == 0)
                return Zero;

            return new Area
            {
                Position = area.Position / factor,
                Width = area.Width / factor,
                Height = area.Height / factor,
                Rotation = area.Rotation
            };
        }

        public static bool Contains(this Area area, Vector2 point)
        {
            // Area might be rotated so we need to check if the point is inside the rotated rectangle
            var angle = area.Rotation;

            var cos = (float) System.Math.Cos(-angle);
            var sin = (float) System.Math.Sin(-angle);

            var centeredPoint = point - area.Position;

            var rotatedX = (centeredPoint.X * cos) - (centeredPoint.Y * sin);
            var rotatedY = (centeredPoint.X * sin) + (centeredPoint.Y * cos);

            return rotatedX >= 0 && rotatedX <= area.Width && rotatedY >= 0 && rotatedY <= area.Height;
        }
    }
}