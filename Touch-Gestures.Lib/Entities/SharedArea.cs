using System.Numerics;
using Newtonsoft.Json;

namespace TouchGestures.Lib.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SharedArea
    {
        [JsonConstructor]
        public SharedArea()
        {
        }

        public SharedArea(double width, double height, Vector2 position, double rotation)
        {
            Width = width;
            Height = height;
            Position = position;
            Rotation = rotation;
        }

        [JsonProperty]
        public double Width { get; set; }

        [JsonProperty]
        public double Height { get; set; }

        [JsonProperty]
        public Vector2 Position { get; set; }

        [JsonProperty]
        public double Rotation { get; set; }

        public static readonly SharedArea Zero = new();

        public bool IsZero()
        {
            return Position == Vector2.Zero && Width == 0 && Height == 0 && Rotation == 0;
        }

        public SharedArea Add(SharedArea other)
        {
            return new SharedArea
            {
                Position = Position + other.Position,
                Width = Width + other.Width,
                Height = Height + other.Height,
                Rotation = Rotation + other.Rotation
            };
        }

        public SharedArea Subtract(SharedArea other)
        {
            return new SharedArea
            {
                Position = Position - other.Position,
                Width = Width - other.Width,
                Height = Height - other.Height,
                Rotation = Rotation - other.Rotation
            };
        }

        public SharedArea Multiply(float factor)
        {
            return new SharedArea
            {
                Position = Position * factor,
                Width = Width * factor,
                Height = Height * factor,
                Rotation = Rotation
            };
        }

        public SharedArea Divide(float factor)
        {
            return new SharedArea
            {
                Position = Position / factor,
                Width = Width / factor,
                Height = Height / factor,
                Rotation = Rotation
            };
        }

        public bool Contains(Vector2 point)
        {
            // Area might be rotated so we need to check if the point is inside the rotated rectangle
            var angle = Rotation;

            var cos = (float) System.Math.Cos(-angle);
            var sin = (float) System.Math.Sin(-angle);

            var centeredPoint = point - Position;

            var rotatedX = (centeredPoint.X * cos) - (centeredPoint.Y * sin);
            var rotatedY = (centeredPoint.X * sin) + (centeredPoint.Y * cos);

            return rotatedX >= 0 && rotatedX <= Width && rotatedY >= 0 && rotatedY <= Height;
        }

        public override string ToString()
        {
            return $"{Width};{Height};{Position.X};{Position.Y};{Rotation}";
        }
    }
}