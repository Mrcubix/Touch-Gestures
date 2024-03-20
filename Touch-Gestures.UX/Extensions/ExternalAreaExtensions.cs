using System.Numerics;
using OpenTabletDriver.External.Avalonia.ViewModels;

namespace TouchGestures.UX.Extentions;

public static class ExternalAreaExtensions
{
    public static OpenTabletDriver.Plugin.Area ToNativeArea(this Area area)
    {
        return new OpenTabletDriver.Plugin.Area
        {
            Position = new Vector2((float)area.X, (float)area.Y),
            Width = (float)area.Width,
            Height = (float)area.Height,
            Rotation = (float)area.Rotation
        };
    }
}