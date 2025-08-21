using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.External.Avalonia.ViewModels;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Input;
using TouchGestures.UX.Models;

namespace TouchGestures.UX.ViewModels;

public partial class GestureDebuggerViewModel : NavigableViewModel
{
    #region Observable Properties

    [ObservableProperty]
    private ObservableCollection<TabletGesturesOverview> _overviews = [];

    [ObservableProperty]
    private ObservableCollection<TouchPoint[]> _touchReports = [];

    [ObservableProperty]
    private AreaDisplayViewModel _selectedTabletArea = new();

    [ObservableProperty]
    private TabletGesturesOverview? _selectedTablet;

    [ObservableProperty]
    private Gesture? _selectedGesture;

    #region Gesture States

    [ObservableProperty]
    private bool _hasStarted;

    [ObservableProperty]
    private bool _hasActivated;

    [ObservableProperty]
    private bool _hasEnded;

    [ObservableProperty]
    private bool _hasCompleted;

    #endregion

    #endregion

    #region Constructor

    public GestureDebuggerViewModel()
    {
        PropertyChanged += OnPropertyChanged;
    }

    #endregion

    #region Event Handlers

    public void OnDeviceReport(object? sender, DeviceReportEventArgs e)
    {
        _ = Dispatcher.UIThread.InvokeAsync(async () => await OnDeviceReportAsync(sender, e), DispatcherPriority.Input);
    }

    private Task OnDeviceReportAsync(object? sender, DeviceReportEventArgs e)
    {
        var overview = Overviews.FirstOrDefault(t => t.Name == e.Name);

        if (overview != null && SelectedGesture != null)
        {
            SelectedGesture.OnInput(e.Report.Touches);

            HasStarted = SelectedGesture.HasStarted;
            HasActivated = SelectedGesture.HasActivated;
            HasEnded = SelectedGesture.HasEnded;
            HasCompleted = SelectedGesture.HasCompleted;

            TouchReports.Add(e.Report.Touches);
        }

        return Task.CompletedTask;
    }

    #region Property Changes Handlers

    private void OnTabletChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (SelectedTablet == null)
            return;

        SelectedGesture = SelectedTablet.Gestures.FirstOrDefault()?.AssociatedGesture;
    }

    private void OnGestureChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (SelectedTablet == null || SelectedGesture == null)
            return;

        var x = Math.Round(SelectedTablet.Reference.Size.X, 5);
        var y = Math.Round(SelectedTablet.Reference.Size.Y, 5);

        SelectedTabletArea = BuildArea(new Rect(0, 0, x, y), SelectedGesture.Bounds);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SelectedTablet):
                OnGestureChanged(sender, e);
                break;
            case nameof(SelectedGesture):
                OnGestureChanged(sender, e);
                break;
        }
    }

    #endregion

    #endregion

    #region Static Methods

    /// <summary>
    ///   Convert an area received remotely into a <see cref="AreaDisplayViewModel"/> that we can update in real-time.
    /// </summary>
    public static AreaDisplayViewModel BuildArea(Rect fullArea, SharedArea? mapped = null)
    {
        if (mapped != null && mapped.IsZero() == false)
        {
            // Let's use the area that is provided
            var nativeAreaPosition = mapped.Position;

            var converted = new Area(Math.Round(nativeAreaPosition.X, 5), Math.Round(nativeAreaPosition.Y, 5),
                                     Math.Round(mapped.Width, 5), Math.Round(mapped.Height, 5),
                                     Math.Round(mapped.Rotation, 5), true);

            return new AreaDisplayViewModel(fullArea, converted);
        }
        else
        {
            // We default to the full area
            return new AreaDisplayViewModel(fullArea);
        }
    }

    #endregion
}