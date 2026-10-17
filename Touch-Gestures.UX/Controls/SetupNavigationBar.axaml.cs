using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace TouchGestures.UX.Controls;

public partial class SetupNavigationBar : TemplatedControl
{
    public static readonly StyledProperty<string> BackButtonTextProperty = AvaloniaProperty.Register<SetupNavigationBar, string>(nameof(BackButtonText), "Back");

    public static readonly StyledProperty<ICommand?> BackCommandProperty = AvaloniaProperty.Register<SetupNavigationBar, ICommand?>(nameof(BackCommand), null);

    public static readonly StyledProperty<bool> CanGoBackProperty = AvaloniaProperty.Register<SetupNavigationBar, bool>(nameof(CanGoBack), true);

    public static readonly StyledProperty<string> NextButtonTextProperty = AvaloniaProperty.Register<SetupNavigationBar, string>(nameof(NextButtonText), "Next");

    public static readonly StyledProperty<ICommand?> NextCommandProperty = AvaloniaProperty.Register<SetupNavigationBar, ICommand?>(nameof(NextCommand), null);

    public static readonly StyledProperty<bool> CanGoNextProperty = AvaloniaProperty.Register<SetupNavigationBar, bool>(nameof(CanGoNext), true);

    public string BackButtonText
    {
        get => GetValue(BackButtonTextProperty);
        set => SetValue(BackButtonTextProperty, value);
    }

    public ICommand? BackCommand
    {
        get => GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public bool CanGoBack
    {
        get => GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    public string NextButtonText
    {
        get => GetValue(NextButtonTextProperty);
        set => SetValue(NextButtonTextProperty, value);
    }

    public ICommand? NextCommand
    {
        get => GetValue(NextCommandProperty);
        set => SetValue(NextCommandProperty, value);
    }

    public bool CanGoNext
    {
        get => GetValue(CanGoNextProperty);
        set => SetValue(CanGoNextProperty, value);
    }
}