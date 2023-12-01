using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Serializables.Gestures;
using TouchGestures.UX.Events;
using TouchGestures.UX.ViewModels.Controls.Setups;
using TouchGestures.UX.ViewModels.Controls.Tiles;

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class GestureSetupWizardViewModel : NavigableViewModel
{
    private Gesture _editedGesture = null!;

    #region Observable Fields

    // Step 1 : Select Gesture
    [ObservableProperty]
    private GestureSelectionScreenViewModel _gestureSelectionScreenViewModel = new();

    [ObservableProperty]
    private GestureSetupScreenViewModel _gestureSetupScreenViewModel = new();

    // Step 2 : Change Gesture Trigger (Gesture Setup)


    // Step 3 : Set Gesture Binding (Gesture Setup)

    #endregion

    #region Constructors

    public GestureSetupWizardViewModel()
    {
        NextViewModel = _gestureSelectionScreenViewModel;

        NextViewModel!.PropertyChanging += OnCurrentGestureSetupChanging;
        NextViewModel!.PropertyChanged += OnCurrentGestureSetupChanged;

        GestureSelectionScreenViewModel.BackRequested += OnBackRequestedAhead;
        GestureSelectionScreenViewModel.GestureSelected += OnGestureSelected;

        GestureSetupScreenViewModel.BackRequested += OnBackRequestedAhead;
    }

    #endregion

    #region Events

    public override event EventHandler? BackRequested;

    public event EventHandler<GestureAddedEventArgs>? SetupCompleted;

    public event EventHandler<GestureChangedEventArgs>? EditCompleted;

    #endregion

    #region Methods

    protected override void GoBack()
    {
        if (NextViewModel is GestureSelectionScreenViewModel || (NextViewModel is GestureSetupScreenViewModel && _editedGesture != null))
            BackRequested?.Invoke(this, EventArgs.Empty);
        else
            NextViewModel = GestureSelectionScreenViewModel;
    }

    public void Edit(GestureBindingDisplayViewModel bindingDisplay)
    {
        if (bindingDisplay == null)
            throw new ArgumentNullException(nameof(bindingDisplay), "A binding display cannot be null.");

        _editedGesture = bindingDisplay.AssociatedGesture;

        // now we need to generate the correct setup view model
        GestureSetupViewModel setupViewModel = _editedGesture switch
        {
            SerializableTapGesture => new TapSetupViewModel(_editedGesture),
            SerializableSwipeGesture => new GestureSetupViewModel(),
            _ => throw new InvalidOperationException("The gesture type is not supported.")
        };

        setupViewModel.BindingDisplay = new(bindingDisplay.Description!, bindingDisplay.Content!, bindingDisplay.PluginProperty);

        setupViewModel.EditCompleted += OnEditCompleted;

        GestureSetupScreenViewModel.StartSetup(setupViewModel);
        NextViewModel = GestureSetupScreenViewModel;
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

        selectedTile.AssociatedSetup.SetupCompleted += OnSetupCompleted;
        selectedTile.AssociatedSetup.EditCompleted += OnEditCompleted;

        GestureSetupScreenViewModel.StartSetup(selectedTile.AssociatedSetup);
        NextViewModel = GestureSetupScreenViewModel;
    }

    private void OnBackRequestedAhead(object? sender, EventArgs e) => GoBack();

    private void OnSetupCompleted(object? sender, EventArgs e)
    {
        if (sender is not GestureSetupViewModel setup)
            throw new InvalidOperationException("The sender must be a GestureSetupViewModel.");

        setup.SetupCompleted -= OnSetupCompleted;
        setup.EditCompleted -= OnEditCompleted;

        var args = new GestureAddedEventArgs(setup);

        SetupCompleted?.Invoke(this, args);
    } 

    private void OnEditCompleted(object? sender, EventArgs e)
    {
        if (sender is not GestureSetupViewModel gestureSetupViewModel)
            throw new InvalidOperationException("The sender must be a GestureSetupViewModel.");

        if (_editedGesture == null)
            throw new InvalidOperationException("The edited gesture cannot be null.");

        gestureSetupViewModel.SetupCompleted -= OnSetupCompleted;
        gestureSetupViewModel.EditCompleted -= OnEditCompleted;

        var gesture = gestureSetupViewModel.BuildGesture() ?? throw new InvalidOperationException("The gesture cannot be null.");

        var args = new GestureChangedEventArgs(_editedGesture, gesture);

        EditCompleted?.Invoke(this, args);
    }

    #endregion
}