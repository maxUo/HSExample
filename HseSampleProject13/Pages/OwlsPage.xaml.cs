using System;
using System.Collections.Generic;
using HseSampleProject13.ViewModel;
using Xamarin.Forms;

namespace HseSampleProject13.Pages
{
    public partial class OwlsPage : ContentPage
    {
        private static OwlsViewModel VM = new OwlsViewModel();
        public OwlsPage()
        {
            InitializeComponent();
            BindingContext = VM;
        }
    }
}
