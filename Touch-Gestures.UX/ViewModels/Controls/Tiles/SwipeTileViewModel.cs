using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

using static TouchGestures.UX.Extentions.AssetLoaderExtensions;

#nullable enable

public partial class SwipeTileViewModel : GestureTileViewModel
{
    public SwipeTileViewModel()
    {
        GestureName = "Swipe";
        Description = "A gesture completed by swiping in a specific direction";
        Icon = LoadBitmap("Assets/Setups/Swipe/swipe_up.png");
        AssociatedSetup = new SwipeSetupViewModel();
    }
}