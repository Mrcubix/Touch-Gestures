using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TouchGestures.UX.ViewModels.Controls.Setups;
using TouchGestures.UX.ViewModels.Controls.Tiles;

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class GestureSetupWizardViewModel : NavigableViewModel
{
    private GestureTileViewModel? _previouslySelectedTile;

    // Step 1 : Select Gesture
    [ObservableProperty]
    private GestureSelectionScreenViewModel _gestureSelectionScreenViewModel = new();

    [ObservableProperty]
    private GestureSetupScreenViewModel _gestureSetupScreenViewModel = new();

    // Step 2 : Change Gesture Trigger (Gesture Setup)


    // Step 3 : Set Gesture Binding (Gesture Setup)

    public GestureSetupWizardViewModel()
    {
        NextViewModel = _gestureSelectionScreenViewModel;

        NextViewModel!.PropertyChanging += OnCurrentGestureSetupChanging;
        NextViewModel!.PropertyChanged += OnCurrentGestureSetupChanged;

        GestureSelectionScreenViewModel.BackRequested += OnBackRequestedAhead;
        GestureSelectionScreenViewModel.GestureSelected += OnGestureSelected;

        GestureSetupScreenViewModel.BackRequested += OnBackRequestedAhead;
    }

    #region Events

    public override event EventHandler? BackRequested;

    #endregion

    #region Methods

    protected override void GoBack()
    {
        if (NextViewModel is GestureSelectionScreenViewModel)
            BackRequested?.Invoke(this, EventArgs.Empty);
        else
            NextViewModel = GestureSelectionScreenViewModel;
    }

    #endregion

    #region Event Handlers

    private void OnCurrentGestureSetupChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (NextViewModel != null)
            NextViewModel.BackRequested -= OnBackRequestedAhead;
    }

    private void OnCurrentGestureSetupChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (NextViewModel != null)
            NextViewModel.BackRequested += OnBackRequestedAhead;
    }

    private void OnGestureSelected(object? sender, GestureTileViewModel selectedTile)
    {
        if (selectedTile == null)
            throw new ArgumentNullException(nameof(selectedTile), "A selected tile cannot be null.");

        if (_previouslySelectedTile != selectedTile)
        {
            _previouslySelectedTile = selectedTile;
            GestureSetupScreenViewModel.StartSetup(selectedTile.BuildSetup());
        }

        NextViewModel = GestureSetupScreenViewModel;
    }

    private void OnBackRequestedAhead(object? sender, EventArgs e) => GoBack();

    #endregion
}