using System.Numerics;

namespace TouchGestures.Lib.Extensions
{
    public static class Vector2Extensions
    {
        public static float Dot(this Vector2 vector, Vector2 other) => Vector2.Dot(vector, other);
    }
}