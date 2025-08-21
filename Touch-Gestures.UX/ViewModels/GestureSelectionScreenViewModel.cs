using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TouchGestures.UX.ViewModels.Controls.Setups;
using TouchGestures.UX.ViewModels.Controls.Tiles;
using static TouchGestures.UX.Extentions.ObservableCollectionExtensions;

namespace TouchGestures.UX.ViewModels;

public partial class GestureSelectionScreenViewModel : NavigableViewModel
{
    #region Fields

    private readonly bool _isMultiTouchDevice;
    private string _searchText = string.Empty;
    
    /// <summary>
    ///   This array contains setup for different gestures, using different view models
    /// </summary>
    private GestureTileViewModel[] _gestureTileViewModels =
    [
        new TapTileViewModel(),
        new HoldTileViewModel(),
        new SwipeTileViewModel(),
        new PanTileViewModel(),
        new PinchTileViewModel(),
        new RotateTileViewModel(),

        //new NodeTileViewModel()
    ];

    [ObservableProperty]
    private ObservableCollection<GestureTileViewModel> _currentGestureTiles = new();

    #endregion

    #region Constructors

    public GestureSelectionScreenViewModel(bool isMultiTouch = true)
    {
        _isMultiTouchDevice = isMultiTouch;
        AddAllTiles();

        foreach (var gestureTileViewModel in CurrentGestureTiles)
            gestureTileViewModel.Selected += OnGestureSelected;

        CanGoBack = true;
        NextViewModel = null;
    }

    #endregion

    #region Events

    public event EventHandler<GestureTileViewModel>? GestureSelected;

    #endregion

    #region Properties

    /// <summary>
    ///   The search text to filter the gestures.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            OnSearchTextChanged(value);
        }
    }

    #endregion

    #region Methods

    private void AddAllTiles()
    {
        if (_isMultiTouchDevice)
            CurrentGestureTiles.AddRange(_gestureTileViewModels);
        else
            CurrentGestureTiles.AddRange(_gestureTileViewModels.Where(x => IsMultiTouchGesture(false, x)));
    }

    #endregion

    #region Event Handlers

    /// <summary>
    ///   Filter the gestures based on the search text.
    /// </summary>
    /// <param name="text">The search text.</param>
    private void OnSearchTextChanged(string text)
    {
        CurrentGestureTiles.Clear();

        if (string.IsNullOrWhiteSpace(text))
            AddAllTiles();
        else
            CurrentGestureTiles.AddRange(_gestureTileViewModels.Where(x => GestureNameStartsWith(x, text) && IsMultiTouchGesture(_isMultiTouchDevice, x)));
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
        return gestureTileViewModel.GestureName.StartsWith(text.ToLower(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMultiTouchGesture(bool isMultiTouchDevice, GestureTileViewModel gestureTileViewModel)
    {
        return isMultiTouchDevice || gestureTileViewModel.IsMultiTouchOnly == false;
    }

    #endregion
}