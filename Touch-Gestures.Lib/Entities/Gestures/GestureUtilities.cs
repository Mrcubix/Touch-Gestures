using System.Collections.Generic;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace TouchGestures.Lib.Input
{
    public static class GestureUtilities
    {
        /// <summary>
        ///   Get the currently active points.
        /// </summary>
        /// <param name="points">The array of points to check</param>
        /// <param name="currentIndex">The current index of the points array</param>
        /// <returns>returns the currently active points, as well as the current index and if the gesture is invalid.</returns>
        public static List<TouchPoint> GetActivePoints(TouchPoint[] points, int requiredTouchesCount, ref bool isInvalidState, out int currentIndex)
        {
            currentIndex = 0;
            isInvalidState = false;

            var currentPoints = new List<TouchPoint>(requiredTouchesCount);

            for (int i = 0; i < points.Length; i++)
            {
                TouchPoint point = points[i];

                // point is active
                if (point != null)
                {
                    // check if currentIndex is less than the required touches count
                    if (currentIndex < requiredTouchesCount)
                    {
                        // add the point to the current points array
                        currentPoints.Add(point);
                        currentIndex++;
                    }
                    else
                    {
                        // if currentIndex is greater than the required touches count, then the gesture is invalid
                        isInvalidState = true;
                        return currentPoints;
                    }
                }
            }

            return currentPoints;
        }
    }
}