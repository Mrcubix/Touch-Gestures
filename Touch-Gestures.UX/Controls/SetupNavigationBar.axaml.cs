using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace TouchGestures.UX.Controls;

#nullable enable

public partial class SetupNavigationBar : TemplatedControl
{
    public static readonly StyledProperty<string> NextButtonTextProperty = AvaloniaProperty.Register<SetupNavigationBar, string>(nameof(NextButtonText), "Next");

    public static readonly StyledProperty<ICommand?> BackCommandProperty = AvaloniaProperty.Register<SetupNavigationBar, ICommand?>(nameof(BackCommand), null);

    public static readonly StyledProperty<ICommand?> NextCommandProperty = AvaloniaProperty.Register<SetupNavigationBar, ICommand?>(nameof(NextCommand), null);

    public string NextButtonText
    {
        get => GetValue(NextButtonTextProperty);
        set => SetValue(NextButtonTextProperty, value);
    }

    public ICommand? BackCommand
    {
        get => GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public ICommand? NextCommand
    {
        get => GetValue(NextCommandProperty);
        set => SetValue(NextCommandProperty, value);
    }
}