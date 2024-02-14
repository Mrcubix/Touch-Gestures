using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TouchGestures.Lib.Entities;
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

        // Design-time constructor
        public BindingsOverviewViewModel()
        {
            _parentViewModel = new MainViewModel();
            IsConnected = false;

            NextViewModel = this;
            BackRequested = null!;
        }

        public BindingsOverviewViewModel(MainViewModel mainViewModel)
        {
            _parentViewModel = mainViewModel;

            _parentViewModel.Connected += OnConnected;
            _parentViewModel.Disconnected += OnDisconnected;

            IsConnected = false;

            NextViewModel = this;
            BackRequested = null!;
        }

        public BindingsOverviewViewModel(MainViewModel mainViewModel, SerializableSettings settings) : this(mainViewModel)
        {
            foreach (var gesture in settings)
            {
                var bindingDisplay = new GestureBindingDisplayViewModel(gesture);

                bindingDisplay.Content = _parentViewModel.GetFriendlyContentFromProperty(bindingDisplay.PluginProperty);

                bindingDisplay.IsConnected = _parentViewModel.IsConnected;
                bindingDisplay.EditRequested += OnEditRequested;
                bindingDisplay.DeletionRequested += OnDeletionRequested;

                GestureBindings.Add(bindingDisplay);
            }

            IsConnected = _parentViewModel.IsConnected;

            SubscribeToEvents();
        }

        public void SubscribeToEvents()
        {
            foreach (var binding in GestureBindings)
                binding.BindingChanged += OnGestureBindingsChanged;
        }

        #endregion

        #region Events

        public override event EventHandler? BackRequested;
        public event EventHandler<EventArgs>? SaveRequested;
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

        [RelayCommand(CanExecute = nameof(IsConnected))]
        public void RequestSave() => SaveRequested?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Event Handlers

        //
        // Navigation
        //

        private void OnBackRequestedAhead(object? sender, EventArgs e)
        {
            if (NextViewModel is not GestureSetupWizardViewModel)
                throw new InvalidOperationException();

            NextViewModel.BackRequested -= OnBackRequestedAhead;
            NextViewModel = this;
        }

        //
        // Additions
        //

        private void OnSetupCompleted(object? sender, GestureAddedEventArgs e)
        {
            if (NextViewModel is not GestureSetupWizardViewModel)
                throw new InvalidOperationException();

            NextViewModel.BackRequested -= OnBackRequestedAhead;
            NextViewModel = this;

            // We build the binding display using content from the plugin property & returned data from the binding dialog
            var bindingDisplay = new GestureBindingDisplayViewModel(e);
            bindingDisplay.Content = _parentViewModel.GetFriendlyContentFromProperty(bindingDisplay.PluginProperty);

            bindingDisplay.IsConnected = _parentViewModel.IsConnected;
            bindingDisplay.EditRequested += OnEditRequested;
            bindingDisplay.DeletionRequested += OnDeletionRequested;
            bindingDisplay.BindingChanged += OnGestureBindingsChanged;

            // We add the binding to the list of bindings, we may need to insert it instead to avoid having to re-sort the list
            GestureBindings.Add(bindingDisplay);

            GesturesChanged?.Invoke(this, new GestureChangedEventArgs(null, e.Gesture!));
        }

        //
        // Changes
        //

        private void OnEditRequested(object? sender, EventArgs e)
        {
            if (sender is not GestureBindingDisplayViewModel bindingDisplay)
                throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

            var setupWizard = new GestureSetupWizardViewModel();

            // We need to check whenever the edit is completed & when te user goes back
            setupWizard.EditCompleted += (s, args) => OnEditCompleted(s, bindingDisplay, args);
            setupWizard.BackRequested += OnBackRequestedAhead;

            setupWizard.Edit(bindingDisplay);

            NextViewModel = setupWizard;
        }

        private void OnEditCompleted(object? sender, GestureBindingDisplayViewModel bindingDisplay, GestureChangedEventArgs args)
        {
            if (NextViewModel is not GestureSetupWizardViewModel setupWizard)
                throw new InvalidOperationException();

            if (args.NewValue is not ISerializable serialized)
                throw new ArgumentException("The edited gesture must be serializable.");

            if (args.NewValue is not INamed named)
                throw new ArgumentException("The edited gesture must be named.");

            // The edit was completed, we need to update the binding display
            bindingDisplay.AssociatedGesture = args.NewValue;
            bindingDisplay.PluginProperty = serialized.PluginProperty;
            bindingDisplay.Description = named.Name;
            bindingDisplay.Content = _parentViewModel.GetFriendlyContentFromProperty(serialized.PluginProperty);

            // We need to unsubscribe from the events
            setupWizard.EditCompleted -= (s, args) => OnEditCompleted(s, bindingDisplay, args);
            setupWizard.BackRequested -= OnBackRequestedAhead;

            NextViewModel = this;

            GesturesChanged?.Invoke(this, args);
        }

        // TODO: Ideally it should OnEditCompleted & OnGestureBindingsChanged should be merged into one event handler
        private void OnGestureBindingsChanged(object? sender, GestureBindingsChangedArgs e)
        {
            if (sender is not GestureBindingDisplayViewModel bindingDisplay)
                throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

            // We need to update the content of the binding display
            var args = new GestureChangedEventArgs(bindingDisplay.AssociatedGesture, bindingDisplay.AssociatedGesture);
            bindingDisplay.Content = _parentViewModel.GetFriendlyContentFromProperty(bindingDisplay.PluginProperty);

            GesturesChanged?.Invoke(this, args);
        }

        //
        // Deletion
        //

        private void OnDeletionRequested(object? sender, EventArgs e)
        {
            if (sender is not GestureBindingDisplayViewModel bindingDisplay)
                throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

            GestureBindings.Remove(bindingDisplay);

            GesturesChanged?.Invoke(this, new GestureChangedEventArgs(bindingDisplay.AssociatedGesture, null));
        }

        //
        // Some Actions need to be disabled / enabled depending on the connection state
        //

        private void OnConnected(object? sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                IsConnected = true;

                foreach (var binding in GestureBindings)
                    binding.IsConnected = true;
            });
        }

        private void OnDisconnected(object? sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                IsConnected = false;

                foreach (var binding in GestureBindings)
                    binding.IsConnected = false;
            });
        }

        #endregion
    }
}