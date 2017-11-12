using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HseSampleProject13
{
    public partial class GeolocationPage : ContentPage
    {
        public GeolocationPage()
        {
            InitializeComponent();

            BindingContext = new GelocationViewModel();

            MessagingCenter.Subscribe<string>(this, "LocationAlert", async (string obj) =>
            {
                await DisplayAlert("Need location", "Gunna need that location", "OK");
            });

            MessagingCenter.Subscribe<string>(this, "LocationDenied", async (string obj) =>
            {
                await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
            });
        }
    }
}
