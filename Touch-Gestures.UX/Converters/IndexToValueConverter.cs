using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace TouchGestures.UX.Converters;

public class IndexToValueConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return new BindingNotification(new ArgumentException("Not enough values provided."), BindingErrorType.Error);

        if (values[1] is not int index)
            return new BindingNotification(new ArgumentException("First value must be an integer."), BindingErrorType.Error);

        if (values[0] is IEnumerable<object> enumerable)
            return enumerable.ElementAtOrDefault(index);

        if (values[0] is object[] array)
            return array[index];

        return new BindingNotification(new ArgumentException("First value must be an enumerable or an array."), BindingErrorType.Error);
    }
}