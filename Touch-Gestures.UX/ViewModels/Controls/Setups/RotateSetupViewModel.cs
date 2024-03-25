using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Serializables.Gestures;
using TouchGestures.UX.Attributes;
using TouchGestures.UX.Extentions;
using TouchGestures.UX.ViewModels.Controls.Tiles;
using DescriptionAttribute = TouchGestures.UX.Attributes.DescriptionAttribute;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

using static AssetLoaderExtensions;

#nullable enable

[Name("Rotation"), Icon("Assets/Setups/Rotation/rotation_clockwise.png"),
 Description("A gesture completed by pinching & rotating 2 fingers, simillar to how you would rotate a map in various application")]
public partial class RotateSetupViewModel : PinchSetupViewModel
{
    private readonly SerializablePinchGesture _gesture;

    #region Observable Fields

    [ObservableProperty]
    protected double _angleThreshold;

    [ObservableProperty]
    protected bool _isClockwise;

    #endregion

    #region Constructors

    /// Design-time constructor
    public RotateSetupViewModel() : this(false) 
    { 
        IsOptionsSelectionStepActive = true;
    }

    public RotateSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        GestureSetupPickText = "Direction the rotation:";
        GestureSetupPickItems = new ObservableCollection<object>(new string[] { "Clockwise", "Counter-Clockwise" });

        Bitmap?[] images = LoadBitmaps(
            "Assets/Setups/Rotation/rotation_clockwise.png",
            "Assets/Setups/Rotation/rotation_anti-clockwise.png"
        );

        GestureSetupPickPreviews = new ObservableCollection<Bitmap?>(images);

        SelectedGestureSetupPickIndex = 0;

        // A 80ms deadline is the minimum required for taps for work properly and about consistently
        //Deadline = 80;

        AngleThreshold = 20;
        IsClockwise = false;
        AreGestureSettingTweaked = AngleThreshold > 0;

        BindingDisplay = new BindingDisplayViewModel();
        AreaDisplay = new AreaDisplayViewModel();
        _gesture = new SerializablePinchGesture();

        SubscribeToSettingsChanges();
    }

    /// Constructor used when editing a gesture
    public RotateSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not SerializablePinchGesture serializedTapGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedTapGesture;

        AngleThreshold = serializedTapGesture.AngleThreshold;
        IsClockwise = serializedTapGesture.IsClockwise;
        AreGestureSettingTweaked = AngleThreshold > 0;

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

            BindingDisplay.Description = $"{value} Rotation";
        }
        else if (IsBindingSelectionStepActive)
        {
            IsBindingSelectionStepActive = false;
            IsSettingsTweakingStepActive = true;
        }
    }

    /// <inheritdoc/>
    protected override void DoComplete()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not string option)
            return;

        OnSetupCompleted(this);
    }

    /// <inheritdoc/>
    public override Gesture? BuildGesture()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not string option)
            return null;

        _gesture.AngleThreshold = AngleThreshold;
        _gesture.IsClockwise = option == "Clockwise";

        _gesture.Bounds = AreaDisplay?.MappedArea.ToSharedArea();
        _gesture.PluginProperty = BindingDisplay.PluginProperty;

        return _gesture;
    }

    #endregion

    #region Events Handlers

    /// <inheritdoc/>
    protected override void OnSettingsTweaksChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DistanceThreshold) || e.PropertyName == nameof(IsInner))
        {
            AreGestureSettingTweaked = DistanceThreshold > 0;
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

public class RotateTileViewModel : GestureTileViewModel<RotateSetupViewModel> {}