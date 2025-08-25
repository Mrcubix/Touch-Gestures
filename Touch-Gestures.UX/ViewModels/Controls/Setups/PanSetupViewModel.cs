using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Media.Imaging;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Enums;
using TouchGestures.UX.Attributes;
using TouchGestures.UX.Extentions;
using Rect = Avalonia.Rect;
using DescriptionAttribute = TouchGestures.UX.Attributes.DescriptionAttribute;
using System.Numerics;
using TouchGestures.UX.ViewModels.Controls.Tiles;
using TouchGestures.Lib.Entities.Gestures;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

using static AssetLoaderExtensions;

[Name("Pan"), Icon("Assets/Setups/Swipe/swipe_up.png"),
 Description("A gesture that can be repeated by swiping in a specified direction, without releasing the touch point."),
 MultiTouchOnly(false)]
public partial class PanSetupViewModel : SwipeSetupViewModel
{
    private readonly PanGesture _gesture;

    #region Constructors

    /// Design-time constructor
    public PanSetupViewModel() : this(false)
    {
    }

    public PanSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        SubscribeToSettingsChanges();

        GestureSetupPickText = "Direction of the Pan:";

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

        BindingDisplay = new BindingDisplayViewModel("Up 1-Touch Pan", string.Empty, null!);
        _gesture = new PanGesture();

        Deadline = 150;
        Threshold = 20;
    }

    public PanSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not PanGesture serializedPanGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedPanGesture;

        Threshold = (int)serializedPanGesture.Threshold.X;
        Deadline = serializedPanGesture.Deadline;

        SelectedGestureSetupPickIndex = (int)serializedPanGesture.Direction;
        RequiredTouchesCount = serializedPanGesture.RequiredTouchesCount;

        BindingDisplay.Store = serializedPanGesture.Store;
        BindingDisplay.Content = serializedPanGesture.Store?.GetHumanReadableString();
        BindingDisplay.Description = gesture.DisplayName;

        AreaDisplay = SetupArea(fullArea, serializedPanGesture.Bounds);
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Gesture? BuildGesture()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not SwipeDirection option)
            return null;

        _gesture.Threshold = new Vector2(Threshold, Threshold);
        _gesture.Bounds = AreaDisplay?.MappedArea.ToSharedArea();
        _gesture.Deadline = Deadline;
        _gesture.Direction = option;
        _gesture.Store = BindingDisplay.Store;
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
                BindingDisplay.Description = $"{GestureSetupPickItems?[SelectedGestureSetupPickIndex]} {RequiredTouchesCount}-Touch Pan";
                break;
            case nameof(Deadline) or nameof(Threshold):
                AreGestureSettingTweaked = Deadline > 0 && Threshold > 0;
                break;
        }

        base.OnPropertyChanged(sender, e);
    }

    #endregion
}

public class PanTileViewModel : GestureTileViewModel<PanSetupViewModel> { }
