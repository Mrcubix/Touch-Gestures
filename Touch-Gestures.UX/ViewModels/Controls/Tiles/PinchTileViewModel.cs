using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

#nullable enable

public partial class PinchTileViewModel : GestureTileViewModel
{
    public PinchTileViewModel()
    {
        GestureName = "Pinch";
        Description = "A gesture completed by pinching, simillar to how you would zoom in, in various application";
        // Icon
    }

    public override GestureSetupViewModel BuildSetup()
    {
        return new GestureSetupViewModel();
        // should return a PinchSetupViewModel later
    }
}