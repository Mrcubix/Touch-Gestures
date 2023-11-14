using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

#nullable enable

public partial class TapTileViewModel : GestureTileViewModel
{
    public TapTileViewModel()
    {
        GestureName = "Tap";
        Description = "A gesture completed by tapping with any specified number of fingers";
        // Icon
        AssociatedSetup = new TapSetupViewModel();
    }
}