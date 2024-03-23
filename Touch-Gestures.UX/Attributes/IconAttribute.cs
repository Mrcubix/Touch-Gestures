using System;
using Avalonia.Media.Imaging;
using TouchGestures.UX.Extentions;

namespace TouchGestures.UX.Attributes;

using static AssetLoaderExtensions;


[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class IconAttribute : Attribute
{
    public IconAttribute(string icon)
    {
        Icon = LoadBitmap(icon);
    }

    public IconAttribute(Bitmap icon)
    {
        Icon = icon;
    }

    public Bitmap? Icon { get; }
}