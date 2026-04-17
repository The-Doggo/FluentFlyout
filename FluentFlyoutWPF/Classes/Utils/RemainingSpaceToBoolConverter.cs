using System;
using System.Globalization;
using System.Windows.Data;

namespace FluentFlyoutWPF.Classes.Utils
{
    public class RemainingSpaceToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3)
            {
                return false;
            }

            if (values[0] is double contentBorderActualWidth &&
                values[1] is double controlIconActualWidth &&
                values[2] is double contentActualWidth)
            {

                double remainingWidth = contentBorderActualWidth - controlIconActualWidth - contentActualWidth;
                
                return remainingWidth < 220; // This appears to be the threshold in the windows settings
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
