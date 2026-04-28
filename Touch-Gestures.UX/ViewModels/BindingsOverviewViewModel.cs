using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenTabletDriver.External.Common.Serializables;
using ReactiveUI;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Entities.Tablet;
using TouchGestures.UX.Events;
using TouchGestures.UX.Extentions;
using TouchGestures.UX.Models;
using TouchGestures.UX.ViewModels.Dialogs;
using Rect = Avalonia.Rect;

namespace TouchGestures.UX.ViewModels;

public partial class BindingsOverviewViewModel : NavigableViewModel, IDisposable
{
    #region Fields

    private string _searchText = "";

    private readonly MainViewModel _parentViewModel;

    private readonly TwoChoiceDialogViewModel _confirmationDialogData = new()
    {
        Title = "Deleting a Gesture...",
        Content = "Are you sure you want to delete this gesture?",
        PositiveChoice = "Yes",
        NegativeChoice = "No"
    };

    private Settings _settings = new();

    [ObservableProperty]
    private bool _isReady = false;

    [ObservableProperty]
    private bool _isTabletsEmpty = true;

    [ObservableProperty]
    private bool _isEmpty = true;

    [ObservableProperty]
    private bool _setupNotInProgress = true;

    [ObservableProperty]
    private ObservableCollection<SerializablePlugin> _plugins = new();

    [ObservableProperty]
    private ObservableCollection<TabletGesturesOverview> _tablets = new();

    [ObservableProperty]
    private TabletGesturesOverview? _selectedTablet;

    private int _selectedTabletIndex = -1;

    [ObservableProperty]
    private ObservableCollection<GestureBindingDisplayViewModel> _currentGestureBindings = new();

    #region Cancellation Tokens

    private CancellationTokenSource _setupToken = new();

    #endregion

    #endregion

    #region Constructors

    // Design-time constructor
    public BindingsOverviewViewModel()
    {
        _parentViewModel = new MainViewModel();
        IsReady = false;

        NextViewModel = this;
    }

    public BindingsOverviewViewModel(MainViewModel mainViewModel)
    {
        _parentViewModel = mainViewModel;
        IsReady = false;

        _parentViewModel.Ready += OnReady;
        _parentViewModel.Disconnected += OnDisconnected;

        NextViewModel = this;
    }

    public BindingsOverviewViewModel(MainViewModel mainViewModel, Settings settings) : this(mainViewModel)
        => SetSettings(settings);

    #endregion

    #region Events

    private event EventHandler? TabletChanged;
    public event EventHandler<EventArgs>? SaveRequested;
    public event EventHandler<GestureProfile>? ProfileChanged;

    #endregion

    #region Properties

    public GestureDebuggerViewModel DebuggerViewModel { get; } = new();

    public Interaction<TwoChoiceDialogViewModel, bool> ConfirmationDialog { get; } = new();

    public int SelectedTabletIndex
    {
        get => _selectedTabletIndex;
        set
        {
            SetProperty(ref _selectedTabletIndex, value);

            if (value < 0 || value >= Tablets.Count)
                return;

            SelectedTablet = Tablets[value];
            TabletChanged?.Invoke(this, EventArgs.Empty);
        }
    }

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
    public void SetSettings(Settings settings)
    {
        // Dispose of previous bindings beforehand
        DisposeCurrentContext();

        _settings = settings;
    }

    /// <summary>
    ///   Set the tablets of the view model.
    /// </summary>
    /// <param name="tablets">The tablets to set.</param>
    public void SetTablets(IEnumerable<SharedTabletReference> tablets)
    {
        Tablets.Clear();

        foreach (var profile in _settings.Profiles)
        {
            var tablet = tablets.FirstOrDefault(x => x.Name == profile.Name);

            if (tablet == null)
                continue;

            // Start building a new tablet overview && build the gesture binsing displays
            TabletGesturesOverview overview = new(tablet, profile);

            overview.Gestures.CollectionChanged += OnGestureCollectionChanged;

            foreach (var gesture in profile)
            {
                var bindingDisplay = SetupNewBindingDisplay(gesture);

                overview.Gestures.Add(bindingDisplay);
            }

            Tablets.Add(overview);
        }

        // If a tablet was selected, re-select it if it still exists
        if (SelectedTablet != null)
            SelectedTablet = Tablets.FirstOrDefault(x => x.Name == SelectedTablet.Name);

        SelectedTablet ??= Tablets.FirstOrDefault();

        if (SelectedTablet != null)
            SelectedTabletIndex = Tablets.IndexOf(SelectedTablet);

        IsTabletsEmpty = !Tablets.Any();
        IsEmpty = !SelectedTablet?.Gestures.Any() ?? true;

        IsReady = _parentViewModel.IsReady;
        TabletChanged += OnTabletChanged;
    }

