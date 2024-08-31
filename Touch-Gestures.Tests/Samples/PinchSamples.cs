using System;
using System.Numerics;
using TouchGestures.Lib.Entities.Tablet.Touch;

namespace TouchGestures.Tests.Samples
{
    public static class PinchSamples
    {
        public const double DegToRad = Math.PI / 180;
        public const int TOUCH_REQUIRED = 2;
        public static Vector2 CircleOrigin { get; } = new(50, 50);

        #region Sample Data

        // A report with no touch points
        public static readonly TouchPoint[] NoTouchSample = new TouchPoint[TOUCH_REQUIRED];

        // A report with two touch points, requirements for a pinch gesture. centered & contained within in a 100 x 100 area
        public static readonly TouchPoint[] OuterPinchInitialSample = new TouchPoint[TOUCH_REQUIRED]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(50, 50),
            },
            new()
            {
                TouchID = 1,
                Position = new Vector2(50, 50),
            },
        };

        // A report with two touch points, requirements for a pinch gesture. centered & contained within in a 100 x 100 area
        public static readonly TouchPoint[] InnerPinchInitialSample = new TouchPoint[TOUCH_REQUIRED]
        {
            new()
            {
                TouchID = 0,
                Position = new Vector2(0, 0),
            },
            new()
            {
                TouchID = 1,
                Position = new Vector2(100, 100),
            },
        };

        #endregion

        #region Genrators

        /// <summary>
        ///   Generate a series of reports that simulate a pinch gesture
        /// </summary>
        /// <param name="n">The number of reports to generate</param>
        /// <param name="origins">The origin of the pinch</param>
        /// <param name="targets">The target points to pinch towards</param>
        /// <returns>A series of reports</returns>
        public static TouchPoint[][] GeneratePinchReportsTowardsTarget(int n, TouchPoint[] origins, Vector2[] targets)
        {
            var reports = new TouchPoint[n + 1][];

            var firstPoint = origins[0];
            var secondPoint = origins[1];

            var firstXIncrement = (targets[0].X - firstPoint.Position.X) / n;
            var firstYIncrement = (targets[0].Y - firstPoint.Position.Y) / n;

            var secondXIncrement = (targets[1].X - secondPoint.Position.X) / n;
            var secondYIncrement = (targets[1].Y - secondPoint.Position.Y) / n;

            for (int i = 0; i < n + 1; i++)
            {
                var report = new TouchPoint[TOUCH_REQUIRED];

                // Generate the touch points
                report[0] = new TouchPoint
                {
                    TouchID = 0,
                    Position = new Vector2(firstPoint.Position.X + (firstXIncrement * i), firstPoint.Position.Y + (firstYIncrement * i)),
                };

                report[1] = new TouchPoint
                {
                    TouchID = 1,
                    Position = new Vector2(secondPoint.Position.X + (secondXIncrement * i), secondPoint.Position.Y + (secondYIncrement * i)),
                };

                reports[i] = report;
            }

            return reports;
        }

        /// <summary>
        ///   Generate a series of reports that simulate a rotation gesture
        /// </summary>
        /// <param name="n">The number of reports to generate</param>
        /// <param name="origins">The origin of the rotation</param>
        /// <param name="targets">The target points to rotate towards</param>
        /// <returns>A series of reports</returns>
        public static TouchPoint[][] GenerateRotateReportsTowardsTarget(int n, Vector2 origin, bool isClockwise)
        {
            var reports = new TouchPoint[n + 1][];

            double degreeIncrement = 90.0 / n;

            if (!isClockwise)
                degreeIncrement *= -1;

            // A spiral path
            for (int i = 0; i < n + 1; i++)
            {
                var report = new TouchPoint[TOUCH_REQUIRED];

                // Generate the touch points, representing 2 circular paths going in opposite directions
                // one starts at 0 towards 90 degrees, the other starts at 180 towards 270 degrees
                report[0] = new TouchPoint
                {
                    TouchID = 0,
                    Position = GeneratePointInCircle(origin, 50, (i * degreeIncrement) * DegToRad),
                };

                report[1] = new TouchPoint
                {
                    TouchID = 1,
                    Position = GeneratePointInCircle(origin, 50, ((i * degreeIncrement) + 180) * DegToRad),
                };

                reports[i] = report;
            }

            return reports;
        }

        /// <summary>
        ///   Generate a point in a circle
        /// </summary>
        /// <param name="origin">The origin of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="angle">The angle in degrees</param>
        /// <returns>A point in the circle</returns>
        public static Vector2 GeneratePointInCircle(Vector2 origin, float radius, double angle)
        {
            var x = origin.X + (float)(Math.Cos(angle) * radius);
            var y = origin.Y + (float)(Math.Sin(angle) * radius);

            return new Vector2(x, y);
        }

        #endregion
    }
}