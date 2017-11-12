using System;
using Xamarin.Forms;
using PropertyChanged;
using HseSampleProject13.Pages;

namespace HseSampleProject13
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void GeoNavigate(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GeolocationPage());
        }

        async void Motion(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeviceMotionPage());
        }

        async void Charts(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChartsPage());
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {

    }
}