    private GestureBindingDisplayViewModel SetupNewBindingDisplay(Gesture gesture)
    {
        var bindingDisplay = new GestureBindingDisplayViewModel(gesture);

        bindingDisplay.Content = bindingDisplay.Store?.GetHumanReadableString();
        bindingDisplay.Description = gesture.DisplayName;

        bindingDisplay.IsReady = _parentViewModel.IsReady;
        bindingDisplay.EditRequested += OnEditRequested;
        bindingDisplay.DeletionRequested += (s, e) => Task.Run(() => OnDeletionRequested(s, e));
        bindingDisplay.BindingChanged += OnGestureBindingsChanged;

        return bindingDisplay;
    }

    #region Commands

    /// <summary>
    ///   Go back to the previous view model.
    /// </summary>
    protected override void GoBack() => throw new InvalidOperationException();

    /// <summary>
    ///   Request the save of the current bindings.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsReady))]
    public void RequestSave() => SaveRequested?.Invoke(this, EventArgs.Empty);

    /// <summary>
    ///   Start the gesture setup process of a new gesture. <br/>
    ///   The Gesture Selection Menu will be shown.
    /// </summary>
    /// 
    [RelayCommand(CanExecute = nameof(IsReady))]
    public void StartSetupWizard()
        => _ = Dispatcher.UIThread.InvokeAsync(StartSetupWizardAsync, DispatcherPriority.Input, _setupToken.Token);

    #endregion

    #region Setup Methods

    private async Task StartSetupWizardAsync()
    {
        if (SelectedTablet == null)
            throw new InvalidOperationException("No tablet selected.");

        var setupWizard = PrepareSetupWizard();

        setupWizard.BackRequested += OnBackRequestedAhead;

        var setup = await setupWizard.SelectionComplete;
        var gesture = await setupWizard.Start(setup);

        setupWizard.BackRequested -= OnBackRequestedAhead;

        NextViewModel = this;
        SetupNotInProgress = true;

        if (gesture == null)
            return;

        // We build the binding display using content from the plugin property & returned data from the binding dialog
        var bindingDisplay = SetupNewBindingDisplay(gesture);

        // We add the binding to the list of bindings, we may need to insert it instead to avoid having to re-sort the list
        SelectedTablet.Add(bindingDisplay);

        IsEmpty = false;

        ProfileChanged?.Invoke(this, SelectedTablet.Profile);
    }

    /// <summary>
    ///   Start the gesture editing process.
    /// </summary>
    /// <param name="bindingDisplay">The Display containing the gesture to edit.</param>
    private async Task StartSetupWizardAsync(GestureBindingDisplayViewModel bindingDisplay)
    {
        if (SelectedTablet == null)
            throw new InvalidOperationException("No tablet selected.");

        var setupWizard = PrepareSetupWizard();

        // We need to cancel the setup when the Cancel button is pressed
        setupWizard.BackRequested += OnBackRequestedAhead;

        var gesture = await setupWizard.Edit(bindingDisplay);

        // Unsubscribe to avoid accidents
        setupWizard.BackRequested -= OnBackRequestedAhead;

        NextViewModel = this;
        SetupNotInProgress = true;

        if (gesture == null)
            return;

        // The edit was completed, we need to update the binding display
        bindingDisplay.Store = gesture.Store;
        bindingDisplay.Content = gesture.Store?.GetHumanReadableString();

        ProfileChanged?.Invoke(this, SelectedTablet.Profile);
    }

