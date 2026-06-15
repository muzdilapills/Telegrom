using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TelegromV4.Converters;

public class ErrorColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool hasError && hasError)
            return Brushes.Red;
        return Brushes.Green;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}