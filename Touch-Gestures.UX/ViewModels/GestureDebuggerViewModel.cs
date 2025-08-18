using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.Plugin.Tablet.Touch;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Input;
using TouchGestures.UX.Extentions;
using TouchGestures.UX.Models;

namespace TouchGestures.UX.ViewModels;

public partial class GestureDebuggerViewModel() : NavigableViewModel
{
    [ObservableProperty]
    private ObservableCollection<TabletGesturesOverview> _overviews = new();

    [ObservableProperty]
    private ObservableCollection<TouchPoint?> _touches = new();

    [ObservableProperty]
    private Gesture? _selectedGesture;

    public void OnDeviceReport(object? sender, DeviceReportEventArgs e)
    {
        _ = Dispatcher.UIThread.InvokeAsync(async () => await OnDeviceReportAsync(sender, e), DispatcherPriority.Input);
    }

    public Task OnDeviceReportAsync(object? sender, DeviceReportEventArgs e)
    {
        var overview = Overviews.FirstOrDefault(t => t.Name == e.Name);

        if (overview != null && SelectedGesture != null)
        {
            SelectedGesture.OnInput(e.Report.Touches);

            if (e.Report.Touches.Length != Touches.Count)
            {
                Touches.Clear();
                Touches.AddRange(e.Report.Touches);
            }
            else
            {
                // Replace data for touches in debugger
                for (int i = 0; i < e.Report.Touches.Length; i++)
                {
                    var reportTouch = e.Report.Touches[i];
                    var debuggerTouch = Touches[i];

                    if (reportTouch == null)
                        Touches[i] = null;
                    else if (debuggerTouch == null)
                        Touches[i] = reportTouch;
                    else 
                    {
                        debuggerTouch.TouchID = reportTouch.TouchID;
                        debuggerTouch.Position = reportTouch.Position;
                    }
                }
            }

            OnPropertyChanged(nameof(Touches));
        }

        return Task.CompletedTask;
    }
}