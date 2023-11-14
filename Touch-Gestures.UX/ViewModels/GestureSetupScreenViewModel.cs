using System;
using System.ComponentModel;
using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class GestureSetupScreenViewModel : NavigableViewModel
{
    #region Constructors

    public GestureSetupScreenViewModel()
    {
        BackRequested = null!;
        CanGoBack = true;
    }

    #endregion

    #region Events

    public override event EventHandler? BackRequested;

    #endregion

    #region Methods

    public void StartSetup(GestureSetupViewModel gestureSetupViewModel)
    {
        NextViewModel = gestureSetupViewModel;
        NextViewModel.BackRequested += OnBackRequestedAhead;

        gestureSetupViewModel.IsOptionsSelectionStepActive = true;
    }

    protected override void GoBack() => BackRequested?.Invoke(this, EventArgs.Empty);

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

    private void OnBackRequestedAhead(object? sender, EventArgs e) => GoBack();

    #endregion
}