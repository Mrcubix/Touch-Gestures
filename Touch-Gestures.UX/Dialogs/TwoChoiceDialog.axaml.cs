using System;
using Avalonia.Controls;
using TouchGestures.UX.ViewModels.Dialogs;

namespace TouchGestures.UX.Views;

public partial class TwoChoiceDialog : Window
{
    public TwoChoiceDialog()
    {
        InitializeComponent();
    }

    protected override void OnDataContextBeginUpdate()
    {
        base.OnDataContextBeginUpdate();

        if (DataContext is TwoChoiceDialogViewModel viewModel)
        {
            viewModel.ResultPicked -= OnResultPicked;
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is TwoChoiceDialogViewModel viewModel)
        {
            viewModel.ResultPicked += OnResultPicked;
        }
    }

    private void OnResultPicked(object? sender, bool e)
    {
        Close(e);
    }
}