using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Extensions;
using TouchGestures.Lib.Serializables.Gestures;
using TouchGestures.UX.Extentions;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

using static AssetLoaderExtensions;

#nullable enable

public partial class TapSetupViewModel : GestureSetupViewModel
{
    private readonly SerializableTapGesture _gesture;

    #region Observable Fields

    [ObservableProperty]
    private int _threshold;

    [ObservableProperty]
    private double _deadline;

    #endregion

    #region Constructors

    /// Design-time constructor
    public TapSetupViewModel() : this(false) 
    { 
        IsOptionsSelectionStepActive = true;
    }

    public TapSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        GestureSetupPickText = "Number of touches:";
        GestureSetupPickItems = new ObservableCollection<object>(Enumerable.Range(1, 10).Cast<object>());

        Bitmap?[] images = LoadBitmaps(
            "Assets/Setups/Tap/tap_single.png",
            "Assets/Setups/Tap/tap_double.png",
            "Assets/Setups/Tap/tap_triple.png",
            "Assets/Setups/Tap/tap_quadruple.png",
            "Assets/Setups/Tap/tap_quintuple.png",
            "Assets/Setups/Tap/tap_sextuple.png",
            "Assets/Setups/Tap/tap_septuple.png",
            "Assets/Setups/Tap/tap_octuple.png",
            "Assets/Setups/Tap/tap_nonuple.png",
            "Assets/Setups/Tap/tap_décuple.png"
        );

        GestureSetupPickPreviews = new ObservableCollection<Bitmap?>(images);

        SelectedGestureSetupPickIndex = 0;

        // A 80ms deadline is the minimum required for taps for work properly and about consistently
        Deadline = 80;

        BindingDisplay = new BindingDisplayViewModel();
        AreaDisplay = new AreaDisplayViewModel();
        _gesture = new SerializableTapGesture();

        SubscribeToSettingsChanges();
    }

    public TapSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not SerializableTapGesture serializedTapGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedTapGesture;

        //Threshold = (int)serializedTapGesture.Threshold.X;
        Deadline = serializedTapGesture.Deadline;

        if (serializedTapGesture.RequiredTouchesCount > GestureSetupPickItems!.Count)
            throw new IndexOutOfRangeException("Gesture required touches count is greater than the number of available options");

        SelectedGestureSetupPickIndex = serializedTapGesture.RequiredTouchesCount - 1;

        BindingDisplay.PluginProperty = serializedTapGesture.PluginProperty;

        SetupArea(fullArea, serializedTapGesture.Bounds);
    }

    protected override void SubscribeToSettingsChanges()
    {
        PropertyChanged += OnSettingsTweaksChanged;

        base.SubscribeToSettingsChanges();
    }

    #endregion

    #region Methods

    protected override void GoBack()
    {
        if (IsBindingSelectionStepActive) // Step 2
        {
            IsBindingSelectionStepActive = false;
            IsOptionsSelectionStepActive = true;
        }
        else if (IsSettingsTweakingStepActive) // Step 3
        {
            IsSettingsTweakingStepActive = false;
            IsBindingSelectionStepActive = true;
        }
        else // Step 1
            base.GoBack();
    }

    protected override void GoNext()
    {
        if (IsOptionsSelectionStepActive)
        {
            IsOptionsSelectionStepActive = false;
            IsBindingSelectionStepActive = true;

            var value = GestureSetupPickItems?[SelectedGestureSetupPickIndex];

            BindingDisplay.Description = $"{value}-Touch Tap";
        }
        else if (IsBindingSelectionStepActive)
        {
            IsBindingSelectionStepActive = false;
            IsSettingsTweakingStepActive = true;
        }
    }

    protected override void DoComplete()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not int option)
            return;

        OnSetupCompleted(this);
    }

    public override Gesture? BuildGesture()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not int option)
            return null;

        //_gesture.Threshold = new Vector2(Threshold, Threshold);
        _gesture.Bounds = AreaDisplay?.MappedArea.ToSharedArea();
        _gesture.Deadline = Deadline;
        _gesture.RequiredTouchesCount = option;
        _gesture.PluginProperty = BindingDisplay.PluginProperty;

        return _gesture;
    }

    #endregion

    #region Events Handlers

    protected override void OnSettingsTweaksChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Deadline))
        {
            AreGestureSettingTweaked = Deadline > 0;
        }
    }

    #endregion

    #region Interface Implementations

    public override void Dispose()
    {
        base.Dispose();

        if (GestureSetupPickPreviews != null)
            foreach (var bitmap in GestureSetupPickPreviews)
                bitmap?.Dispose();
    }

    #endregion
}
