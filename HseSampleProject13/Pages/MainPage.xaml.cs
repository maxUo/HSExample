using System;
using Xamarin.Forms;
using PropertyChanged;
using HseSampleProject13.Pages;

namespace HseSampleProject13
{
    public partial class MainPage : ContentPage
    {
        private static MainViewModel VM = new MainViewModel();

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

        async void Owls(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OwlsPage());
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {

    }
}
