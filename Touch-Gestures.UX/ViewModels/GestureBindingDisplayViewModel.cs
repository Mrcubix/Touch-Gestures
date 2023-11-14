using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.Entities.Gestures;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Interfaces;
using TouchGestures.UX.Events;

namespace TouchGestures.UX.ViewModels
{
    public partial class GestureBindingDisplayViewModel : BindingDisplayViewModel
    {
        #region Fields

        [ObservableProperty]
        private bool _isConnected = false;

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

            InitializeDescription();

            Initialize();
        }

        public GestureBindingDisplayViewModel(GestureAddedEventArgs e)
        {
            if (e.Gesture == null)
                throw new ArgumentNullException(nameof(e.Gesture));

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

        public event EventHandler? BindingChanged;

        #endregion

        #region Commands

        [RelayCommand(CanExecute = nameof(IsConnected))]
        public void EditGesture() => EditRequested?.Invoke(this, EventArgs.Empty);

        [RelayCommand]
        public void DeleteGesture() => DeletionRequested?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Methods

        private void InitializeDescription() =>
            // TODO: Implement other gestures
            Description = AssociatedGesture switch
            {
                TapGesture => "Tap",
                SwipeGesture => "Swipe",
                _ => "Unknown Gesture"
            };

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PluginProperty))
            {
                if (AssociatedGesture is not ISerializable serialized)
                    return;

                serialized.PluginProperty = PluginProperty;

                BindingChanged?.Invoke(this, EventArgs.Empty);
            }
                
        }

        #endregion
    }
}