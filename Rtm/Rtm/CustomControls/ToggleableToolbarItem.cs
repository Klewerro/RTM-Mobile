using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rtm.CustomControls
{
    public class ToggleableToolbarItem : ToolbarItem
    {
        public bool IsToggled
        {
            get => (bool)GetValue(IsToggledProperty); 
            set => SetValue(IsToggledProperty, value); 
        }

        public ImageSource IconImageSourceToggled
        {
            get => (ImageSource)GetValue(IconImageSourceToggledProperty);
            set => SetValue(IconImageSourceToggledProperty, value);
        }


        public static readonly BindableProperty IsToggledProperty =
          BindableProperty.Create(nameof(IsToggled),
            typeof(bool),
            typeof(ToggleableToolbarItem),
            true,
            propertyChanged: OnIsVisibleChanged);

        public static readonly BindableProperty IconImageSourceToggledProperty
            = BindableProperty.Create(nameof(IconImageSourceToggled),
                typeof(ImageSource),
                typeof(ToggleableToolbarItem));

        private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var item = bindable as ToggleableToolbarItem;

            var newValueBool = (bool)newValue;
            var oldValueBool = (bool)oldValue;
            
            if (!newValueBool && oldValueBool)  //!newValueBool && oldValueBool
            {
                var tempIS = item.IconImageSource;
                item.IconImageSource = item.IconImageSourceToggled;
                item.IconImageSourceToggled = tempIS;
            }

            if (newValueBool && !oldValueBool)  //newValueBool && !oldValueBool
            {
                var tempIS = item.IconImageSource;
                item.IconImageSource = item.IconImageSourceToggled;
                item.IconImageSourceToggled = tempIS;
            }
        }
    }
}
