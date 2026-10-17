using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.UX.ViewModels.Controls.Setups;
using TouchGestures.UX.ViewModels.Controls.Tiles;

namespace TouchGestures.UX.ViewModels;

public partial class GestureSetupWizardViewModel : NavigableViewModel
{
    #region Fields

    protected readonly TaskCompletionSource<GestureSetupViewModel> _selectionCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Rect _bounds;
    private readonly bool _isMultiTouch;

    private Gesture _editedGesture = null!;

    #endregion

    #region Observable Fields

    // Step 1 : Select Gesture
    [ObservableProperty]
    private GestureSelectionScreenViewModel _gestureSelectionScreenViewModel;

    [ObservableProperty]
    private GestureSetupScreenViewModel _gestureSetupScreenViewModel = new();

    #endregion

    #region Constructors

    public GestureSetupWizardViewModel(bool isMultiTouch = true)
    {
        _gestureSelectionScreenViewModel = new(isMultiTouch);
        _isMultiTouch = isMultiTouch;

        NextViewModel = _gestureSelectionScreenViewModel;

        PropertyChanging += OnPropertyChanging;
        PropertyChanged += OnGestureChanged;

        GestureSelectionScreenViewModel.BackRequested += OnBackRequestedAhead;
        GestureSelectionScreenViewModel.GestureSelected += OnGestureSelected;

        CanGoBack = true;
    }

    public GestureSetupWizardViewModel(Rect bounds, bool isMultiTouch = true) : this(isMultiTouch)
    {
        _bounds = bounds;
    }

    #endregion

    #region Properties

    public Task<GestureSetupViewModel> SelectionComplete => _selectionCompletionSource.Task;

    #endregion

    #region Methods

    protected override void GoBack()
    {
        if (NextViewModel is GestureSetupScreenViewModel && _editedGesture == null)
            NextViewModel = GestureSelectionScreenViewModel;
        else if (NextViewModel is GestureSelectionScreenViewModel || NextViewModel is GestureSetupScreenViewModel)
            base.GoBack();
    }

    /// <summary>
    ///   Start the gesture setup process.
    /// </summary>
    /// <param name="setupViewModel">The view model to start the setup with.</param>
    public async Task<Gesture?> Start(GestureSetupViewModel setupViewModel)
    {
        NextViewModel = GestureSetupScreenViewModel;
        await GestureSetupScreenViewModel.StartSetup(setupViewModel, _isMultiTouch);

        // Setup might have been cancelled
        if (setupViewModel.Cancel.IsCompleted)
            return null;
        else
            return setupViewModel.BuildGesture() ?? throw new InvalidOperationException("The gesture cannot be null.");
    }

    /// <summary>
    ///   Start editing a specified gesture.
    /// </summary>
    /// <param name="bindingDisplay">The binding display to edit.</param>
    /// <exception cref="ArgumentNullException"/>
    public async Task<Gesture?> Edit(GestureBindingDisplayViewModel bindingDisplay)
    {
        if (bindingDisplay == null)
            throw new ArgumentNullException(nameof(bindingDisplay), "A binding display cannot be null.");

        _editedGesture = bindingDisplay.AssociatedGesture;

        // now we need to generate the correct setup view model
        GestureSetupViewModel setupViewModel = _editedGesture switch
        {
            HoldGesture => new HoldSetupViewModel(_editedGesture, _bounds),
            TapGesture => new TapSetupViewModel(_editedGesture, _bounds),
            PanGesture => new PanSetupViewModel(_editedGesture, _bounds),
            SwipeGesture => new SwipeSetupViewModel(_editedGesture, _bounds),
            PinchGesture pinchGesture => DifferentiatePinchFromRotation(pinchGesture, _bounds),

            _ => new GestureSetupViewModel()
        };

        return await Start(setupViewModel);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    ///   Handle the event when a gesture is selected on the <see cref="GestureSelectionScreen"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="selectedTile">The selected tile.</param>
    /// <exception cref="ArgumentNullException"/>
    private void OnGestureSelected(object? sender, GestureTileViewModel selectedTile)
    {
        if (selectedTile == null)
            throw new ArgumentNullException(nameof(selectedTile), "A selected tile cannot be null.");

        var associatedSetup = selectedTile.AssociatedSetup;

        associatedSetup.AreaDisplay = new(_bounds);

        _selectionCompletionSource.TrySetResult(associatedSetup);
    }

    private void OnBackRequestedAhead(object? sender, EventArgs e) => GoBack();

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

    #region static Methods

    public static GestureSetupViewModel DifferentiatePinchFromRotation(PinchGesture gesture, Rect bounds)
    {
        return gesture.DistanceThreshold > 0
            ? new PinchSetupViewModel(gesture, bounds)
            : new RotateSetupViewModel(gesture, bounds);
    }

    #endregion
}