using System;
using System.Drawing;
using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Extensions;

namespace TouchGestures.Entities.Gestures
{
    /// <summary>
    ///   Represent a swipe gesture in any of the 8 directions in <see cref="SwipeDirection"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PanGesture : SwipeGesture
    {
        #region Constructors

        public PanGesture() : base() { }

        public PanGesture(Vector2 threshold) : base(threshold) { }

        public PanGesture(double deadline) : base(deadline) { }

        public PanGesture(Vector2 threshold, double deadline) : base(threshold, deadline) { }

        public PanGesture(SwipeDirection direction) : base(direction) { }

        public PanGesture(Vector2 threshold, double deadline, SwipeDirection direction)
            : base(threshold, deadline, direction) { }

        public PanGesture(Vector2 threshold, double deadline, SwipeDirection direction, Rectangle bounds)
            : base(threshold, deadline, direction, bounds) { }

        public PanGesture(Vector2 threshold, double deadline, SwipeDirection direction, SharedArea? bounds)
            : base(threshold, deadline, direction, bounds) { }

        #endregion

        public override void OnInput(TouchPoint[] points)
        {
            if (points.Length > 0)
            {
                var point = points[0];

                if (point != null)
                {
                    if (!HasStarted)
                    {
                        if (IsRestrained && Bounds != null && !point.IsInside(Bounds))
                            return;

                        StartPosition = point.Position;
                        IsInvalidState = false;
                        HasStarted = true;
                    }
                    else
                    {
                        if (Deadline != 0 && (DateTime.Now - TimeStarted).TotalMilliseconds >= Deadline)
                            IsInvalidState = true;

                        if (IsRestrained && Bounds != null && !point.IsInside(Bounds))
                            IsInvalidState = true;

                        if (IsInvalidState)
                        {
                            HasEnded = true;
                            return;
                        }

                        _delta = point.Position - StartPosition;

                        switch (Direction)
                        {
                            case SwipeDirection.Up:
                                if (_delta.Y <= -Threshold.Y)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.Down:
                                if (_delta.Y >= Threshold.Y)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.Left:
                                if (_delta.X <= -Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.Right:
                                if (_delta.X >= Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.UpLeft:
                                if (_delta.Y <= -Threshold.Y && _delta.X <= -Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.UpRight:
                                if (_delta.Y <= -Threshold.Y && _delta.X >= Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.DownLeft:
                                if (_delta.Y >= Threshold.Y && _delta.X <= -Threshold.X)
                                    CompleteGesture();
                                break;
                            case SwipeDirection.DownRight:
                                if (_delta.Y >= Threshold.Y && _delta.X >= Threshold.X)
                                    CompleteGesture();
                                break;
                        }
                    }
                }
                else
                {
                    // finger may have been lifted
                    if (HasStarted)
                        HasEnded = true;
                }
            }
        }
    }
}