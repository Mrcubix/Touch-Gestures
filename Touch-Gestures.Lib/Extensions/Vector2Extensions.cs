using System;
using System.Numerics;

namespace TouchGestures.Lib.Extensions
{
    public static class Vector2Extensions
    {
        public static float Dot(this Vector2 vector, Vector2 other) => Vector2.Dot(vector, other);
        
        public static Vector2 Round(this Vector2 vector, int decimals) 
        {
            decimal roundedX = Math.Round((decimal)vector.X, decimals);
            decimal roundedY = Math.Round((decimal)vector.Y, decimals);

            vector.X = (float)roundedX;
            vector.Y = (float)roundedY;

            return vector;
        }
    }
}