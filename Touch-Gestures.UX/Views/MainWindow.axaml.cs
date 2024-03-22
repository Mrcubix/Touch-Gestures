using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using OpenTabletDriver.External.Avalonia.Dialogs;
using OpenTabletDriver.External.Avalonia.ViewModels;
using OpenTabletDriver.External.Avalonia.Views;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.UX.ViewModels;

namespace TouchGestures.UX.Views;

public partial class MainWindow : AppMainWindow
{
    private static readonly BindingEditorDialogViewModel _bindingEditorDialogViewModel = new();
    private static readonly AdvancedBindingEditorDialogViewModel _advancedBindingEditorDialogViewModel = new();
    private static bool _isEditorDialogOpen = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    public override void ShowBindingEditorDialog(object? sender, BindingDisplayViewModel e)
    {
        _ = Dispatcher.UIThread.InvokeAsync(() => ShowBindingEditorDialogCore(e));
    }

    public override void ShowAdvancedBindingEditorDialog(object? sender, BindingDisplayViewModel e)
    {
        _ = Dispatcher.UIThread.InvokeAsync(() => ShowAdvancedBindingEditorDialogCore(e));
    }

    private async Task ShowBindingEditorDialogCore(BindingDisplayViewModel e)
    {
        if (DataContext is MainViewModel vm && !_isEditorDialogOpen)
        {
            _isEditorDialogOpen = true;

            // Now we setup the dialog

            var dialog = new BindingEditorDialog()
            {
                Plugins = vm.Plugins,
                DataContext = _bindingEditorDialogViewModel
            };

            // Now we show the dialog

            var res = await dialog.ShowDialog<SerializablePluginSettings>(this);

            HandleBindingEditorResult(res, e);
        }
    }

    private async Task ShowAdvancedBindingEditorDialogCore(BindingDisplayViewModel e)
    {
        if (DataContext is MainViewModel vm && !_isEditorDialogOpen)
        {
            _isEditorDialogOpen = true;

            // Fetch some data from the plugins

            var types = vm.Plugins.Select(p => p.PluginName ?? p.FullName ?? "Unknown").ToList();

            var currentPlugin = vm.Plugins.FirstOrDefault(p => p.Identifier == e.PluginProperty?.Identifier);
            var selectedType = currentPlugin?.PluginName ?? currentPlugin?.FullName ?? "Unknown";

            var validProperties = currentPlugin?.ValidProperties ?? new string[0];
            var selectedProperty = e.PluginProperty?.Value ?? "";

            // Now we set the view model's properties

            _advancedBindingEditorDialogViewModel.Types = new ObservableCollection<string>(types);
            _advancedBindingEditorDialogViewModel.SelectedType = selectedType;
            _advancedBindingEditorDialogViewModel.ValidProperties = new ObservableCollection<string>(validProperties);
            _advancedBindingEditorDialogViewModel.SelectedProperty = selectedProperty;

            // Now we setup the dialog

            var dialog = new AdvancedBindingEditorDialog()
            {
                DataContext = _advancedBindingEditorDialogViewModel,
                Plugins = vm.Plugins
            };

            // Now we show the dialog

            var res = await dialog.ShowDialog<SerializablePluginSettings>(this);

            HandleBindingEditorResult(res, e);
        }
    }

    private void HandleBindingEditorResult(SerializablePluginSettings result, BindingDisplayViewModel e)
    {
        if (DataContext is MainViewModel vm)
        {
            _isEditorDialogOpen = false;

            // We handle the result

            // The dialog was closed or the cancel button was pressed
            if (result == null)
                return;

            // The user selected "Clear"
            if (result.Identifier == -1 || result.Value == "None")
            {
                e.PluginProperty = null;
                e.Content = "";
            }
            else
            {
                e.PluginProperty = result;
                e.Content = vm.GetFriendlyContentFromProperty(result);
            }
        }
    }
}