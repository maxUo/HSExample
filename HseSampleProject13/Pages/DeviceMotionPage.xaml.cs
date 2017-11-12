using System;
using System.Collections.Generic;
using HseSampleProject13.ViewModel;
using Xamarin.Forms;

namespace HseSampleProject13.Pages
{
    public partial class DeviceMotionPage : ContentPage
    {
        public DeviceMotionPage()
        {
            InitializeComponent();
            BindingContext = new DeviceMotionViewModel();
        }
    }
}
