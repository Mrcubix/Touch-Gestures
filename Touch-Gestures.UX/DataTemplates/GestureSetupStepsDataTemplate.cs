using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using TouchGestures.UX.ViewModels.Controls.Setups;

namespace TouchGestures.UX.DataTemplates;

public class GestureSetupStepsDataTemplate : IDataTemplate
{
    // TODO : Move to resx file
    public const string UNSUPPORTED_PARAM = "Provided parameter is not a GestureSetupViewModel";
    public const string UNSUPPORTED_PROPERTY_TYPE = "Index is out of range";

    [Content]
    public List<IDataTemplate> PropertyTemplates { get; } = [];

    public Control? Build(object? param)
    {
        if (param is not GestureSetupViewModel gestureSetup)
            return new TextBlock { Text = UNSUPPORTED_PARAM };

        if (gestureSetup.CurrentStep >= 0 && gestureSetup.CurrentStep < PropertyTemplates.Count)
            return PropertyTemplates[gestureSetup.CurrentStep].Build(null);

        return new TextBlock { Text = UNSUPPORTED_PROPERTY_TYPE };
    }

    public bool Match(object? data)
        => data is GestureSetupViewModel;
}