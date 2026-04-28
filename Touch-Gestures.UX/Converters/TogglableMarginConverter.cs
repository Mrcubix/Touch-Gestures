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
        if (values.Count == 1)
        {
            if (values[0] is double d)
                return new Thickness(d);
            else if (values[0] is Thickness t)
                return t;
        }
        else if (values.Count == 2 && values[1] is bool toggled && toggled)
        {
            if (values[0] is double d1)
                return new Thickness(d1);
            else if (values[0] is string s)
                return Thickness.Parse(s);
            else if (values[0] is Thickness t1)
                return t1;
        }

        return new Thickness(0);
    }
}
