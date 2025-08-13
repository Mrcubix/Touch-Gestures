using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Serializables.Gestures;
using TouchGestures.UX.Attributes;
using TouchGestures.UX.Extentions;
using Rect = Avalonia.Rect;
using DescriptionAttribute = TouchGestures.UX.Attributes.DescriptionAttribute;
using TouchGestures.UX.ViewModels.Controls.Tiles;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

using static AssetLoaderExtensions;

[Name("Swipe"), Icon("Assets/Setups/Swipe/swipe_up.png"),
 Description("A gesture completed by swiping in a specified direction, then releasing the touch point."),
 MultiTouchOnly(false)]
public partial class SwipeSetupViewModel : GestureSetupViewModel
{
    private readonly SerializableSwipeGesture _gesture;

    #region Observable Fields

    [ObservableProperty]
    private int _threshold;

    [ObservableProperty]
    private double _deadline;

    #endregion

    #region Constructors

    /// Design-time constructor
    public SwipeSetupViewModel() : this(false)
    {
        IsOptionsSelectionStepActive = true;
    }

    public SwipeSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not SerializableSwipeGesture serializedSwipeGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedSwipeGesture;

        Threshold = (int)serializedSwipeGesture.Threshold.X;
        Deadline = serializedSwipeGesture.Deadline;

        SelectedGestureSetupPickIndex = (int)serializedSwipeGesture.Direction;

        BindingDisplay.PluginProperty = serializedSwipeGesture.PluginProperty;

        SetupArea(fullArea, serializedSwipeGesture.Bounds);
    }

    public SwipeSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        GestureSetupPickText = "Direction of the Swipe:";

        GestureSetupPickItems = new ObservableCollection<object>(Enum.GetValues<SwipeDirection>().Cast<object>());

        Bitmap?[] images = LoadBitmaps(
            "Assets/Setups/Swipe/swipe_up.png",
            "Assets/Setups/Swipe/swipe_down.png",
            "Assets/Setups/Swipe/swipe_left.png",
            "Assets/Setups/Swipe/swipe_right.png",
            "Assets/Setups/Swipe/swipe_up_left.png",
            "Assets/Setups/Swipe/swipe_up_right.png",
            "Assets/Setups/Swipe/swipe_down_left.png",
            "Assets/Setups/Swipe/swipe_down_right.png"
        );

        GestureSetupPickPreviews = new ObservableCollection<Bitmap?>(images);

        SelectedGestureSetupPickIndex = 0;

        BindingDisplay = new BindingDisplayViewModel();
        _gesture = new SerializableSwipeGesture();

        SubscribeToSettingsChanges();

        Deadline = 150;
        Threshold = 40;
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

            BindingDisplay.Description = $"{value} Swipe";
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
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not SwipeDirection option)
            return;

        OnSetupCompleted(this);
    }

    /// <inheritdoc/>
    public override Gesture? BuildGesture()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not SwipeDirection option)
            return null;

        _gesture.Threshold = new Vector2(Threshold, Threshold);
        _gesture.Bounds = AreaDisplay?.MappedArea.ToSharedArea();
        _gesture.Deadline = Deadline;
        _gesture.Direction = option;
        _gesture.PluginProperty = BindingDisplay.PluginProperty;

        return _gesture;
    }

    #endregion

    #region Events Handlers

    /// <inheritdoc/>
    protected override void OnSettingsTweaksChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Threshold) || e.PropertyName == nameof(Deadline))
            AreGestureSettingTweaked = Threshold > 0 && Deadline > 0;
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

public class SwipeTileViewModel : GestureTileViewModel<SwipeSetupViewModel> { }
