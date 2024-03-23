using System;
using System.ComponentModel;
using Avalonia;
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

    private Rect _bounds;

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
        
        PropertyChanging += OnPropertyChanging;
        PropertyChanged += OnGestureChanged;

        GestureSelectionScreenViewModel.BackRequested += OnBackRequestedAhead;
        GestureSelectionScreenViewModel.GestureSelected += OnGestureSelected;

        GestureSetupScreenViewModel.BackRequested += OnBackRequestedAhead;
    }

    public GestureSetupWizardViewModel(Rect bounds) : this()
    {
        _bounds = bounds;
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
            SerializableTapGesture => new TapSetupViewModel(_editedGesture, _bounds),
            SerializableHoldGesture => new HoldSetupViewModel(_editedGesture, _bounds),
            SerializableSwipeGesture => new SwipeSetupViewModel(_editedGesture, _bounds),
            SerializablePanGesture => new PanSetupViewModel(_editedGesture, _bounds),
            _ => new GestureSetupViewModel()
        };

        setupViewModel.BindingDisplay = new(bindingDisplay.Description!, bindingDisplay.Content!, bindingDisplay.PluginProperty);

        // Subscribe to the events
        setupViewModel.EditCompleted += OnEditCompleted;

        GestureSetupScreenViewModel.StartSetup(setupViewModel);
        NextViewModel = GestureSetupScreenViewModel;
    }

    #endregion

    #region Event Handlers

    private void OnGestureSelected(object? sender, GestureTileViewModel selectedTile)
    {
        if (selectedTile == null)
            throw new ArgumentNullException(nameof(selectedTile), "A selected tile cannot be null.");

        var associatedSetup = selectedTile.AssociatedSetup;

        associatedSetup.SetupCompleted += OnSetupCompleted;
        associatedSetup.EditCompleted += OnEditCompleted;

        associatedSetup.AreaDisplay = new(_bounds);

        GestureSetupScreenViewModel.StartSetup(associatedSetup);
        NextViewModel = GestureSetupScreenViewModel;
    }

    private void OnBackRequestedAhead(object? sender, EventArgs e) => GoBack();

    private void OnSetupCompleted(object? sender, EventArgs e)
    {
        if (sender is not GestureSetupViewModel setup)
            throw new InvalidOperationException("The sender must be a GestureSetupViewModel.");

        // Unsubscribe from the events
        setup.SetupCompleted -= OnSetupCompleted;
        setup.EditCompleted -= OnEditCompleted;

        var args = new GestureAddedEventArgs(setup);

        SetupCompleted?.Invoke(this, args);
    } 

    private void OnEditCompleted(object? sender, EventArgs e)
    {
        if (sender is not GestureSetupViewModel setup)
            throw new InvalidOperationException("The sender must be a GestureSetupViewModel.");

        if (_editedGesture == null)
            throw new InvalidOperationException("The edited gesture cannot be null.");

        // Unsubscribe from the events
        setup.SetupCompleted -= OnSetupCompleted;
        setup.EditCompleted -= OnEditCompleted;

        var gesture = setup.BuildGesture() ?? throw new InvalidOperationException("The gesture cannot be null.");

        var args = new GestureChangedEventArgs(_editedGesture, gesture);

        EditCompleted?.Invoke(this, args);
    }

    private void OnPropertyChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName == nameof(NextViewModel) && NextViewModel != null)
            NextViewModel.BackRequested -= OnBackRequestedAhead;
    }

    private void OnGestureChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NextViewModel) && NextViewModel != null)
            NextViewModel.BackRequested += OnBackRequestedAhead;
    }

    #endregion
}