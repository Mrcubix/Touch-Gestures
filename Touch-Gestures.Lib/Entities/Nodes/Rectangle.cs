using System;
using System.Numerics;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Nodes
{
    public class Rectangle : IGestureNode
    {
        /// <summary>
        ///   Index of the node in the gesture
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///   Shape of the node
        /// </summary>
        public GestureNodeShape Shape { get; init; } = GestureNodeShape.Rectangle;

        /// <summary>
        ///   Whether the node is the start of the gesture
        /// </summary>
        public bool IsGestureStart { get; set; }

        /// <summary>
        ///   Whether the node is the end of the gesture
        /// </summary>
        public bool IsGestureEnd { get; set; }

        /// <summary>
        ///   Top left corner of the rectangle
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///   Width and height of the rectangle
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        ///   Timestamp at which the node needs to be reached
        /// </summary>
        public float Timestamp { get; set; }

        /// <summary>
        ///   Tolerance for the timestamp
        /// </summary>
        public float TimestampTolerance { get; set; }

        /// <summary>
        ///   Whether the node needs to be held for a certain duration
        /// </summary>
        public bool IsHold { get; set; }

        /// <summary>
        ///   Duration for which the node needs to be held
        /// </summary>
        public float HoldDuration { get; set; }

        public Rectangle(Vector2 size)
        {
            Position = Vector2.Zero;
            Size = size;
        }

        public Rectangle(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Rectangle(float width, float height)
        {
            Position = Vector2.Zero;
            Size = new Vector2(width, height);
        }

        public Rectangle(float left, float top, float width, float height)
        {
            Position = new Vector2(left, top);
            Size = new Vector2(width, height);
        }

        /// <summary>
        ///   Checks whether the given position is within the node
        /// </summary>
        /// <param name="position"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public bool IsWithinNode(Vector2 position, float timestamp)
        {
            var isWithinWidth = position.X >= Position.X && position.X <= Position.X + Size.X;
            var isWithinHeight = position.Y >= Position.Y && position.Y <= Position.Y + Size.Y;
            var isWithinTimestamp = Math.Abs(Timestamp - TimestampTolerance) <= timestamp;
            return isWithinWidth && isWithinHeight && isWithinTimestamp;
        }
    }
}