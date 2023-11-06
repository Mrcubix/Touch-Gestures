using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class MainViewModel : NavigableViewModel
{
    // Home Menu

    // Bindings Overview
      // Add Button
        // -> GestureSetupWizard
          // Gesture Selection
          // Gesture Trigger Selection
          // Gesture Action Selection
            // Add Button

    // Settings

    [ObservableProperty]
    private NodeGestureEditorViewModel _nodeGestureEditorViewModel = new();

    [ObservableProperty]
    private GestureSetupWizardViewModel _gestureSetupWizardViewModel = new();

    #region Constructors

    public MainViewModel()
    {
        BackRequested = null!;

        CanGoBack = false;
        NextViewModel = _gestureSetupWizardViewModel;

        NextViewModel!.PropertyChanged += OnCurrentGestureSetupChanged;
        NextViewModel!.PropertyChanging += OnCurrentGestureSetupChanging;
        NextViewModel!.BackRequested += OnBackRequestedAhead;
    }

    #endregion

    #region Events

    public override event EventHandler? BackRequested;

    #endregion

    #region Methods

    protected override void GoBack()
    {
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

    private void OnBackRequestedAhead(object? sender, EventArgs e)
    {
        NextViewModel = this;
    }

    #endregion
}
