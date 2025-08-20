using System;
using System.ComponentModel;
using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels;

public partial class GestureSetupScreenViewModel : NavigableViewModel
{
    #region Constructors

    public GestureSetupScreenViewModel() => CanGoBack = true;

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
        // TODO : Remove this once a better way to update the current step Template is found
        NextViewModel.PropertyChanged += OnPropertyChanged;

        // There shouldn't be a situation where an unsupported gesture should make its way here normally, 
        // as these are hidden during the selection process.

        gestureSetupViewModel.IsMultiTouchSetup = isMultiTouch;

        gestureSetupViewModel.CurrentStep = -1;
        gestureSetupViewModel.GoNextCommand.Execute(null);
    }

    protected override void GoBack()
    {
        if (NextViewModel != null)
        {
            NextViewModel.BackRequested -= OnBackRequestedAhead;
            // TODO : Remove this once a better way to update the current step Template is found
            NextViewModel.PropertyChanged -= OnPropertyChanged;
        }

        NextViewModel = null;

        base.GoBack();
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

    // TODO : Remove this once a better way to update the current step Template is found
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GestureSetupViewModel.CurrentStep) && NextViewModel is GestureSetupViewModel)
        {
            var previous = NextViewModel;
            NextViewModel = null;
            NextViewModel = previous;
        }
    }

    #endregion
}