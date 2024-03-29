using System;

namespace TouchGestures.UX.ViewModels.Dialogs;

#nullable enable

public class TwoChoiceDialogViewModel : ViewModelBase
{
    #region Events

    public event EventHandler<bool>? ResultPicked;

    #endregion

    #region Properties

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string PositiveChoice { get; set; } = string.Empty;

    public string NegativeChoice { get; set; } = string.Empty;

    #endregion

    public void ReturnResult(bool result)
        => ResultPicked?.Invoke(this, result);
}