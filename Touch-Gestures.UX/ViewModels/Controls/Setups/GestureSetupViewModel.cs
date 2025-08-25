using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures.Bases;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

public partial class GestureSetupViewModel : NavigableViewModel, IDisposable
{
    #region Constants

    private static readonly Uri _placeholderImageUri = new("avares://Touch-Gestures.UX/Assets/Displays/placeholder.png");
    private static readonly Bitmap _placeholderImage = new(AssetLoader.Open(_placeholderImageUri));

    protected readonly TaskCompletionSource _completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    protected readonly TaskCompletionSource _cancelSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    #endregion

    #region Fields

    private int _skippedSteps = 0;

    #endregion

    #region Observable Fields

    [ObservableProperty]
    private string _gestureSetupPickText = string.Empty;

    [ObservableProperty]
    private string _backButtonText = "Cancel";

    /// <summary>
    ///   The index of the selected option, usually modified during the first step.
    /// </summary>
    [ObservableProperty]
    private int _selectedGestureSetupPickIndex = -1;

    // Setting tweaking checks

    [ObservableProperty]
    private bool _isGestureBindingSet = false;

    [ObservableProperty]
    private bool _areGestureSettingTweaked = false;

    // Steps

    [ObservableProperty]
    private int _currentStep = 0;

    // Editing

    [ObservableProperty]
    private bool _isEditing;

    // Previews

    [ObservableProperty]
    private Bitmap? _selectedSetupPickPreview = _placeholderImage;

    [ObservableProperty]
    private ObservableCollection<object>? _gestureSetupPickItems;

    [ObservableProperty]
    private ObservableCollection<Bitmap?>? _gestureSetupPickPreviews;

    // Binding & Area Displays

    [ObservableProperty]
    private BindingDisplayViewModel _bindingDisplay;

    [ObservableProperty]
    private AreaDisplayViewModel? _areaDisplay;

    #endregion

    #region Constructors

    public GestureSetupViewModel()
    {
        CanGoBack = true;
        CanGoNext = false;

        GestureSetupPickText = "Gesture:";
        GestureSetupPickItems = null;

        BindingDisplay = new BindingDisplayViewModel();
        AreaDisplay = new AreaDisplayViewModel();
    }

    protected virtual void SubscribeToSettingsChanges()
    {
        PropertyChanging += OnPropertyChanging;
        PropertyChanged += OnPropertyChanged;
        BindingDisplay.PropertyChanged += OnBindingDisplayPropertyChanged;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   The task that is marked as completed when the gesture setup is complete.
    /// </summary>
    public Task Complete => _completionSource.Task;

    /// <summary>
    ///   The task that is marked as completed when the gesture setup is cancelled.
    /// </summary>
    public Task Cancel => _cancelSource.Task;

    /// <summary>
    ///   Whether the user can go to the next step.
    /// </summary>
    public bool CanGoNext { get; init; }

    /// <summary>
    ///   Whether this setup is for a multi-touch gesture or not.
    /// </summary>
    public bool IsMultiTouchSetup { get; set; } = true;

    /// <summary>
    ///   The steps that should be skipped when the device is single-touch.
    /// </summary>
    public IEnumerable<int> MultitouchSteps { get; protected set; } = [];

    #endregion

    #region Methods

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    protected virtual void GoNext()
    {
        CurrentStep++;

        // Skip multi-touch steps if the device is single-touch
        if (IsMultiTouchSetup == false && MultitouchSteps.Contains(CurrentStep))
        {
            _skippedSteps++;
            GoNext();
        }

        if (CurrentStep - _skippedSteps > 0)
            BackButtonText = "Back";
    }

    protected override void GoBack()
    {
        CurrentStep--;

        // Skip multi-touch steps if the device is single-touch
        if (IsMultiTouchSetup == false && MultitouchSteps.Contains(CurrentStep))
        {
            _skippedSteps--;
            GoBack();
        }

        // Next step will be below 0, so exit the setup
        if (CurrentStep == -1)
        {
            _cancelSource.TrySetResult();

            CurrentStep = 0;
            base.GoBack();
        }

        // Back Button Text should be cancel if we skipped the first steps
        if (CurrentStep - _skippedSteps < 1)
            BackButtonText = "Cancel";
    }

    /// <summary>
    ///   Complete the gesture setup process.
    /// </summary>
    [RelayCommand]
    protected virtual void DoComplete() => throw new NotImplementedException("DoComplete has not been overriden.");

    /// <summary>
    ///   Build the gesture based on the setup.
    /// </summary>
    /// <returns>The gesture that was built.</returns>
    public virtual Gesture? BuildGesture() => throw new NotImplementedException("BuildGesture has not been overriden.");

    #endregion

    #region Event Handlers

    /// <summary>
    ///   Handle the event when the binding display property changes. <br/>
    ///   If not done, it will prevent the user from proceeding to the next step.
    /// </summary>
    protected virtual void OnBindingDisplayPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BindingDisplayViewModel.Store))
            IsGestureBindingSet = BindingDisplay.Store != null;
    }

    protected virtual void OnPropertyChanging(object? sender, PropertyChangingEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(BindingDisplay) when BindingDisplay != null:
                BindingDisplay.PropertyChanged -= OnBindingDisplayPropertyChanged;
                break;
        }
    }

    protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SelectedGestureSetupPickIndex):
                SelectedSetupPickPreview = GestureSetupPickPreviews?.ElementAtOrDefault(SelectedGestureSetupPickIndex)
                ?? _placeholderImage;
                break;
            case nameof(BindingDisplay) when BindingDisplay != null:
                BindingDisplay.PropertyChanged += OnBindingDisplayPropertyChanged;
                break;
        }
    }

    #endregion

    #region Interface Implementations

    public virtual void Dispose()
    {
        _placeholderImage.Dispose();
    }

    #endregion

    #region Static Methods

    /// <summary>
    ///   Convert an area received remotely into a <see cref="AreaDisplayViewModel"/> that we can update in real-time.
    /// </summary>
    public static AreaDisplayViewModel SetupArea(Rect fullArea, SharedArea? mapped = null)
    {
        if (mapped != null && mapped.IsZero() == false)
        {
            // Let's use the area that is provided
            var nativeAreaPosition = mapped.Position;

            var converted = new Area(Math.Round(nativeAreaPosition.X, 5), Math.Round(nativeAreaPosition.Y, 5),
                                     Math.Round(mapped.Width, 5), Math.Round(mapped.Height, 5),
                                     Math.Round(mapped.Rotation, 5), true);

            return new AreaDisplayViewModel(fullArea, converted);
        }
        else
        {
            // We default to the full area
            return new AreaDisplayViewModel(fullArea);
        }
    }

    #endregion
}