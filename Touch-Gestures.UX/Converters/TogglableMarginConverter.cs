using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace TouchGestures.UX.Converters;

public class TogglableMarginConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 1 && values[0] is double d)
            return new Thickness(d);

        if (values.Count == 1 && values[0] is Thickness t)
            return t;

        if (values.Count == 2 && values[0] is double d1 && values[1] is bool toggled)
            return new Thickness(d1 * (toggled ? 1 : 0));

        if (values.Count == 2 && values[0] is Thickness t1 && values[1] is bool toggled1)
            if (toggled1)
                return t1;

        if (values.Count == 2 && values[0] is string thickness && values[1] is bool toggled2)
            if (toggled2)
                return Thickness.Parse(thickness);

        return new Thickness(0);
    }
}
