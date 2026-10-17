using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace TouchGestures.UX.Controls;

public partial class GestureTile : TemplatedControl, IDisposable
{
    private static readonly Uri _placeholderIconUri = new("avares://Touch-Gestures.UX/Assets/Icons/placeholder.png");

    private static readonly Bitmap _placeholderIcon;

    static GestureTile()
    {
        var stream = AssetLoader.Open(_placeholderIconUri);
        _placeholderIcon = new Bitmap(stream);
    }

    public static readonly StyledProperty<string> GestureNameProperty = AvaloniaProperty.Register<GestureTile, string>(nameof(GestureName));

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<GestureTile, string>(nameof(Description));

    public static readonly DirectProperty<GestureTile, IImage?> IconProperty = AvaloniaProperty.RegisterDirect<GestureTile, IImage?>(nameof(Icon), o => o.Icon, (o, v) => o.Icon = v);

    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<GestureTile, ICommand?>(nameof(Command));

    public string GestureName
    {
        get => GetValue(GestureNameProperty);
        set => SetValue(GestureNameProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    private IImage? _icon = _placeholderIcon;

    public IImage? Icon
    {
        get => _icon;
        set => SetAndRaise(IconProperty, ref _icon, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == IconProperty)
            if (change.NewValue == null)
                Icon = _placeholderIcon;

        base.OnPropertyChanged(change);
    }

    public void Dispose()
    {
        if (Icon is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}