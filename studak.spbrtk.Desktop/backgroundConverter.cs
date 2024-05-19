using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace studak.spbrtk.Desktop;

public class backgroundConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values?.Count == 1 && values[0] is bool Isactice)
        {
            return Isactice ? new SolidColorBrush(Color.Parse("#E2E2FF")) : new SolidColorBrush(Color.Parse("#F5F5FF"));
        }

        return AvaloniaProperty.UnsetValue;
    }
}