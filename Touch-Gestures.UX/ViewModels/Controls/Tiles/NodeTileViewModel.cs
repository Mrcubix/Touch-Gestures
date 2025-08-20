using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

public partial class NodeTileViewModel : GestureTileViewModel<GestureSetupViewModel>
{
    public NodeTileViewModel()
    {
        GestureName = "Node";
        Description = "A Node-Based gesture, built manually by the user using shapes";
        // Icon
        // Gesture Setup
    }
}