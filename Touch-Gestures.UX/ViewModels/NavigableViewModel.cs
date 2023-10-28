using System;
using CommunityToolkit.Mvvm.Input;

namespace TouchGestures.UX.ViewModels;

public abstract partial class NavigableViewModel : ViewModelBase
{
    public abstract event EventHandler? BackRequested;

    public bool CanGoBack { get; init; }

    public NavigableViewModel? NextViewModel { get; set; } = null!;

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    protected abstract void GoBack();
}