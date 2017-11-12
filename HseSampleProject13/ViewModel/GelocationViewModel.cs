using System;
using System.Windows.Input;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using PropertyChanged;
using Xamarin.Forms;

namespace HseSampleProject13
{
    [AddINotifyPropertyChangedInterface]
    public class GelocationViewModel
    {
        public GelocationViewModel()
        {
            GeolocationTestCommand = new Command(GetGeolocation);
            GeolocationField = "Информация";
        }

        public string GeolocationField { get; set; }

        public ICommand GeolocationTestCommand { get; set; }

        private async void GetGeolocation()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        MessagingCenter.Send(this, "LocationAlert");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    var results = await CrossGeolocator.Current.GetPositionAsync();
                    GeolocationField = "Lat: " + results.Latitude + " Long: " + results.Longitude;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                    MessagingCenter.Send(this, "LocationDenied");
                }
            }
            catch (Exception ex)
            {

                GeolocationField = "Error: " + ex;
            }
        }
    }
}
