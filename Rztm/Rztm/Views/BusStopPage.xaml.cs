using Rztm.Models;
using Rztm.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rztm.Views
{
    [DesignTimeVisible(false)] 
    public partial class BusStopPage : ContentPage
    {
        private int _counter = 0;
        private List<Label> _labelsToBlink;
        private bool _isBlinking;

        public BusStopPage()
        {
            InitializeComponent();
            _labelsToBlink = new List<Label>(10);
        }

        private void RenameToolbarItem_Clicked(object sender, EventArgs e)
        {
            CustomNameGrid.IsVisible = true;
        }

        private void NameButton_Clicked(object sender, EventArgs e)
        {
            CustomNameGrid.IsVisible = false;
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

    }
}