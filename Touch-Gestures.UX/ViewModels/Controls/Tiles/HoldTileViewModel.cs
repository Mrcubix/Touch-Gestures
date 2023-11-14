using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

#nullable enable

public partial class HoldTileViewModel : GestureTileViewModel
{
    public HoldTileViewModel()
    {
        GestureName = "Hold";
        Description = "A gesture completed by holding for a certain amount of time";
        // Icon
        // Gesture Setup
    }
}