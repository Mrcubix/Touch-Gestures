using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

public partial class GestureSetupViewModel : NavigableViewModel
{
    private static readonly Uri _placeholderImageUri = new("avares://Touch-Gestures.UX/Assets/Displays/placeholder.png");

    [ObservableProperty]
    private string _gestureSetupPickText = string.Empty;

    private int _selectedGestureSetupPickIndex = 0;

    [ObservableProperty]
    private Bitmap? _selectedSetupPickPreview = new(AssetLoader.Open(_placeholderImageUri));

    [ObservableProperty]
    private ObservableCollection<object>? _gestureSetupPickItems;

    [ObservableProperty]
    private ObservableCollection<string>? _gestureSetupPickPreviews;

    public GestureSetupViewModel()
    {
        BackRequested = null!;

        CanGoBack = true;
        CanGoNext = false;

        GestureSetupPickText = "Gesture:";
        GestureSetupPickItems = null;
    }

    public override event EventHandler? BackRequested;

    public bool CanGoNext { get; init; }

    public int SelectedGestureSetupPickIndex
    {
        get => _selectedGestureSetupPickIndex;
        set
        {
            SetProperty(ref _selectedGestureSetupPickIndex, value);

            if (value >= 0 && value < GestureSetupPickPreviews!.Count)
            {
                var path = GestureSetupPickPreviews[value];
                var realPath = new Uri("avares://Touch-Gestures.UX" + path);

                if (AssetLoader.Exists(realPath))
                    SelectedSetupPickPreview = new Bitmap(AssetLoader.Open(realPath));
                else
                    SelectedSetupPickPreview = new Bitmap(AssetLoader.Open(_placeholderImageUri));
            }
            else
                SelectedSetupPickPreview = new Bitmap(AssetLoader.Open(_placeholderImageUri));
        }
    }

    protected override void GoBack() => BackRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    protected virtual void GoNext() => throw new NotImplementedException();
}