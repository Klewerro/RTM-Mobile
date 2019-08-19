using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Rtm.Converters
{
    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var distance = (double)value;
            if (distance < 1)
                return $"{(distance * 1000).ToString("F0")}m";

            return distance.ToString("0.00km"); ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string)value;
            var number = text.Contains("km") ? double.Parse(text.Substring(text.Length - 2)) : (double.Parse(text.Substring(text.Length - 1)) / 1000);
            return number;
        }
    }
}
