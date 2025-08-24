using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

[Name("Tap"), Icon("Assets/Setups/Tap/tap_triple.png"),
 Description("A gesture completed by tapping with any specified number of fingers"),
 MultiTouchOnly(false)]
public partial class TapSetupViewModel : GestureSetupViewModel
{
    private readonly TapGesture _gesture;

    #region Observable Fields

    [ObservableProperty]
    protected int _threshold;

    [ObservableProperty]
    protected double _deadline;

    #endregion

    #region Constructors

    /// Design-time constructor
    public TapSetupViewModel() : this(false) { }

    public TapSetupViewModel(bool isEditing = false)
    {
        IsEditing = isEditing;

        CanGoBack = true;
        CanGoNext = true;

        SubscribeToSettingsChanges();

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
        MultitouchSteps = [0];

        BindingDisplay = new BindingDisplayViewModel("1-Touch Tap", string.Empty, null!);
        AreaDisplay = new AreaDisplayViewModel();
        _gesture = new TapGesture();

        // A 80ms deadline is the minimum required for taps for work properly and about consistently
        Deadline = 80;
    }

    public TapSetupViewModel(Gesture gesture, Rect fullArea) : this(true)
    {
        if (gesture is not TapGesture serializedTapGesture)
            throw new ArgumentException("Gesture is not a SerializableTapGesture", nameof(gesture));

        _gesture = serializedTapGesture;

        //Threshold = (int)serializedTapGesture.Threshold.X;
        Deadline = serializedTapGesture.Deadline;

        if (serializedTapGesture.RequiredTouchesCount > GestureSetupPickItems!.Count)
            throw new IndexOutOfRangeException("Gesture required touches count is greater than the number of available options");

        SelectedGestureSetupPickIndex = serializedTapGesture.RequiredTouchesCount - 1;

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
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not int option)
            return;

        _completionSource.TrySetResult();
    }

    /// <inheritdoc/>
    public override Gesture? BuildGesture()
    {
        if (GestureSetupPickItems?[SelectedGestureSetupPickIndex] is not int option)
            return null;

        //_gesture.Threshold = new Vector2(Threshold, Threshold);
        _gesture.Bounds = AreaDisplay?.MappedArea.ToSharedArea();
        _gesture.Deadline = Deadline;
        _gesture.RequiredTouchesCount = option;
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
                BindingDisplay.Description = $"{GestureSetupPickItems?[SelectedGestureSetupPickIndex]}-Touch Tap";
                break;
            case nameof(Deadline):
                AreGestureSettingTweaked = Deadline > 0;
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

public class TapTileViewModel : GestureTileViewModel<TapSetupViewModel> { }