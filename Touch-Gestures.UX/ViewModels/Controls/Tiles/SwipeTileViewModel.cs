using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

#nullable enable

public partial class SwipeTileViewModel : GestureTileViewModel
{
    public SwipeTileViewModel()
    {
        GestureName = "Swipe";
        Description = "A gesture completed by swiping in a specific direction";
        // Icon
    }

    public override GestureSetupViewModel BuildSetup()
    {
        return new GestureSetupViewModel();
        // should return a SwipeSetupViewModel later
    }
}