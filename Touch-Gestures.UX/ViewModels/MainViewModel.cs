using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.External.Common.Serializables;
using TouchGestures.Lib.Contracts;
using StreamJsonRpc;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TouchGestures.Lib.Entities;
using System.Threading;
using TouchGestures.UX.Events;
using Avalonia.Threading;
using System.Numerics;
using Avalonia;
using Newtonsoft.Json;
using TouchGestures.Lib.Converters;
using TouchGestures.Lib.Entities.Tablet;

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class MainViewModel : NavigableViewModel
{
    #region Fields

    private static readonly List<JsonConverter> Converters = new() { new SharedAreaConverter() };

    private CancellationTokenSource _reconnectionTokenSource = new();
    private RpcClient<IGesturesDaemon> _client;
    private SerializableSettings _settings;

    #endregion

    #region Observable Fields

    // Home Menu

    // Bindings Overview
    // Add Button
    // -> GestureSetupWizard
    // Gesture Selection
    // Gesture Option Selection
    // Gesture Binding Selection
    // Gesture Settings Tweaking

    // Settings

    [ObservableProperty]
    private ObservableCollection<SerializablePlugin> _plugins = new();

    [ObservableProperty]
    private string _connectionStateText = "Not connected";

    [ObservableProperty]
    private bool _isReady = false;

    #region VMs

    [ObservableProperty]
    private BindingsOverviewViewModel _bindingsOverviewViewModel;

    #endregion

    #region Testing VMs

    [ObservableProperty]
    private NodeGestureEditorViewModel _nodeGestureEditorViewModel = new();

    #endregion

    #endregion

    #region Constructors

    // TODO: Implement settings for plugin
    public MainViewModel()
    {
        _settings = new();

        BindingsOverviewViewModel = new(this);
        BindingsOverviewViewModel.SaveRequested += OnSaveRequested;
        BindingsOverviewViewModel.ProfileChanged += OnProfileChanged;

        BackRequested = null!;

        CanGoBack = false;
        // TODO: Change in production to the home view
        //NextViewModel = _gestureSetupWizardViewModel;
        NextViewModel = this;

        NextViewModel!.PropertyChanged += OnCurrentViewChanged;
        NextViewModel!.PropertyChanging += OnCurrentViewChanging;
        NextViewModel!.BackRequested += OnBackRequestedAhead;

        _client = new("GesturesDaemon");

        InitializeClient();
    }

    private void InitializeClient()
    {
        _client.Converters.AddRange(Converters);

        _client.Connected += OnClientConnected;
        _client.Connecting += OnClientConnecting;
        _client.Disconnected += OnClientDisconnected;
        _client.Attached += (sender, args) => Task.Run(() => OnClientAttached(sender, args));

        _ = Task.Run(ConnectRpcAsync);
    }

    #endregion

    #region Events

    public event EventHandler<ObservableCollection<SerializablePlugin>>? PluginChanged;

    public event EventHandler<SerializableSettings>? SettingsChanged;

    public override event EventHandler? BackRequested;

    public event EventHandler? Ready;

    public event EventHandler? Disconnected;

    #endregion

    #region Methods

    //
    // UX Methods
    //

    protected override void GoBack()
    {
        throw new InvalidOperationException();
    }

    //
    // RPC Methods
    //

    private async Task AttemptReconnectionIndefinitelyAsync()
    {
        if (_client.IsConnected)
            return;

        _reconnectionTokenSource = new();
        var token = _reconnectionTokenSource.Token;

        while (!_client.IsConnected && !token.IsCancellationRequested)
        {
            await Task.Delay(500, token);
            await ConnectRpcAsync();
        }
    }

    private async Task ConnectRpcAsync()
    {
        if (_client.IsConnected)
            return;

        try
        {
            await _client.ConnectAsync();
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    private async Task<List<SerializablePlugin>?> FetchPluginsAsync()
    {
        if (!_client.IsConnected)
            return null;

        try
        {
            return await _client.Instance.GetPlugins();
        }
        catch (Exception e)
        {
            HandleException(e);
        }

        return null;
    }

    private async Task<SerializableSettings?> FetchSettingsAsync()
    {
        if (!_client.IsConnected)
            return null;

        try
        {
            return await _client.Instance.GetSettings();
        }
        catch (Exception e)
        {
            HandleException(e);
        }

        return null;
    }

    private async Task SaveSettingsAsync()
    {
        if (!_client.IsConnected)
            return;

        try
        {
            await _client.Instance.SaveSettings();
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    private async Task UpdateProfileAsync(SerializableProfile profile)
    {
        if (!_client.IsConnected)
            return;

        try
        {
            await _client.Instance.UpdateProfile(profile);
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    // Get Settings

    public string GetFriendlyContentFromProperty(SerializablePluginSettings? property)
    {
        if (property == null || property.Identifier == 0)
            return "";

        var pluginName = GetPluginNameFromIdentifier(property.Identifier);

        return $"{pluginName} : {property.Value}";
    }

    private string? GetPluginNameFromIdentifier(int identifier)
    {
        if (Plugins == null)
            return null;

        return Plugins.FirstOrDefault(x => x.Identifier == identifier)?.PluginName ?? "Unknown";
    }

    #endregion

    #region Event Handlers

    //
    // Plugin Client Event Handlers
    //

    private void OnClientConnected(object? sender, EventArgs e)
    {
        ConnectionStateText = "Connected";
    }

    private void OnClientConnecting(object? sender, EventArgs e)
    {
        ConnectionStateText = "Connecting...";
    }

    private void OnClientDisconnected(object? sender, EventArgs e)
    {
        ConnectionStateText = "Disconnected";
        IsReady = false;
        Disconnected?.Invoke(this, EventArgs.Empty);
        _client.Instance.TabletsChanged -= OnTabletsChanged;

        _ = Task.Run(() => AttemptReconnectionIndefinitelyAsync());
        NextViewModel = this;
    }

    private async Task OnClientAttached(object? sender, EventArgs e)
    {
        ConnectionStateText = "Connected, Fetching Plugins & Settings ...";

        _client.Instance.TabletsChanged += OnTabletsChanged;

        var tempPlugins = await FetchPluginsAsync();

        if (tempPlugins != null)
        {
            Plugins = new ObservableCollection<SerializablePlugin>(tempPlugins);
            PluginChanged?.Invoke(this, Plugins);
        }

        var tablets = await _client.Instance.GetTablets();

        if (tablets != null)
            OnTabletsChanged(this, tablets);

        IsReady = true;
        Ready?.Invoke(this, EventArgs.Empty);
        NextViewModel = BindingsOverviewViewModel;
    }

    public void OnTabletsChanged(object? sender, IEnumerable<SharedTabletReference> tablets)
    {
        if (tablets == null || !tablets.Any())
            return;

        _ = OnTabletsChangedCore(sender, tablets);
    }

    private async Task OnTabletsChangedCore(object? sender, IEnumerable<SharedTabletReference> tablets)
    {
        if (tablets == null || !tablets.Any())
            return;

        SerializableSettings? tempSettings = await FetchSettingsAsync();

        if (tempSettings != null && tablets != null)
        {
            _settings = tempSettings;
            Dispatcher.UIThread.Post(() => OnSettingsChanged(_settings));
            Dispatcher.UIThread.Post(() => BindingsOverviewViewModel.SetTablets(tablets));
        } 
    }

    //
    // UX Event Handlers
    //

    private void OnCurrentViewChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (e.PropertyName != nameof(NextViewModel))
            return;

        if (NextViewModel != null)
            NextViewModel.BackRequested += OnBackRequestedAhead;
    }

    private void OnCurrentViewChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(NextViewModel))
            return;

        if (NextViewModel != null)
            NextViewModel.BackRequested -= OnBackRequestedAhead;
    }

    private void OnBackRequestedAhead(object? sender, EventArgs e)
    {
        NextViewModel = this;
    }

    private void OnSaveRequested(object? sender, EventArgs e)
    {
        _ = SaveSettingsAsync();
    }

    private void OnSettingsChanged(SerializableSettings e)
    {
        bool isOverviewNextViewModel = NextViewModel is BindingsOverviewViewModel;

        BindingsOverviewViewModel.SetSettings(e);

        if (isOverviewNextViewModel)
            NextViewModel = BindingsOverviewViewModel;

        SettingsChanged?.Invoke(this, e);
    }

    //
    // Handling whenever gestures are added / changed / deleted
    //

    private void OnProfileChanged(object? sender, SerializableProfile e)
    {
        _ = UpdateProfileAsync(e);
    }

    #endregion

    #region Exception Handling

    private void HandleException(Exception e)
    {
        switch (e)
        {
            case RemoteRpcException re:
                Console.WriteLine($"An Error occured while attempting to connect to the RPC server: {re.Message}");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("This error could have occured due to an different version of WheelAddon being used with this Interface.");

                ConnectionStateText = "Disconnected";
                break;
            case OperationCanceledException _:
                break;
            default:
                Console.WriteLine($"An unhanded exception occured: {e.Message}");

                // write the exception to a file
                File.WriteAllText("exception.txt", e.ToString());

                Console.WriteLine("The exception has been written to exception.txt");

                break;
        }
    }

    #endregion
}
