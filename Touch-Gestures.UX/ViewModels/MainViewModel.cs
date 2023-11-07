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

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class MainViewModel : NavigableViewModel
{
    #region Fields

    private RpcClient<IGesturesDaemon> _client;
    private object? _settings;

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
    private NodeGestureEditorViewModel _nodeGestureEditorViewModel = new();

    [ObservableProperty]
    private GestureSetupWizardViewModel _gestureSetupWizardViewModel = new();

    #endregion

    #region Events

    public event EventHandler<ObservableCollection<SerializablePlugin>>? OnPluginChanged;

    #endregion

    #region Constructors

    // TODO: Implement settings for plugin
    public MainViewModel()
    {
        BackRequested = null!;

        CanGoBack = false;
        NextViewModel = _gestureSetupWizardViewModel;

        NextViewModel!.PropertyChanged += OnCurrentGestureSetupChanged;
        NextViewModel!.PropertyChanging += OnCurrentGestureSetupChanging;
        NextViewModel!.BackRequested += OnBackRequestedAhead;

        _client = new("TouchGestures");

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
    }

    //
    // RPC Methods
    //

    private async Task ConnectRpcAsync()
    {
        if (!_client.IsConnected)
        {
            try
            {
                await _client.ConnectAsync();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
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
        IsConnected = false;
    }

    private async Task OnClientAttached(object? sender, EventArgs e)
    {
        var tempPlugins = await FetchPluginsAsync();

        if (tempPlugins != null)
        {
            Plugins = new ObservableCollection<SerializablePlugin>(tempPlugins);
            OnPluginChanged?.Invoke(this, Plugins);
        }

        object? tempSettings = null;

        if (tempPlugins != null)
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
