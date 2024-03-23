using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TouchGestures.UX.Attributes;
using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.ViewModels.Controls.Tiles
{

using static TouchGestures.UX.Extentions.AssetLoaderExtensions;

#nullable enable

    [Name("Non-Implemented")]
    [Description("This gesture has not been implemented yet")]
    [Icon("")]
    public partial class GestureTileViewModel<T> : GestureTileViewModel where T : GestureSetupViewModel, new()
    {
        private const string DEFAULT_NAME = "Non-Implemented";
        private const string DEFAULT_DESCRIPTION = "This gesture has not been implemented yet";

        public GestureTileViewModel()
        {
            var nameAttribute = (NameAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(NameAttribute));
            var descriptionAttribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(DescriptionAttribute));
            var iconAttribute = (IconAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(IconAttribute));

            GestureName = nameAttribute?.Name ?? DEFAULT_NAME;
            Description = descriptionAttribute?.Description ?? DEFAULT_DESCRIPTION;
            Icon = iconAttribute?.Icon;

            AssociatedSetup = new T();
        }
    }

    public partial class GestureTileViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _gestureName = "[Insert Gesture Name Here]";

        [ObservableProperty]
        private string _description = "[Insert Gesture Description Here]";

        [ObservableProperty]
        private IImage? _icon = null;
        
        [ObservableProperty]
        private GestureSetupViewModel _associatedSetup = new();

        public event EventHandler? Selected;

        [RelayCommand]
        protected virtual void SelectGesture() => Selected?.Invoke(this, EventArgs.Empty);
    }
}