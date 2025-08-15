using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures.Bases;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

#nullable enable

public partial class GestureSetupViewModel : NavigableViewModel, IDisposable
{
    #region Constants

    private static readonly Uri _placeholderImageUri = new("avares://Touch-Gestures.UX/Assets/Displays/placeholder.png");
    private static readonly Bitmap _placeholderImage = new(AssetLoader.Open(_placeholderImageUri));

    #endregion

    #region Observable Fields

    [ObservableProperty]
    private string _gestureSetupPickText = string.Empty;

    private int _selectedGestureSetupPickIndex = 0;

    // Setting tweaking checks

    [ObservableProperty]
    private bool _isGestureBindingSet = false;

    [ObservableProperty]
    private bool _areGestureSettingTweaked = false;

    // Steps

    [ObservableProperty]
    private bool _isOptionsSelectionStepActive = false;

    [ObservableProperty]
    private bool _isBindingSelectionStepActive = false;

    [ObservableProperty]
    private bool _isSettingsTweakingStepActive = false;

    // Editing

    [ObservableProperty]
    private bool _isEditing;

    // Visual Parts

    [ObservableProperty]
    private Bitmap? _selectedSetupPickPreview = _placeholderImage;

    [ObservableProperty]
    private ObservableCollection<object>? _gestureSetupPickItems;

    [ObservableProperty]
    private ObservableCollection<Bitmap?>? _gestureSetupPickPreviews;

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
        BindingDisplay.PropertyChanged += OnBindingDisplayPropertyChanged;
    }

    #endregion

    #region Events

    public event EventHandler? SetupCompleted;

    public event EventHandler? EditCompleted;

    #endregion

    #region Properties

    public bool CanGoNext { get; init; }

    /// <summary>
    ///   The index of the selected optino on the first step.
    /// </summary>
    public int SelectedGestureSetupPickIndex
    {
        get => _selectedGestureSetupPickIndex;
        set
        {
            SetProperty(ref _selectedGestureSetupPickIndex, value);

            if (value >= 0 && value < GestureSetupPickPreviews!.Count)
                SelectedSetupPickPreview = GestureSetupPickPreviews[value] ?? _placeholderImage;
            else
                SelectedSetupPickPreview = _placeholderImage;
        }
    }

    /// <summary>
    ///   Whether single touch gestures are supported for this setup.
    /// </summary>
    public virtual bool SingleTouchSupported { get; } = true;

    /// <summary>
    ///   Whether option selection is enabled for this setup when not multi-touch.
    /// </summary>
    public virtual bool SingleTouchOptionSelectionEnabled { get; } = true;

    /// <summary>
    ///   Whether this setup is for a multi-touch gesture or not.
    /// </summary>
    public bool IsMultiTouchSetup { get; set; } = true;

    #endregion

    #region Methods

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    protected virtual void GoNext() => throw new NotImplementedException("GoNext has not been overriden.");

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

    /// <summary>
    ///   Convert an area received remotely into a <see cref="AreaDisplayViewModel"/> that we can update in real-time.
    /// </summary>
    public virtual void SetupArea(Rect fullArea, SharedArea? mapped = null)
    {
        if (mapped != null && mapped.IsZero() == false)
        {
            // Let's use the area that is provided
            var nativeAreaPosition = mapped.Position;

            var converted = new Area(Math.Round(nativeAreaPosition.X, 5), Math.Round(nativeAreaPosition.Y, 5),
                                     Math.Round(mapped.Width, 5), Math.Round(mapped.Height, 5),
                                     Math.Round(mapped.Rotation, 5), true);

            AreaDisplay = new AreaDisplayViewModel(fullArea, converted);
        }
        else
        {
            // We default to the full area
            AreaDisplay = new AreaDisplayViewModel(fullArea);
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    ///   Handle setup completion.
    /// </summary>
    /// <remarks>
    ///   A different event will be invoked depending on whether the setup is being edited or not.
    /// </remarks>
    /// <param name="sender"></param>
    protected virtual void OnSetupCompleted(GestureSetupViewModel sender)
    {
        if (!IsEditing)
            SetupCompleted?.Invoke(sender, EventArgs.Empty);
        else
            EditCompleted?.Invoke(sender, EventArgs.Empty);
    }

    /// <summary>
    ///   Handle the event when the binding display property changes. <br/>
    ///   If not done, it will prevent the user from proceeding to the next step.
    /// </summary>
    protected virtual void OnBindingDisplayPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BindingDisplayViewModel.PluginProperty))
        {
            IsGestureBindingSet = BindingDisplay.PluginProperty != null;
        }
    }

    /// <summary>
    ///   Handle the event when the settings are being tweaked. <br/>
    ///   Setups need to make use of this to enable the user to proceed to the next step or complete the setup.
    /// </summary>
    protected virtual void OnSettingsTweaksChanged(object? sender, PropertyChangedEventArgs e)
    {
        throw new NotImplementedException("OnSettingsTweakingCompleted has not been overriden.");
    }

    #endregion

    #region Interface Implementations

    public virtual void Dispose()
    {
        _placeholderImage.Dispose();
    }

    #endregion
}