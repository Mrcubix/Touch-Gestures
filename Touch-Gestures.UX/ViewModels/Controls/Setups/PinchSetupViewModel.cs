using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.UX.Attributes;
using TouchGestures.UX.Extentions;
using TouchGestures.UX.ViewModels.Controls.Tiles;
using DescriptionAttribute = TouchGestures.UX.Attributes.DescriptionAttribute;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

using static AssetLoaderExtensions;

[Name("Pinch"), Icon("Assets/Setups/Pinch/pinch_inner.png"),
 Description("A gesture completed by pinching, simillar to how you would zoom in, in various application"),
 MultiTouchOnly(true)]
public partial class PinchSetupViewModel : GestureSetupViewModel
{
    private readonly PinchGesture _gesture;

    #region Observable Fields

    [ObservableProperty]
    protected double _distanceThreshold;

    [ObservableProperty]
    protected bool _isInner;

    #endregion

    #region Constructors

    /// Design-time constructor
    public PinchSetupViewModel() : this(false) { }

    public PinchSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        SubscribeToSettingsChanges();

        GestureSetupPickText = "Direction of pinch:";
        GestureSetupPickItems = new ObservableCollection<object>(["Inner", "Outer"]);

        Bitmap?[] images = LoadBitmaps(
            "Assets/Setups/Pinch/pinch_inner.png",
            "Assets/Setups/Pinch/pinch_outer.png"
        );

        GestureSetupPickPreviews = new ObservableCollection<Bitmap?>(images);

        SelectedGestureSetupPickIndex = 0;

        BindingDisplay = new BindingDisplayViewModel("Inner Pinch", string.Empty, null!);
        AreaDisplay = new AreaDisplayViewModel();
        _gesture = new PinchGesture();

        DistanceThreshold = 20;
        IsInner = false;
    }

    /// Constructor used when editing a gesture
    public PinchSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not PinchGesture serializedTapGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedTapGesture;

        DistanceThreshold = serializedTapGesture.DistanceThreshold;
        IsInner = serializedTapGesture.IsInner;

        BindingDisplay.Store = serializedTapGesture.Store;
        BindingDisplay.Content = serializedTapGesture.Store?.GetHumanReadableString();
        BindingDisplay.Description = gesture.DisplayName;

        AreaDisplay = SetupArea(fullArea, serializedTapGesture.Bounds);
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void DoComplete()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not string option)
            return;

        _completionSource.TrySetResult();
    }

    /// <inheritdoc/>
    public override Gesture? BuildGesture()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not string option)
            return null;

        _gesture.DistanceThreshold = DistanceThreshold;
        _gesture.IsInner = option == "Inner";

        _gesture.Bounds = AreaDisplay?.MappedArea.ToSharedArea();
        _gesture.Store = BindingDisplay.Store;

        return _gesture;
    }

    #endregion

    #region Events Handlers

    /// <inheritdoc/>
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SelectedGestureSetupPickIndex):
                BindingDisplay.Description = $"{GestureSetupPickItems?[SelectedGestureSetupPickIndex]} Pinch";
                break;
            case nameof(DistanceThreshold):
                AreGestureSettingTweaked = DistanceThreshold > 0;
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

public class PinchTileViewModel : GestureTileViewModel<PinchSetupViewModel> { }