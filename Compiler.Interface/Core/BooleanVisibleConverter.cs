using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Compiler.Interface;

public class BooleanVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool val = (bool)value;
        if (val == true)
            return Visibility.Hidden;
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
