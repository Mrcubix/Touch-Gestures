using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Media.Imaging;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Enums;
using TouchGestures.Lib.Serializables.Gestures;
using TouchGestures.UX.Attributes;
using TouchGestures.UX.Extentions;
using Rect = Avalonia.Rect;
using DescriptionAttribute = TouchGestures.UX.Attributes.DescriptionAttribute;
using System.Numerics;
using TouchGestures.UX.ViewModels.Controls.Tiles;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

using static AssetLoaderExtensions;


[Name("Pan"), Icon("Assets/Setups/Swipe/swipe_up.png"),
 Description("A gesture that can be repeated by swiping in a specified direction, without releasing the touch point.")]
public partial class PanSetupViewModel : SwipeSetupViewModel
{
    private readonly SerializablePanGesture _gesture;

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

        SelectedGestureSetupPickIndex = 0;

        BindingDisplay = new BindingDisplayViewModel();
        _gesture = new SerializablePanGesture();

        SubscribeToSettingsChanges();
    }

    public PanSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not SerializablePanGesture serializedPanGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedPanGesture;

        Threshold = (int)serializedPanGesture.Threshold.X;
        Deadline = serializedPanGesture.Deadline;

        SelectedGestureSetupPickIndex = (int)serializedPanGesture.Direction;

        BindingDisplay.PluginProperty = serializedPanGesture.PluginProperty;

        SetupArea(fullArea, serializedPanGesture.Bounds);
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

            BindingDisplay.Description = $"{value} Pan";
        }
        else if (IsBindingSelectionStepActive)
        {
            IsBindingSelectionStepActive = false;
            IsSettingsTweakingStepActive = true;
        }
    }

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
}

public class PanTileViewModel : GestureTileViewModel<PanSetupViewModel> {}
