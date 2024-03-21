using System.Numerics;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities;

namespace TouchGestures.UX.Extentions;

public static class ExternalAreaExtensions
{
    public static SharedArea ToSharedArea(this Area area)
    {
        return new SharedArea
        {
            Position = new Vector2((float)area.X, (float)area.Y),
            Width = area.Width,
            Height = area.Height,
            Rotation = area.Rotation
        };
    }
}