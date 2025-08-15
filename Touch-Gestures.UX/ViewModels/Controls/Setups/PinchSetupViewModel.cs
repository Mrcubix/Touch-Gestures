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

[Name("Pinch"), Icon("Assets/Setups/Pinch/pinch_inner.png"),
 Description("A gesture completed by pinching, simillar to how you would zoom in, in various application"),
 MultiTouchOnly(true)]
public partial class PinchSetupViewModel : GestureSetupViewModel
{
    private readonly SerializablePinchGesture _gesture;

    #region Observable Fields

    [ObservableProperty]
    protected double _distanceThreshold;

    [ObservableProperty]
    protected bool _isInner;

    public override bool SingleTouchSupported { get; } = false;

    #endregion

    #region Constructors

    /// Design-time constructor
    public PinchSetupViewModel() : this(false) 
    { 
        IsOptionsSelectionStepActive = true;
    }

    public PinchSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        GestureSetupPickText = "Direction of pinch:";
        GestureSetupPickItems = new ObservableCollection<object>(new string[] { "Inner", "Outer" });

        Bitmap?[] images = LoadBitmaps(
            "Assets/Setups/Pinch/pinch_inner.png",
            "Assets/Setups/Pinch/pinch_outer.png"
        );

        GestureSetupPickPreviews = new ObservableCollection<Bitmap?>(images);

        SelectedGestureSetupPickIndex = 0;

        BindingDisplay = new BindingDisplayViewModel();
        AreaDisplay = new AreaDisplayViewModel();
        _gesture = new SerializablePinchGesture();

        SubscribeToSettingsChanges();

        DistanceThreshold = 20;
        IsInner = false;
    }

    /// Constructor used when editing a gesture
    public PinchSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not SerializablePinchGesture serializedTapGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedTapGesture;

        DistanceThreshold = serializedTapGesture.DistanceThreshold;
        IsInner = serializedTapGesture.IsInner;

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

            BindingDisplay.Description = $"{value} Pinch";
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

        _gesture.DistanceThreshold = DistanceThreshold;
        _gesture.IsInner = option == "Inner";

        _gesture.Bounds = AreaDisplay?.MappedArea.ToSharedArea();
        _gesture.PluginProperty = BindingDisplay.PluginProperty;

        return _gesture;
    }

    #endregion

    #region Events Handlers

    /// <inheritdoc/>
    protected override void OnSettingsTweaksChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DistanceThreshold))
            AreGestureSettingTweaked = DistanceThreshold > 0;
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

public class PinchTileViewModel : GestureTileViewModel<PinchSetupViewModel> {}