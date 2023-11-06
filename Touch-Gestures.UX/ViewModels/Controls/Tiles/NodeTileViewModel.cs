using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles;

#nullable enable

public partial class NodeTileViewModel : GestureTileViewModel
{
    public NodeTileViewModel()
    {
        GestureName = "Node";
        Description = "A Node-Based gesture, built manually by the user using shapes";
        // Icon
    }

    public override GestureSetupViewModel BuildSetup()
    {
        return new GestureSetupViewModel();
        // should return a NodeSetupViewModel later
    }
}