using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rztm.Helpers
{
    public class BusStopDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NameTemplate { get; set; }
        public DataTemplate CustomNameTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var busStop = item as BusStop;
            return string.IsNullOrEmpty(busStop.CustomName) ? NameTemplate : CustomNameTemplate;
        }
    }
}
