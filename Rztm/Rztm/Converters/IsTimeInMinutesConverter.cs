using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Rztm.Converters
{
    public class IsTimeInMinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var distanceText = (string)value;
            if (string.IsNullOrEmpty(distanceText))
                return false;

            return distanceText.Contains("min") ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Conver back is not implemented yet!");
        }
    }
}
