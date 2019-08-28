using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rztm.Converters
{
    public class IsListEmptyConverter : IValueConverter/*, IMarkupExtension*/
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as IEnumerable<object>;

            return list.Any() ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
