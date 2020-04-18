using Microsoft.AppCenter.Analytics;
using System;
using Xamarin.Forms;

namespace MyStop
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage ()
        {
            InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            Analytics.TrackEvent("NAV - About Us");

            if (App.IsNight)
            {
                imgFooter.Source = ImageSource.FromFile("bg_donate_city_night.png");
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                if (App.ScreenWidth == 828 && App.ScreenHeight == 1792 ||  // iPhone XR
                    App.ScreenWidth == 1125 && App.ScreenHeight == 2436 || // iPhone X/XS
                    App.ScreenWidth == 1242 && App.ScreenHeight == 2688)   // iPhone XS Max
                {
                    grdContent.Padding = new Thickness(0, 35, 0, 0);
                }
            }

            string version = DependencyService.Get<IAppVersionAndBuild>().GetVersionNumber();
            string build = "(" + DependencyService.Get<IAppVersionAndBuild>().GetBuildNumber() + ")";

            lblVersion.Text = "Version " + version + " " + build;
        }

        void BtnBackClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            btnBack.Clicked += BtnBackClicked;
        }

        protected override void OnDisappearing()
        {
            btnBack.Clicked -= BtnBackClicked;

            base.OnDisappearing();
        }
    }
}