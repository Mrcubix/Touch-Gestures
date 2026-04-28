using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TouchGestures.UX.ViewModels;

public abstract partial class NavigableViewModel : ViewModelBase
{
    public event EventHandler? BackRequested;

    public bool CanGoBack { get; init; }

    [ObservableProperty]
    protected NavigableViewModel? _nextViewModel = null;

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    protected virtual void GoBack()
    {
        if (CanGoBack)
            BackRequested?.Invoke(this, EventArgs.Empty);
    }
}