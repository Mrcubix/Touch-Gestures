using System.Collections.ObjectModel;
using System.Linq;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

public partial class TapSetupViewModel : GestureSetupViewModel
{
    public TapSetupViewModel()
    {
        GestureSetupPickText = "Number of touches:";
        GestureSetupPickItems = new ObservableCollection<object>(Enumerable.Range(1, 10).Cast<object>());
        GestureSetupPickPreviews = new ObservableCollection<string>(Enumerable.Repeat("/Assets/Displays/placeholder.png", 10));
        GestureSetupPickPreviews[1] = "/Assets/Displays/placeholder-2.png";
    }
}
