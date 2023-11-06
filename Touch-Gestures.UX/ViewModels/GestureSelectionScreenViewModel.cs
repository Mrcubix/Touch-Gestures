
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TouchGestures.UX.ViewModels.Controls.Tiles;
using static TouchGestures.UX.Extentions.ObservableCollectionExtensions;

namespace TouchGestures.UX.ViewModels;

public partial class GestureSelectionScreenViewModel : NavigableViewModel
{
    private string _searchText = string.Empty;

    private GestureTileViewModel[] _gestureTileViewModels = new GestureTileViewModel[]
    {
        new TapTileViewModel(),
        new HoldTileViewModel(),
        new PinchTileViewModel(),
        new PanTileViewModel(),
        new RotateTileViewModel(),
        new SwipeTileViewModel(),
        new NodeTileViewModel(),
    };

    [ObservableProperty]
    private ObservableCollection<GestureTileViewModel> _currentGestureTiles = new();

    public GestureSelectionScreenViewModel()
    {
        CurrentGestureTiles.AddRange(_gestureTileViewModels);

        foreach (var gestureTileViewModel in CurrentGestureTiles)
            gestureTileViewModel.Selected += OnGestureSelected;

        CanGoBack = true;
        NextViewModel = null;
    }

    public event EventHandler<GestureTileViewModel>? GestureSelected;

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            OnSearchTextChanged(value);
        }
    }

    #region Events

    public override event EventHandler? BackRequested;

    #endregion

    #region Methods

    protected override void GoBack() => BackRequested?.Invoke(this, EventArgs.Empty);

    #endregion

    #region Event Handlers

    private void OnSearchTextChanged(string text)
    {
        CurrentGestureTiles.Clear();

        if (string.IsNullOrWhiteSpace(text))
            CurrentGestureTiles.AddRange(_gestureTileViewModels);
        else
            CurrentGestureTiles.AddRange(_gestureTileViewModels.Where(x => GestureNameStartsWith(x, text)));
    }

    private void OnGestureSelected(object? sender, EventArgs e)
    {
        if (sender is GestureTileViewModel gestureTileViewModel)
            GestureSelected?.Invoke(this, gestureTileViewModel);
    }

    #endregion

    #region Static Methods

    private static bool GestureNameStartsWith(GestureTileViewModel gestureTileViewModel, string text)
    {
        return gestureTileViewModel.GestureName.ToLower().StartsWith(text.ToLower());
    }

    #endregion
}