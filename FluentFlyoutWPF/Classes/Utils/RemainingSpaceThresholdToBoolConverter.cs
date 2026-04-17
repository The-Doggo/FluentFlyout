using System;
using System.Globalization;
using System.Windows.Data;

namespace FluentFlyoutWPF.Classes.Utils
{
    public class RemainingSpaceThresholdToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4)
            {
                return false;
            }

            if (values[0] is double contentBorderActualWidth &&
                values[1] is double controlIconActualWidth &&
                values[2] is double contentActualWidth &&
                values[2] is double threshold)
            {

                double remainingWidth = contentBorderActualWidth - controlIconActualWidth - contentActualWidth;
                
                return remainingWidth < threshold;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
