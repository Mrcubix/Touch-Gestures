using System.Numerics;

namespace TouchGestures.Lib.Entities.Tablet
{
    public class SharedTabletDigitizer
    {
        /// <summary>
        /// The tablet's horizontal active area in millimeters.
        /// </summary>
        public float Width { set; get; }

        /// <summary>
        /// The tablet's vertical active area in millimeters.
        /// </summary>
        public float Height { set; get; }

        /// <summary>
        /// The tablet's maximum horizontal input.
        /// </summary>
        public float MaxX { set; get; }

        /// <summary>
        /// The tablet's maximum vertical input.
        /// </summary>
        public float MaxY { set; get; }

        public Vector2 GetLPMM()
        {
            return new Vector2(MaxX / Width, MaxY / Height);
        }
    }
}