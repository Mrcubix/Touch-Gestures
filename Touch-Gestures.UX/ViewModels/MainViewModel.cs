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
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Entities;
using TouchGestures.Lib.Serializables.Gestures;
using System.Threading;
using TouchGestures.UX.Events;

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class MainViewModel : NavigableViewModel
{
    #region Fields

    private RpcClient<IGesturesDaemon> _client;
    private SerializableSettings _settings;
    private CancellationTokenSource _reconnectionTokenSource = new();

    // Home Menu

    // Bindings Overview
      // Add Button
        // -> GestureSetupWizard
          // Gesture Selection
          // Gesture Trigger Selection
          // Gesture Action Selection
            // Add Button

    // Settings

    [ObservableProperty]
    private string _connectionStateText = "Not connected";

    [ObservableProperty]
    private bool _isConnected = false;

    [ObservableProperty]
    private ObservableCollection<SerializablePlugin> _plugins = new();

    [ObservableProperty]
    private BindingsOverviewViewModel _bindingsOverviewViewModel;

    #region Testing VMs

    [ObservableProperty]
    private NodeGestureEditorViewModel _nodeGestureEditorViewModel = new();

    [ObservableProperty]
    private GestureSetupWizardViewModel _gestureSetupWizardViewModel = new();

    #endregion

    #endregion

    #region Events

    public event EventHandler<ObservableCollection<SerializablePlugin>>? OnPluginChanged;

    public event EventHandler? Connected;

    public event EventHandler? Disconnected;

    #endregion

    #region Constructors

    // TODO: Implement settings for plugin
    public MainViewModel()
    {
        _settings = new();
        _bindingsOverviewViewModel = new(this);

        BackRequested = null!;

        CanGoBack = false;
        // TODO: Change in production to the home view
        //NextViewModel = _gestureSetupWizardViewModel;
        NextViewModel = _bindingsOverviewViewModel;

        NextViewModel!.PropertyChanged += OnCurrentGestureSetupChanged;
        NextViewModel!.PropertyChanging += OnCurrentGestureSetupChanging;
        NextViewModel!.BackRequested += OnBackRequestedAhead;

        BindingsOverviewViewModel.GestureAdded += OnGestureAdded;
        BindingsOverviewViewModel.GesturesChanged += OnGesturesChanged;

        _client = new("GesturesDaemon");

        InitializeClient();
    }

    private void InitializeClient()
    {
        _client.Connected += OnClientConnected;
        _client.Connecting += OnClientConnecting;
        _client.Disconnected += OnClientDisconnected;
        _client.Attached += (sender, args) => Task.Run(() => OnClientAttached(sender, args));

        _ = Task.Run(ConnectRpcAsync);
    }

    #endregion

    #region Events

    public override event EventHandler? BackRequested;

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

        while(!_client.IsConnected && !token.IsCancellationRequested)
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

    private async Task UploadSettingsAsync()
    {
        if (!_client.IsConnected)
            return;

        try
        {
            await _client.Instance.UpdateSettings(_settings);
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
        Connected?.Invoke(this, EventArgs.Empty);
    }

    private void OnClientConnecting(object? sender, EventArgs e)
    {
        ConnectionStateText = "Connecting...";
    }

    private void OnClientDisconnected(object? sender, EventArgs e)
    {
        ConnectionStateText = "Disconnected";
        IsConnected = false;
        Disconnected?.Invoke(this, EventArgs.Empty);

        _ = Task.Run(() => AttemptReconnectionIndefinitelyAsync());
    }

    private async Task OnClientAttached(object? sender, EventArgs e)
    {
        var tempPlugins = await FetchPluginsAsync();

        if (tempPlugins != null)
        {
            Plugins = new ObservableCollection<SerializablePlugin>(tempPlugins);
            OnPluginChanged?.Invoke(this, Plugins);
        }

        SerializableSettings? tempSettings = null;

        if (tempSettings != null)
        {
            _settings = tempSettings;
        }

        IsConnected = true;
    }

    //
    // UX Event Handlers
    //

    private void OnCurrentGestureSetupChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (NextViewModel != null)
            NextViewModel.BackRequested -= OnBackRequestedAhead;
    }

    private void OnCurrentGestureSetupChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (NextViewModel != null)
            NextViewModel.BackRequested += OnBackRequestedAhead;
    }

    private void OnBackRequestedAhead(object? sender, EventArgs e)
    {
        NextViewModel = this;
    }

    private void OnGestureAdded(object? sender, Gesture e)
    {
        switch (e)
        {
            case SerializableTapGesture tapGesture:
                _settings.TapGestures.Add(tapGesture);
                break;
            case SerializableSwipeGesture swipeGesture:
                _settings.SwipeGestures.Add(swipeGesture);
                break;
            default:
                throw new NotImplementedException();
        }

        _ = UploadSettingsAsync();
    }

    private void OnGesturesChanged(object? sender, GestureChangedEventArgs e)
    {
        int index;

        // TODO: Rewrite this garbage somehow
        switch(e.OldValue)
        {
            case SerializableTapGesture tapGesture:
                index = _settings.TapGestures.IndexOf(tapGesture);
                _settings.TapGestures[index] = (SerializableTapGesture)e.NewValue;
                break;
            case SerializableSwipeGesture swipeGesture:
                index = _settings.SwipeGestures.IndexOf(swipeGesture);
                _settings.SwipeGestures[index] = (SerializableSwipeGesture)e.NewValue;
                break;
            default:
                throw new NotImplementedException();
        }

        _ = UploadSettingsAsync();
    }

    #endregion

    #region Exception Handling

    private void HandleException(Exception e)
    {
        switch(e)
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
