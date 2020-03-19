using Rg.Plugins.Popup.Pages;
using Rztm.DependencyInterfaces;
using Rztm.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rztm.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BusStopPopupPage : PopupPage
    {
        private readonly ITextMeter _textMeter;

        private int _counter = 0;
        private List<Label> _labelsToBlink;
        private bool _isBlinking;

        public BusStopPopupPage()
        {
            InitializeComponent();

            _labelsToBlink = new List<Label>(10);
            _textMeter = DependencyService.Get<ITextMeter>();
        }

        private async void ArrowButton_Clicked(object sender, EventArgs e)
        {
            if (DescriptionLabel.Text.Length > 0)
            {
                DescriptionStackLayout.IsVisible = !DescriptionStackLayout.IsVisible;
            }
            Carousel.IsVisible = !Carousel.IsVisible;

            if (ArrowButton.Rotation == 0)
            {
                await ArrowButton.RotateTo(180, easing: Easing.SinInOut);
            }
            else
            {
                await ArrowButton.RotateTo(0, easing: Easing.SinInOut);
            }
        }

        private async void TimeLabel_BindingContextChanged(object sender, EventArgs e)
        {
            _counter++;
            var label = sender as Label;

            if (_counter % 11 == 0)
            {
                _labelsToBlink.Clear();
            }

            if (label.Text == null)
            {
                _counter = 11;
                return;
            }


            if (label.Text != null && label.Text.Contains("<"))
            {
                _labelsToBlink.Add(label);
            }

            //10 means last time label in view checked (first iteration) -> start task
            if (_counter >= 10 && !_isBlinking && _labelsToBlink.Count > 0)
            {
                _isBlinking = true;
                await BlinkLabels(_labelsToBlink, _labelsToBlink[0].TextColor, 1000);
            }
        }

        private void CollectionView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var labelWidth = 120;
            var propName = e.PropertyName;

            if (propName.Equals("ItemsSource"))
            {
                var cv = sender as CollectionView;
                if (cv.ItemsSource == null)
                    return;

                var departuresNames = cv.ItemsSource as List<string>;
                if (departuresNames.Count > 0)
                {
                    var height = GetCellHeight(departuresNames, labelWidth);
                    if (height > cv.HeightRequest)
                        cv.HeightRequest = height;
                }
            }
        }

        private async Task BlinkLabels(List<Label> labels, Color initialColor, int delayTime)
        {
            while (true)
            {
                await Task.Delay(delayTime);
                for (int i = 0; i < labels.Count; i++)
                {
                    labels[i].TextColor = labels[i].TextColor == initialColor ? Color.Transparent : initialColor;
                }
            }
        }

        private double GetCellHeight(IEnumerable<string> departures, int labelWitdh)
        {
            double result = 0.0;
            double frameMarginBottom = 10.0;
            double framePadding = 10.0;     //top+bottom

            //Get highest label height
            foreach (var departure in departures)
            {
                var height = _textMeter.MeasureTextSize(departure, labelWitdh,
                    Device.GetNamedSize(NamedSize.Default, typeof(Label)));
                if (height > result)
                    result = height;
            }

            return result + frameMarginBottom + framePadding;
        }

    }
}