using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using OpenTabletDriver.External.Avalonia.ViewModels;
using TouchGestures.UX.Extentions;

namespace TouchGestures.UX.ViewModels.Controls.Setups;

using static AssetLoaderExtensions;

public partial class TapSetupViewModel : GestureSetupViewModel
{
    #region Constructors

    public TapSetupViewModel()
    {
        CanGoBack = true;
        CanGoNext = true;

        GestureSetupPickText = "Number of touches:";
        GestureSetupPickItems = new ObservableCollection<object>(Enumerable.Range(1, 10).Cast<object>());

        Bitmap?[] images = LoadBitmaps(
            "Assets/Setups/Tap/tap_single.png",
            "Assets/Setups/Tap/tap_double.png",
            "Assets/Setups/Tap/tap_triple.png",
            "Assets/Setups/Tap/tap_quadruple.png",
            "Assets/Setups/Tap/tap_quintuple.png",
            "Assets/Setups/Tap/tap_sextuple.png",
            "Assets/Setups/Tap/tap_septuple.png",
            "Assets/Setups/Tap/tap_octuple.png",
            "Assets/Setups/Tap/tap_nonuple.png",
            "Assets/Setups/Tap/tap_décuple.png"
        );

        GestureSetupPickPreviews = new ObservableCollection<Bitmap?>(images);

        SelectedGestureSetupPickIndex = 0;

        BindingDisplay = new BindingDisplayViewModel();
    }

    #endregion

    protected override void GoBack()
    {
        if (IsBindingSelectionStepActive)
            IsBindingSelectionStepActive = false;
        else
            base.GoBack();
    }

    protected override void GoNext()
    {
        if (!IsBindingSelectionStepActive)
        {
            IsBindingSelectionStepActive = true;

            if (BindingDisplay != null)
            {
                var value = GestureSetupPickItems?[SelectedGestureSetupPickIndex];

                BindingDisplay.Description = $"Tap with {value} Touches";
            }
        }
    }

    #region Interface Implementations

    public override void Dispose()
    {
        base.Dispose();

        if (GestureSetupPickPreviews != null)
            foreach (var bitmap in GestureSetupPickPreviews)
                bitmap?.Dispose();
    }

    #endregion
}
