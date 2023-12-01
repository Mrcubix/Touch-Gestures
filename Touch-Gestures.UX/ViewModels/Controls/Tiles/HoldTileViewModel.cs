using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

using static TouchGestures.UX.Extentions.AssetLoaderExtensions;

#nullable enable

public partial class HoldTileViewModel : GestureTileViewModel
{
    public HoldTileViewModel()
    {
        GestureName = "Hold";
        Description = "A gesture completed by holding for a certain amount of time";
        Icon = LoadBitmap("Assets/Setups/Hold/hold.png");
        // Gesture Setup
    }
}