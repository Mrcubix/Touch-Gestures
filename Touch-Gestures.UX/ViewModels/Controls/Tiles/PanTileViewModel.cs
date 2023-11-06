using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

#nullable enable

public partial class PanTileViewModel : GestureTileViewModel
{
    public PanTileViewModel()
    {
        GestureName = "Pan";
        Description = "A gesture that progresses by panning until the final touch is released";
        // Icon
    }

    public override GestureSetupViewModel BuildSetup()
    {
        return new GestureSetupViewModel();
        // should return a PanSetupViewModel later
    }
}