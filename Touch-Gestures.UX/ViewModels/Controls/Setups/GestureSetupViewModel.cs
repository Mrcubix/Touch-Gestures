using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.External.Avalonia.ViewModels;
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

    #endregion

    #region Constructors

    public GestureSetupViewModel()
    {
        BackRequested = null!;

        CanGoBack = true;
        CanGoNext = false;

        GestureSetupPickText = "Gesture:";
        GestureSetupPickItems = null;

        BindingDisplay = new BindingDisplayViewModel();
    }

    protected virtual void SubscribeToSettingsChanges()
    {
        BindingDisplay.PropertyChanged += OnBindingDisplayPropertyChanged;
    }

    #endregion

    #region Events

    public override event EventHandler? BackRequested;

    public event EventHandler? SetupCompleted;

    public event EventHandler? EditCompleted;

    #endregion

    #region Properties

    public bool CanGoNext { get; init; }

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

    #endregion

    #region Methods

    protected override void GoBack() => BackRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    protected virtual void GoNext() => throw new NotImplementedException("GoNext has not been overriden.");

    [RelayCommand]
    protected virtual void DoComplete() => throw new NotImplementedException("DoComplete has not been overriden.");

    public virtual Gesture? BuildGesture() => throw new NotImplementedException("BuildGesture has not been overriden.");

    #endregion

    #region Event Handlers

    protected virtual void OnSetupCompleted(GestureSetupViewModel sender)
    {
        if (!IsEditing)
            SetupCompleted?.Invoke(sender, EventArgs.Empty);
        else
            EditCompleted?.Invoke(sender, EventArgs.Empty);
    }

    protected virtual void OnBindingDisplayPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BindingDisplayViewModel.PluginProperty))
        {
            IsGestureBindingSet = BindingDisplay.PluginProperty != null;
        }
    }

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