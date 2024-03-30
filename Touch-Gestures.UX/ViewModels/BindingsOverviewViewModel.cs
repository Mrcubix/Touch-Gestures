using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.Lib.Interfaces;
using TouchGestures.UX.Events;
using TouchGestures.UX.Extentions;
using TouchGestures.UX.ViewModels.Dialogs;
using Rect = Avalonia.Rect;

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class BindingsOverviewViewModel : NavigableViewModel, IDisposable
{
    #region Fields

    private readonly MainViewModel _parentViewModel;

    private readonly TwoChoiceDialogViewModel _confirmationDialogData = new()
    {
        Title = "Deleting a Gesture...",
        Content = "Are you sure you want to delete this gesture?",
        PositiveChoice = "Yes",
        NegativeChoice = "No"
    };

    [ObservableProperty]
    private bool _isReady = false;

    [ObservableProperty]
    private bool _isEmpty = true;

    private string _searchText = "";

    [ObservableProperty]
    private ObservableCollection<SharedTabletReference> _tablets = new();

    private SharedTabletReference? _selectedTablet;

    private ObservableCollection<GestureBindingDisplayViewModel> _gestureBindings = new();

    [ObservableProperty]
    private ObservableCollection<GestureBindingDisplayViewModel> _currentGestureBindings = new();

    #endregion

    #region Constructors

    // Design-time constructor
    public BindingsOverviewViewModel()
    {
        _parentViewModel = new MainViewModel();
        IsReady = false;

        _gestureBindings.CollectionChanged += OnGestureCollectionChanged;

        NextViewModel = this;
        BackRequested = null!;
    }

    public BindingsOverviewViewModel(MainViewModel mainViewModel)
    {
        _parentViewModel = mainViewModel;
        IsReady = false;

        _parentViewModel.Ready += OnReady;
        _parentViewModel.Disconnected += OnDisconnected;

        _gestureBindings.CollectionChanged += OnGestureCollectionChanged;

        NextViewModel = this;
        BackRequested = null!;
    }

    public BindingsOverviewViewModel(MainViewModel mainViewModel, SerializableSettings settings) : this(mainViewModel)
        => SetSettings(settings);

    #endregion

    #region Events

    private event EventHandler? TabletChanged;

    public override event EventHandler? BackRequested;
    public event EventHandler<EventArgs>? SaveRequested;
    public event EventHandler<GestureChangedEventArgs>? GesturesChanged;

    #endregion

    #region Properties

    public Interaction<TwoChoiceDialogViewModel, bool> ConfirmationDialog { get; } = new();

    public SharedTabletReference? SelectedTablet
    {
        get => _selectedTablet;
        set
        {
            SetProperty(ref _selectedTablet, value);
            TabletChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public Rect Bounds { get; set; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            OnSearchTextChanged(value);
        }
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Set the settings of the view model.
    /// </summary>
    /// <param name="settings">The settings to set.</param>
    public void SetSettings(SerializableSettings settings)
    {
        // Dispose of previous bindings beforehand
        DisposeCurrent();

        foreach (var gesture in settings)
        {
            var bindingDisplay = SetupNewBindingDisplay(gesture);

            _gestureBindings.Add(bindingDisplay);
        }

        IsReady = _parentViewModel.IsReady;
        IsEmpty = _gestureBindings.Count == 0;
    }

    /// <summary>
    ///   Set the tablets of the view model.
    /// </summary>
    /// <param name="tablets">The tablets to set.</param>
    public void SetTablets(SharedTabletReference[] tablets)
    {
        Tablets.Clear();
        Tablets.AddRange(tablets);

        SelectedTablet = tablets.FirstOrDefault();
    }

    private GestureBindingDisplayViewModel SetupNewBindingDisplay(Gesture gesture)
    {
        var bindingDisplay = new GestureBindingDisplayViewModel(gesture);

        bindingDisplay.Content = _parentViewModel.GetFriendlyContentFromProperty(bindingDisplay.PluginProperty);

        bindingDisplay.IsReady = _parentViewModel.IsReady;
        bindingDisplay.EditRequested += OnEditRequested;
        bindingDisplay.DeletionRequested += (s, e) => Task.Run(() => OnDeletionRequested(s, e));
        bindingDisplay.BindingChanged += OnGestureBindingsChanged;

        return bindingDisplay;
    }

    /// <summary>
    ///   Start the gesture setup process of a new gesture.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsReady))]
    public void StartSetupWizard()
    {
        var setupWizard = new GestureSetupWizardViewModel(Bounds);

        setupWizard.SetupCompleted += OnSetupCompleted;
        setupWizard.BackRequested += OnBackRequestedAhead;

        NextViewModel = setupWizard;
    }

    /// <summary>
    ///   Request the save of the current bindings.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsReady))]
    public void RequestSave() => SaveRequested?.Invoke(this, EventArgs.Empty);

    protected override void GoBack() => throw new InvalidOperationException();

    #endregion

    #region Event Handlers

    /// <summary>
    ///   Filter the gestures based on the search text.
    /// </summary>
    /// <param name="text">The search text.</param>
    private void OnSearchTextChanged(string text)
    {
        CurrentGestureBindings.Clear();

        if (string.IsNullOrWhiteSpace(text))
            CurrentGestureBindings.AddRange(_gestureBindings);
        else
            CurrentGestureBindings.AddRange(_gestureBindings.Where(x => GestureNameStartsWith(x, text)));
    }

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

    #region Gesture Changes

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
        var bindingDisplay = SetupNewBindingDisplay(e.Gesture!);

        //bindingDisplay.Description = e.BindingDisplay.Description;    
        //bindingDisplay.PluginProperty = e.BindingDisplay.PluginProperty;       

        // We add the binding to the list of bindings, we may need to insert it instead to avoid having to re-sort the list
        _gestureBindings.Add(bindingDisplay);

        IsEmpty = _gestureBindings.Count == 0;

        GesturesChanged?.Invoke(this, new GestureChangedEventArgs(null, e.Gesture!));
    }

    //
    // Changes
    //

    private void OnEditRequested(object? sender, EventArgs e)
    {
        if (sender is not GestureBindingDisplayViewModel bindingDisplay)
            throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

        var setupWizard = new GestureSetupWizardViewModel(Bounds);

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

    private async Task OnDeletionRequested(object? sender, EventArgs e)
    {
        if (sender is not GestureBindingDisplayViewModel bindingDisplay)
            throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

        var res = await ConfirmationDialog.Handle(_confirmationDialogData).ToTask();

        if (!res)
            return;

        _gestureBindings.Remove(bindingDisplay);

        IsEmpty = _gestureBindings.Count == 0;

        GesturesChanged?.Invoke(this, new GestureChangedEventArgs(bindingDisplay.AssociatedGesture, null));
    }

    private void OnGestureCollectionChanged(object? sender, EventArgs e)
        => Dispatcher.UIThread.InvokeAsync(() => OnSearchTextChanged(SearchText));

    #endregion

    #region Connection related events

    //
    // Some Actions need to be disabled / enabled depending on the connection state
    //

    private void OnReady(object? sender, EventArgs e)
    {
        IsReady = true;

        foreach (var binding in _gestureBindings)
            binding.IsReady = true;
    }

    private void OnDisconnected(object? sender, EventArgs e)
    {
        IsReady = false;

        foreach (var binding in _gestureBindings)
            binding.IsReady = false;
    }

    #endregion

    #endregion

    #region Static Methods

    private static bool GestureNameStartsWith(GestureBindingDisplayViewModel gestureTileViewModel, string text)
    {
        return gestureTileViewModel.Description?.StartsWith(text, StringComparison.CurrentCultureIgnoreCase) ?? false;
    }

    #endregion

    #region Disposal

    public void DisposeCurrent()
    {
        foreach (var binding in _gestureBindings)
            binding.Dispose();

        _gestureBindings.Clear();
    }

    public void Dispose()
    {
        DisposeCurrent();

        _parentViewModel.Ready -= OnReady;
        _parentViewModel.Disconnected -= OnDisconnected;

        SaveRequested = null;
        GesturesChanged = null;
        BackRequested = null;

        GC.SuppressFinalize(this);
    }

    #endregion
}
