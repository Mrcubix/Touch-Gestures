using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

using static TouchGestures.UX.Extentions.AssetLoaderExtensions;

#nullable enable

public partial class TapTileViewModel : GestureTileViewModel
{
    public TapTileViewModel()
    {
        GestureName = "Tap";
        Description = "A gesture completed by tapping with any specified number of fingers";
        Icon = LoadBitmap("Assets/Setups/Tap/tap_triple.png");
        AssociatedSetup = new TapSetupViewModel();
    }
}