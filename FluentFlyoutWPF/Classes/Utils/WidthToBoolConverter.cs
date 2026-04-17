using System;
using System.Globalization;
using System.Windows.Data;

namespace FluentFlyoutWPF.Classes.Utils
{
    public class WidthToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
                return width < 600;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}