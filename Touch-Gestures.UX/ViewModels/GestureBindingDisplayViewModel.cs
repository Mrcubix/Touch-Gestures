using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.UX.Events;

namespace TouchGestures.UX.ViewModels;

public partial class GestureBindingDisplayViewModel : BindingDisplayViewModel, IDisposable
{
    #region Fields

    [ObservableProperty]
    private bool _isReady = false;

    [ObservableProperty]
    private Gesture _associatedGesture;

    #endregion

    #region Constructors

    public GestureBindingDisplayViewModel(Gesture gesture) : base(gesture.Store!)
    {
        AssociatedGesture = gesture;

        Store = gesture.Store;
        Content = gesture.Store?.GetHumanReadableString();

        Initialize();
    }

    private void Initialize()
    {
        PropertyChanged += OnPropertyChanged;
    }

    #endregion

    #region Events

    public event EventHandler? EditRequested;

    public event EventHandler? DeletionRequested;

    public event EventHandler<GestureBindingsChangedArgs>? BindingChanged;

    #endregion

    #region Commands

    [RelayCommand(CanExecute = nameof(IsReady))]
    public void EditGesture() => EditRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand(CanExecute = nameof(IsReady))]
    public void DeleteGesture() => DeletionRequested?.Invoke(this, EventArgs.Empty);

    #endregion

    #region Event Handlers

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Store))
        {
            var args = new GestureBindingsChangedArgs(AssociatedGesture.Store, Store);
            AssociatedGesture.Store = Store;
            BindingChanged?.Invoke(this, args);
        }
    }

    #endregion

    #region IDisposable

    public new void Dispose()
    {
        PropertyChanged -= OnPropertyChanged;

        EditRequested = null;
        DeletionRequested = null;
        BindingChanged = null;

        base.Dispose();

        GC.SuppressFinalize(this);
    }

    #endregion
}
