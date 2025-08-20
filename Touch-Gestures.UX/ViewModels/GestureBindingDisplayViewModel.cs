using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Interfaces;
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

    public GestureBindingDisplayViewModel(Gesture gesture)
    {
        if (gesture is not ISerializable serialized)
            throw new ArgumentException("Gesture must be serializable");

        AssociatedGesture = gesture;

        PluginProperty = serialized.PluginProperty;

        if (gesture is INamed named)
            Description = named.Name;

        Initialize();
    }

    public GestureBindingDisplayViewModel(GestureAddedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e.Gesture);

        if (e.Gesture is not ISerializable)
            throw new ArgumentException("Gesture must implement ISerializable");

        Description = e.BindingDisplay.Description;
        Content = e.BindingDisplay.Content;
        PluginProperty = e.BindingDisplay.PluginProperty;

        AssociatedGesture = e.Gesture;

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
        if (e.PropertyName == nameof(PluginProperty))
        {
            if (AssociatedGesture is not ISerializable serialized)
                return;

            var args = new GestureBindingsChangedArgs(serialized.PluginProperty, PluginProperty);

            serialized.PluginProperty = PluginProperty;

            BindingChanged?.Invoke(this, args);
        }
    }

    public void Dispose()
    {
        PropertyChanged -= OnPropertyChanged;

        EditRequested = null;
        DeletionRequested = null;
        BindingChanged = null;

        GC.SuppressFinalize(this);
    }

    #endregion
}
