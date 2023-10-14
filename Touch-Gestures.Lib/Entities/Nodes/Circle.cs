using System;
using System.Numerics;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.Lib.Entities.Nodes
{
    public class Circle : IGestureNode
    {
        private float _radius;
        private float _radiusSquared;

        /// <summary>
        ///   Index of the node in the gesture
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///   Shape of the node
        /// </summary>
        public GestureNodeShape Shape { get; init; } = GestureNodeShape.Circle;

        /// <summary>
        ///   Whether the node is the start of the gesture
        /// </summary>
        public bool IsGestureStart { get; set; }

        /// <summary>
        ///   Whether the node is the end of the gesture
        /// </summary>
        public bool IsGestureEnd { get; set; }

        /// <summary>
        ///   Center of the circle
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///   Radius of the circle
        /// </summary>
        public Vector2 Size
        {
            get => new Vector2(_radius);
            set
            {
                _radius = value.X;
                _radiusSquared = _radius * _radius;
            }
        }

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

        public Circle(float radius)
        {
            Position = Vector2.Zero;
            Size = new Vector2(radius);
        }

        public Circle(Vector2 position, float radius)
        {
            Position = position;
            Size = new Vector2(radius);
        }

        public Circle(Vector2 size)
        {
            Position = Vector2.Zero;
            Size = size;
        }

        public Circle(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        /// <summary>
        ///   Whether the given point is within the node
        /// </summary>
        /// <param name="position"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public bool IsWithinNode(Vector2 point, float timestamp)
        {
            var distanceSquared = Vector2.DistanceSquared(point, Position);
            var isWithinTimestamp = Math.Abs(Timestamp - TimestampTolerance) <= timestamp;

            return distanceSquared <= _radiusSquared && isWithinTimestamp;
        }
    }
}