    private GestureSetupWizardViewModel PrepareSetupWizard()
    {
        if (SelectedTablet == null)
            throw new InvalidOperationException("No tablet selected.");

        var x = Math.Round(SelectedTablet.Reference.Size.X, 5);
        var y = Math.Round(SelectedTablet.Reference.Size.Y, 5);

        var setupWizard = new GestureSetupWizardViewModel(new Rect(0, 0, x, y),
                                                          SelectedTablet.Profile.IsMultiTouch);

        SetupNotInProgress = false;
        NextViewModel = setupWizard;
        return setupWizard;
    }

    #endregion

    #endregion

    #region Event Handlers

    /// <summary>
    ///   Filter the gestures based on the search text.
    /// </summary>
    /// <param name="text">The search text.</param>
    private void OnSearchTextChanged(string text)
    {
        if (SelectedTablet == null)
            return;

        CurrentGestureBindings.Clear();

        if (string.IsNullOrWhiteSpace(text))
            CurrentGestureBindings.AddRange(SelectedTablet.Gestures);
        else
            CurrentGestureBindings.AddRange(SelectedTablet.Gestures.Where(x => GestureNameContains(x, text)));
    }

    /// <summary>
    ///   Handle the change of the selected tablet.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTabletChanged(object? sender, EventArgs e)
    {
        if (SelectedTablet == null)
            return;

        IsEmpty = !SelectedTablet.Gestures.Any();

        OnSearchTextChanged(SearchText);
        DebuggerViewModel.SelectedTablet = SelectedTablet;
    }

    #region Navigation

    private void OnEditRequested(object? sender, EventArgs e)
    {
        if (sender is not GestureBindingDisplayViewModel bindingDisplay)
            throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

        _ = Dispatcher.UIThread.InvokeAsync(() => StartSetupWizardAsync(bindingDisplay), DispatcherPriority.Input, _setupToken.Token);
    }

    // Setup Cancelled
    private void OnBackRequestedAhead(object? sender, EventArgs e)
    {
        if (NextViewModel is not GestureSetupWizardViewModel)
            throw new InvalidOperationException();

        NextViewModel.BackRequested -= OnBackRequestedAhead;
        NextViewModel = this;
        SetupNotInProgress = true;
    }

    #endregion

    #region Gesture Changes

    private void OnGestureBindingsChanged(object? sender, GestureBindingsChangedArgs e)
    {
        if (SelectedTablet == null)
            return;

        ProfileChanged?.Invoke(this, SelectedTablet.Profile);
    }

    private async Task OnDeletionRequested(object? sender, EventArgs e)
    {
        if (sender is not GestureBindingDisplayViewModel bindingDisplay)
            throw new ArgumentException("Sender must be a GestureBindingDisplayViewModel");

        if (SelectedTablet == null)
            return;

        if (await ConfirmationDialog.Handle(_confirmationDialogData).ToTask() == false)
            return;

        SelectedTablet.Remove(bindingDisplay);

        IsEmpty = !SelectedTablet.Gestures.Any();

        ProfileChanged?.Invoke(this, SelectedTablet.Profile);
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

        foreach (var tablet in Tablets)
            foreach (var binding in tablet.Gestures)
                binding.IsReady = true;
    }

    private void OnDisconnected(object? sender, EventArgs e)
    {
        IsReady = false;

        DisposeCurrentContext();

        foreach (var tablet in Tablets)
            foreach (var binding in tablet.Gestures)
                binding.IsReady = false;
    }

    #endregion

    #endregion

    #region Static Methods

    private static bool GestureNameContains(GestureBindingDisplayViewModel gestureTileViewModel, string text)
    {
        return gestureTileViewModel.Description?.Contains(text, StringComparison.CurrentCultureIgnoreCase) ?? false;
    }

    #endregion

    #region Disposal

    public void DisposeCurrentContext()
    {
        foreach (var tablet in Tablets)
            tablet.Dispose();

        TabletChanged -= OnTabletChanged;

        Tablets.Clear();
        IsTabletsEmpty = true;
        IsEmpty = true;
    }

    public void Dispose()
    {
        DisposeCurrentContext();

        _parentViewModel.Ready -= OnReady;
        _parentViewModel.Disconnected -= OnDisconnected;

        SaveRequested = null;
        ProfileChanged = null;

        GC.SuppressFinalize(this);
    }

    #endregion
}
