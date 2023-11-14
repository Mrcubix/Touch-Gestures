using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

#nullable enable

public partial class RotateTileViewModel : GestureTileViewModel
{
    public RotateTileViewModel()
    {
        GestureName = "Rotate";
        Description = "A gesture completed by rotating 2 fingers, similar to a pinch";
        // Icon
        // Gesture Setup
    }
}