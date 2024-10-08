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

    /// <summary>
    ///   Start the gesture setup process.
    /// </summary>
    /// <param name="gestureSetupViewModel">The view model to start the setup with.</param>
    public void StartSetup(GestureSetupViewModel gestureSetupViewModel, bool isMultiTouch = false)
    {
        NextViewModel = gestureSetupViewModel;
        NextViewModel.BackRequested += OnBackRequestedAhead;

        // There shouldn't be a situation where an unsupported gesture should make its way here normally, 
        // as these are hidden during the selection process.

        gestureSetupViewModel.IsOptionsSelectionStepActive = false;

        // Check if single touch even matters for the setup.
        if (isMultiTouch || gestureSetupViewModel.SingleTouchOptionSelectionEnabled)
            gestureSetupViewModel.IsOptionsSelectionStepActive = true;
        else
            gestureSetupViewModel.IsBindingSelectionStepActive = true;
        
        gestureSetupViewModel.IsMultiTouchSetup = isMultiTouch;
    }

    protected override void GoBack()
    {
        if (NextViewModel != null)
            NextViewModel.BackRequested -= OnBackRequestedAhead;

        BackRequested?.Invoke(this, EventArgs.Empty);
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

    private void OnBackRequestedAhead(object? sender, EventArgs e) => GoBack();

    #endregion
}