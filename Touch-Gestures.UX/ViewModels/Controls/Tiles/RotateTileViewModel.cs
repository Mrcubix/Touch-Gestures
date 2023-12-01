using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

using static TouchGestures.UX.Extentions.AssetLoaderExtensions;

#nullable enable

public partial class RotateTileViewModel : GestureTileViewModel
{
    public RotateTileViewModel()
    {
        GestureName = "Rotate";
        Description = "A gesture completed by rotating 2 fingers, similar to a pinch";
        Icon = LoadBitmap("Assets/Setups/Rotation/rotation_anti-clockwise.png");
        // Gesture Setup
    }
}