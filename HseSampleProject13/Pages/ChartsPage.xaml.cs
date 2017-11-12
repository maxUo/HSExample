using System;
using System.Collections.Generic;
using HseSampleProject13.ViewModel;
using Microcharts;
using Xamarin.Forms;

namespace HseSampleProject13.Pages
{
    public partial class ChartsPage : ContentPage
    {
        private static ChartsViewModel VM = new ChartsViewModel();
        public ChartsPage()
        {
            InitializeComponent();
            BindingContext = VM;

            MessagingCenter.Subscribe<Chart>(this, "NewChartAvaliable", (ch) =>
            {
                if (ch != null)
                {
                    ChartViewControl.Chart = ch;
                }
            });

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<Chart>(this,"NewChartAvaliable");
        }
    }
}
