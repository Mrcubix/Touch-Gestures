
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using ReactiveUI;
using TouchGestures.UX.Controls.GestureTiles;

namespace TouchGestures.UX.ViewModels;

public class GestureSelectionScreenViewModel : ViewModelBase
{

    private string _searchText = string.Empty;

    private GestureTileViewModel[] _gestureTileViewModels = new[]
    {
        new GestureTileViewModel
        {
            GestureName = "Tap",
            Description = "A gesture completed by tapping with any specified number of fingers"
        },
        new GestureTileViewModel
        {
            GestureName = "Hold",
            Description = "A gesture completed by holding for a certain amount of time"
        },
        new GestureTileViewModel
        {
            GestureName = "Pinch",
            Description = "A gesture completed by pinching, simillar to how you would zoom in, in various applications"
        },
        new GestureTileViewModel
        {
            GestureName = "Pan",
            Description = "A gesture that progresses by panning until the final touch is released"
        },
        new GestureTileViewModel
        {
            GestureName = "Rotate",
            Description = "A gesture completed by rotating 2 fingers, similar to a pinch"
        },
        new GestureTileViewModel
        {
            GestureName = "Swipe",
            Description = "A gesture completed by swiping in a specific direction"
        }
    };

    private ObservableCollection<GestureTileViewModel> _currentGestureTiles = new();

    public GestureSelectionScreenViewModel()
    {
        _currentGestureTiles.AddRange(_gestureTileViewModels);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            OnSearchTextChanged(value);
        }
    }

    public ObservableCollection<GestureTileViewModel> CurrentGestureTiles
    {
        get => _currentGestureTiles;
        set => this.RaiseAndSetIfChanged(ref _currentGestureTiles, value);
    }

    private void OnSearchTextChanged(string text)
    {
        CurrentGestureTiles.Clear();

        if (string.IsNullOrWhiteSpace(text))
            CurrentGestureTiles.AddRange(_gestureTileViewModels);
        else
            CurrentGestureTiles.AddRange(_gestureTileViewModels.Where(x => GestureNameStartsWith(x, text)));
    }

    private static bool GestureNameStartsWith(GestureTileViewModel gestureTileViewModel, string text)
    {
        return gestureTileViewModel.GestureName.ToLower().StartsWith(text.ToLower());
    }
}