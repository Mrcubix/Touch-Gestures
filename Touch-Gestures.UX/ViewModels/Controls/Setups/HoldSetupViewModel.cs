using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Avalonia;
using Avalonia.Media.Imaging;
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

[Name("Hold"), Icon("Assets/Setups/Hold/hold.png"),
 Description("A gesture completed by holding any specified number of fingers down for a specified amount of time")]
public partial class HoldSetupViewModel : TapSetupViewModel
{
    private readonly SerializableHoldGesture _gesture;

    #region Constructors

    /// Design-time constructor
    public HoldSetupViewModel() : this(false)
    {
        IsOptionsSelectionStepActive = true;
    }

    public HoldSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not SerializableHoldGesture serializedHoldGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedHoldGesture;

        Threshold = (int)serializedHoldGesture.Threshold.X;
        Deadline = serializedHoldGesture.Deadline;

        if (serializedHoldGesture.RequiredTouchesCount > GestureSetupPickItems!.Count)
            throw new IndexOutOfRangeException("Gesture required touches count is greater than the number of available options");

        SelectedGestureSetupPickIndex = serializedHoldGesture.RequiredTouchesCount - 1;

        BindingDisplay.PluginProperty = serializedHoldGesture.PluginProperty;

        SetupArea(fullArea, serializedHoldGesture.Bounds);
    }

    public HoldSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        GestureSetupPickText = "Number of holds:";
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

        // A 1s time threshold to trigger the hold
        Deadline = 1000;

        // Moving outside of the 20px radius of the touch will invalidate the hold
        Threshold = 20;

        BindingDisplay = new BindingDisplayViewModel();
        AreaDisplay = new AreaDisplayViewModel();
        _gesture = new SerializableHoldGesture();

        SubscribeToSettingsChanges();
    }

    #endregion

    #region Methods

    protected override void GoNext()
    {
        if (IsOptionsSelectionStepActive)
        {
            IsOptionsSelectionStepActive = false;
            IsBindingSelectionStepActive = true;

            var value = GestureSetupPickItems?[SelectedGestureSetupPickIndex];

            BindingDisplay.Description = $"{value}-Touch Hold";
        }
        else if (IsBindingSelectionStepActive)
        {
            IsBindingSelectionStepActive = false;
            IsSettingsTweakingStepActive = true;
        }
    }

    /// <inheritdoc/>
    public override Gesture? BuildGesture()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not int option)
            return null;

        _gesture.Deadline = Deadline;
        _gesture.Threshold = new Vector2(Threshold, Threshold);

        _gesture.Bounds = AreaDisplay?.MappedArea.ToSharedArea();
        _gesture.RequiredTouchesCount = option;
        _gesture.PluginProperty = BindingDisplay.PluginProperty;

        return _gesture;
    }

    #endregion

    #region Events Handlers

    /// <inheritdoc/>
    protected override void OnSettingsTweaksChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Deadline) || e.PropertyName == nameof(Threshold))
        {
            AreGestureSettingTweaked = Deadline > 0 && Threshold > 0;
        }
    }

    #endregion
}

public class HoldTileViewModel : GestureTileViewModel<HoldSetupViewModel> { }