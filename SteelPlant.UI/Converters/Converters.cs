using Avalonia.Data.Converters;
using Avalonia.Media;
using SteelPlant.Domain;
using System;
using System.Globalization;

namespace SteelPlant.UI.Converters;

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool b && !b);
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class FurnaceStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FurnaceStatus status)
        {
            return value switch
            {
                FurnaceStatus.Overheated => Brushes.Red,
                _ => Brushes.Green,
            };
        }
        return Brushes.Gray;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
