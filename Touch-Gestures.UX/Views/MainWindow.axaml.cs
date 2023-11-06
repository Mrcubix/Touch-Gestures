using OpenTabletDriver.External.Avalonia.ViewModels;
using OpenTabletDriver.External.Avalonia.Views;

namespace TouchGestures.UX.Views;

public partial class MainWindow : AppMainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public override void ShowAdvancedBindingEditorDialog(object? sender, BindingDisplayViewModel e)
    {
        throw new System.NotImplementedException();
    }

    public override void ShowBindingEditorDialog(object? sender, BindingDisplayViewModel e)
    {
        throw new System.NotImplementedException();
    }
}