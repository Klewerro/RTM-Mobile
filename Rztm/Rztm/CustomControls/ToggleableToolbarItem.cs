using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rztm.CustomControls
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

        public String TextToggled
        {
            get => (string)GetValue(TextToggledProperty);
            set => SetValue(TextToggledProperty, value);
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

        public static readonly BindableProperty TextToggledProperty
            = BindableProperty.Create(nameof(TextToggled),
                typeof(string),
                typeof(ToggleableToolbarItem));

       
        protected override void OnParentSet()
        {
            this.Order = Device.RuntimePlatform == Device.Android
                ? ToolbarItemOrder.Primary : ToolbarItemOrder.Secondary;

            if (Device.RuntimePlatform == Device.iOS)
            {
                this.IconImageSource = null;
                this.IconImageSourceToggled = null;
            }
        }

        private static void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var item = bindable as ToggleableToolbarItem;

            var newValueBool = (bool)newValue;
            var oldValueBool = (bool)oldValue;

            if (!newValueBool && oldValueBool)
            {
                ToogleProperties(item);
            }

            if (newValueBool && !oldValueBool)
            {
                ToogleProperties(item);
            }
        }

        private static void ToogleProperties(ToggleableToolbarItem item)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                var tempIS = item.IconImageSource;
                item.IconImageSource = item.IconImageSourceToggled;
                item.IconImageSourceToggled = tempIS;
            }

            var tempText = item.Text;
            item.Text = item.TextToggled;
            item.TextToggled = tempText;
        }
    }
}
