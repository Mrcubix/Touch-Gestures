using System;
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
using TouchGestures.Lib.Interfaces;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

using static AssetLoaderExtensions;

[Name("Swipe"), Icon("Assets/Setups/Swipe/swipe_up.png"),
 Description("A gesture completed by swiping in a specified direction, then releasing the touch point."),
 MultiTouchOnly(false)]
public partial class SwipeSetupViewModel : GestureSetupViewModel, ITouchesCountDependant
{
    private readonly SerializableSwipeGesture _gesture;

    #region Observable Fields

    [ObservableProperty]
    private int _threshold;

    [ObservableProperty]
    private double _deadline;

    [ObservableProperty]
    private int _requiredTouchesCount = 1;

    #endregion

    #region Constructors

    /// Design-time constructor
    public SwipeSetupViewModel() : this(false) { }

    public SwipeSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not SerializableSwipeGesture serializedSwipeGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedSwipeGesture;

        Threshold = (int)serializedSwipeGesture.Threshold.X;
        Deadline = serializedSwipeGesture.Deadline;

        SelectedGestureSetupPickIndex = (int)serializedSwipeGesture.Direction;
        RequiredTouchesCount = serializedSwipeGesture.RequiredTouchesCount;

        BindingDisplay.PluginProperty = serializedSwipeGesture.PluginProperty;

        SetupArea(fullArea, serializedSwipeGesture.Bounds);
    }

    public SwipeSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        SubscribeToSettingsChanges();

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

        RequiredTouchesCount = 1;
        SelectedGestureSetupPickIndex = 0;
        MultitouchSteps = [1];

        BindingDisplay = new BindingDisplayViewModel("Up 1-Touch Swipe", string.Empty, null);
        _gesture = new SerializableSwipeGesture();

        Deadline = 150;
        Threshold = 40;
    }

    #endregion

    #region Methods

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
        _gesture.RequiredTouchesCount = RequiredTouchesCount;

        return _gesture;
    }

    #endregion

    #region Events Handlers

    /// <inheritdoc/>
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SelectedGestureSetupPickIndex) or nameof(RequiredTouchesCount):
                BindingDisplay.Description = $"{GestureSetupPickItems?[SelectedGestureSetupPickIndex]} {RequiredTouchesCount}-Touch Swipe";
                break;
            case nameof(Threshold) or nameof(Deadline):
                AreGestureSettingTweaked = Deadline > 0 && Threshold > 0;
                break;
        }

        base.OnPropertyChanged(sender, e);
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
