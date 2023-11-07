using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace TouchGestures.UX.Extentions;

public static class AssetLoaderExtensions
{
    private static Uri _baseUri = new("avares://Touch-Gestures.UX");

    public static Bitmap?[] LoadBitmaps(params Uri[] uris)
    {
        var bitmaps = new Bitmap?[uris.Length];

        for (var i = 0; i < uris.Length; i++)
        {
            var uri = uris[i];

            if (AssetLoader.Exists(uri))
                bitmaps[i] = new Bitmap(AssetLoader.Open(uri, _baseUri));
        }

        return bitmaps;
    }

    public static Bitmap?[] LoadBitmaps(params string[] paths)
    {
        var bitmaps = new Bitmap?[paths.Length];

        for (var i = 0; i < paths.Length; i++)
        {
            var path = paths[i];
            var uri = new Uri(_baseUri, path);

            if (AssetLoader.Exists(uri))
                bitmaps[i] = new Bitmap(AssetLoader.Open(uri));
        }

        return bitmaps;
    }
}