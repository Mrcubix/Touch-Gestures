using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Interfaces;
using TouchGestures.UX.Events;

namespace TouchGestures.UX.ViewModels
{
    public partial class BindingsOverviewViewModel : NavigableViewModel
    {
        #region Fields

        private readonly MainViewModel _parentViewModel;

        [ObservableProperty]
        private bool _isConnected;

        [ObservableProperty]
        private string _searchText = "";

        [ObservableProperty]
        private ObservableCollection<GestureBindingDisplayViewModel> _gestureBindings = new();

        #endregion

        #region Constructors

        public BindingsOverviewViewModel(MainViewModel mainViewModel)
        {
            _parentViewModel = mainViewModel;

            _parentViewModel.Connected += OnConnected;
            _parentViewModel.Disconnected += OnDisconnected;

            IsConnected = false;

            NextViewModel = this;
            BackRequested = null!;
        }

        public BindingsOverviewViewModel(MainViewModel mainViewModel, ObservableCollection<Gesture> gestures) : this(mainViewModel)
        {
            foreach (var gesture in gestures)
            {
                var bindingDisplay = new GestureBindingDisplayViewModel(gesture);
                bindingDisplay.Content = _parentViewModel.GetFriendlyContentFromProperty(bindingDisplay.PluginProperty);

                bindingDisplay.IsConnected = _parentViewModel.IsConnected;
                bindingDisplay.EditRequested += OnEditRequested;
                bindingDisplay.DeletionRequested += OnDeletionRequested;

                GestureBindings.Add(bindingDisplay);
            }

            SubscribeToEvents();
        }

        public void SubscribeToEvents()
        {
            foreach (var binding in GestureBindings)
            {
                binding.BindingChanged += OnGestureBindingsChanged;
            }
        }

        #endregion

        #region Events

        public override event EventHandler? BackRequested;
        public event EventHandler<GestureChangedEventArgs>? GesturesChanged;

        #endregion

        #region Methods

        protected override void GoBack()
        {
            throw new InvalidOperationException();
        }

        [RelayCommand(CanExecute = nameof(IsConnected))]
        public void StartSetupWizard()
        {
            var setupWizard = new GestureSetupWizardViewModel();
            setupWizard.SetupCompleted += OnSetupCompleted;
            setupWizard.BackRequested += OnBackRequestedAhead;

            NextViewModel = setupWizard;
        }

        #endregion

        #region Event Handlers

        private void OnBackRequestedAhead(object? sender, EventArgs e)
        {
            if (NextViewModel is not GestureSetupWizardViewModel)
                throw new InvalidOperationException();

            NextViewModel.BackRequested -= OnBackRequestedAhead;
            NextViewModel = this;
        }

        private void OnSetupCompleted(object? sender, GestureAddedEventArgs e)
        {
            if (NextViewModel is not GestureSetupWizardViewModel)
                throw new InvalidOperationException();

            NextViewModel.BackRequested -= OnBackRequestedAhead;
            NextViewModel = this;

            var bindingDisplay = new GestureBindingDisplayViewModel(e);
            bindingDisplay.Content = _parentViewModel.GetFriendlyContentFromProperty(bindingDisplay.PluginProperty);

            bindingDisplay.IsConnected = _parentViewModel.IsConnected;
            bindingDisplay.EditRequested += OnEditRequested;
            bindingDisplay.DeletionRequested += OnDeletionRequested;
            bindingDisplay.BindingChanged += OnGestureBindingsChanged;

            GestureBindings.Add(bindingDisplay);

            GesturesChanged?.Invoke(this, new GestureChangedEventArgs(null, e.Gesture!));
        }

        private void OnEditRequested(object? sender, EventArgs e)
        {
            if (sender is not GestureBindingDisplayViewModel bindingDisplay)
                throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

            var setupWizard = new GestureSetupWizardViewModel();
            setupWizard.Edit(bindingDisplay);

            setupWizard.EditCompleted += (s, args) => OnEditCompleted(s, bindingDisplay, args);
            setupWizard.BackRequested += OnBackRequestedAhead;

            NextViewModel = setupWizard;
        }

        private void OnDeletionRequested(object? sender, EventArgs e)
        {
            if (sender is not GestureBindingDisplayViewModel bindingDisplay)
                throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

            GestureBindings.Remove(bindingDisplay);

            GesturesChanged?.Invoke(this, new GestureChangedEventArgs(bindingDisplay.AssociatedGesture, null));
        }

        private void OnEditCompleted(object? sender, GestureBindingDisplayViewModel bindingDisplay, GestureChangedEventArgs args)
        {
            if (NextViewModel is not GestureSetupWizardViewModel setupWizard)
                throw new InvalidOperationException();

            if (args.NewValue is not ISerializable serialized)
                throw new ArgumentException("The edited gesture must be serializable.");

            bindingDisplay.AssociatedGesture = args.NewValue;
            bindingDisplay.PluginProperty = serialized.PluginProperty;
            bindingDisplay.Content = _parentViewModel.GetFriendlyContentFromProperty(serialized.PluginProperty);

            setupWizard.EditCompleted -= (s, args) => OnEditCompleted(s, bindingDisplay, args);
            NextViewModel = this;
        }

        // TODO: Ideally it should OnEditCompleted & OnGestureBindingsChanged should be merged into one event handler
        private void OnGestureBindingsChanged(object? sender, EventArgs e)
        {
            if (sender is not GestureBindingDisplayViewModel bindingDisplay)
                throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

            GesturesChanged?.Invoke(this, new GestureChangedEventArgs(bindingDisplay.AssociatedGesture, bindingDisplay.AssociatedGesture));
        }

        private void OnConnected(object? sender, EventArgs e)
        {
            IsConnected = true;

            foreach (var binding in GestureBindings)
                binding.IsConnected = true;
        }

        private void OnDisconnected(object? sender, EventArgs e)
        {
            IsConnected = false;

            foreach (var binding in GestureBindings)
                binding.IsConnected = false;
        }

        #endregion
    }
}