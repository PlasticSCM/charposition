using System;
using System.Globalization;
using System.Windows.Data;

namespace charposition.Converters;

public class SizeAdjustmentConverter : IValueConverter
{
    public double Adjustment { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is double size ? size + Adjustment : value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is double size ? size - Adjustment : value;
}
