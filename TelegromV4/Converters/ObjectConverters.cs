using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TelegromV4.Converters;

public static class ObjectConverters
{
    public static readonly IValueConverter IsEqual = new IsEqualConverter();
}

public class IsEqualConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null && parameter == null) return true;
        if (value == null || parameter == null) return false;
        return value.ToString() == parameter.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}