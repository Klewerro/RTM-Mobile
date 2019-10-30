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
        public BusStopPage()
        {
            InitializeComponent();
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
    }
}