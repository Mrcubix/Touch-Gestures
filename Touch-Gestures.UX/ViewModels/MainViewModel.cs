﻿using System;
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
using Avalonia.Threading;
using System.Numerics;
using Avalonia;
using Newtonsoft.Json;
using TouchGestures.Lib.Converters;
using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels;

#nullable enable

public partial class MainViewModel : NavigableViewModel
{
    #region Fields

    private static readonly List<JsonConverter> Converters = new() { new SharedAreaConverter() };

    private CancellationTokenSource _reconnectionTokenSource = new();
    private RpcClient<IGesturesDaemon> _client;
    private SerializableSettings _settings;
    private Rect _tabletRect;
    private Vector2 _LinesPerMM = Vector2.Zero;
    
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
        BindingsOverviewViewModel.GesturesChanged += OnGestureChanged;

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

    //
    // Adding / Changing / Removing Gestures
    //

    private void AddGesture(Gesture gesture)
    {
        switch (gesture)
        {
            case SerializableTapGesture tapGesture:
                _settings.TapGestures.Add(tapGesture);
                break;
            case SerializableHoldGesture holdGesture:
                _settings.HoldGestures.Add(holdGesture);
                break;
            case SerializableSwipeGesture swipeGesture:
                _settings.SwipeGestures.Add(swipeGesture);
                break;
            case SerializablePanGesture panGesture:
                _settings.PanGestures.Add(panGesture);
                break;
            case SerializablePinchGesture pinchGesture:
                AddPinchGesture(pinchGesture);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void RemoveGesture(Gesture gesture)
    {
        switch (gesture)
        {
            case SerializableTapGesture tapGesture:
                _settings.TapGestures.Remove(tapGesture);
                break;
            case SerializableHoldGesture holdGesture:
                _settings.HoldGestures.Remove(holdGesture);
                break;
            case SerializableSwipeGesture swipeGesture:
                _settings.SwipeGestures.Remove(swipeGesture);
                break;
            case SerializablePanGesture panGesture:
                _settings.PanGestures.Remove(panGesture);
                break;
            case SerializablePinchGesture pinchGesture:
                RemovePinchGesture(pinchGesture);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void AddPinchGesture(SerializablePinchGesture pinchGesture)
    {
        if (pinchGesture.DistanceThreshold > 0)
            _settings.PinchGestures.Add(pinchGesture);
        else
            _settings.RotateGestures.Add(pinchGesture);
    }

    private void RemovePinchGesture(SerializablePinchGesture pinchGesture)
    {
        if (pinchGesture.DistanceThreshold > 0)
            _settings.PinchGestures.Remove(pinchGesture);
        else
            _settings.RotateGestures.Remove(pinchGesture);
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

        _ = Task.Run(() => AttemptReconnectionIndefinitelyAsync());
    }

    private async Task OnClientAttached(object? sender, EventArgs e)
    {
        ConnectionStateText = "Connected, Fetching Plugins & Settings ...";

        var tempPlugins = await FetchPluginsAsync();

        if (tempPlugins != null)
        {
            Plugins = new ObservableCollection<SerializablePlugin>(tempPlugins);
            PluginChanged?.Invoke(this, Plugins);
        }

        SerializableSettings? tempSettings = await FetchSettingsAsync();

        if (tempSettings != null)
        {
            _settings = tempSettings;
            Dispatcher.UIThread.Post(() => OnSettingsChanged(_settings));
        }

        if (await _client.Instance.IsTabletConnected())
        {
            Vector2? tempLinesPerMM = await _client.Instance.GetTabletLinesPerMM();

            if (tempLinesPerMM != null)
            {
                _LinesPerMM = tempLinesPerMM.Value;
            }

            Vector2? tempTabletSize = await _client.Instance.GetTabletSize();

            if (tempTabletSize != null)
            {
                double x = Math.Round(tempTabletSize.Value.X, 5), y = Math.Round(tempTabletSize.Value.Y, 5);
                _tabletRect = new Rect(0, 0, x, y);
                Dispatcher.UIThread.Invoke(() => OnTabletRectChanged(_tabletRect, _LinesPerMM));
            }
        }

        IsReady = true;
        Ready?.Invoke(this, EventArgs.Empty);
        NextViewModel = BindingsOverviewViewModel;
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

    private void OnTabletRectChanged(Rect bounds, Vector2 linesPerMM)
    {
        // TODO: set it in the appropriates Node Canvases

        // TODO: set it in the GestureSetupWizard
        BindingsOverviewViewModel.Bounds = bounds;
    }

    //
    // Handling whenever gestures are added / changed / deleted
    //

    private void OnGestureChanged(object? sender, GestureChangedEventArgs e)
    {
        if (e.OldValue == null && e.NewValue != null)
            AddGesture(e.NewValue);
        else if (e.OldValue != null && e.NewValue == null)
            RemoveGesture(e.OldValue);

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
