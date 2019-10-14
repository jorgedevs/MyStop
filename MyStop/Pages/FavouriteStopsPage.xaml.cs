using MyStop.Models;
using System;
using Xamarin.Forms;
using MyStop.ViewModels;
using Microsoft.AppCenter.Analytics;
using System.Threading.Tasks;

namespace MyStop
{
    public partial class FavouriteStopsPage : ContentPage
    {
        int animationLength = 15;
        FavouritesStopsViewModel vm;

        public FavouriteStopsPage ()
        {
            InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            Analytics.TrackEvent("NAV - Favourites");
            BindingContext = vm = new FavouritesStopsViewModel();

            if (App.IsNight)
            {
                imgBus.Source = ImageSource.FromFile("img_bus_side_night.png");
                imgFooter.Source = ImageSource.FromFile("bg_terminal_night.png");
                imgTopList.Source = ImageSource.FromFile("img_gradient_top_night.png");
                imgBottomList.Source = ImageSource.FromFile("img_gradient_bottom_night.png");
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

            Device.StartTimer(new TimeSpan(0, 0, animationLength), Tick);
            Animate();
        }

        public bool Tick()
        {
            Animate();

            return true;
        }

        async Task Animate()
        {            
            if (imgBus == null)
                return;

            Random randomBusModel = new Random();
            imgBus.IsVisible = true;
            imgBus.TranslationX = this.Width + 10;
            await imgBus?.TranslateTo(0, 0, 3500, Easing.CubicOut);
            await imgBus?.TranslateTo(0, 0, 2000, null);
            await imgBus?.TranslateTo(-(this.Width + 10), 0, 3500, Easing.CubicIn);

            string busImageSource = "";
            busImageSource = (randomBusModel.Next(2) == 0) ? "img_bus_side" : "img_bus_side_long";
            if (App.IsNight)
                busImageSource += "_night";

            imgBus.Source = ImageSource.FromFile(busImageSource);
        }

        void BtnBackClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        async void BtnDonateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AboutPage());
        }

        async void ListStopsItemTapped(object sender, ItemTappedEventArgs e)
        {
            listStops.SelectedItem = null;

            if (e.Item != null)
            {
                if (((FavouriteStop)e.Item).EditMode)
                {
                    vm.StopNumber = ((FavouriteStop)e.Item).StopNo;
                    vm.TagName = ((FavouriteStop)e.Item).Tag;
                    vm.IsEditVisible = true;
                }
                else
                {
                    await Navigation.PushAsync(new BusArrivalsPage(e.Item as Stop));
                }
            }
        }

        protected override void OnAppearing ()   
        {
            base.OnAppearing ();

            btnBack.Clicked += BtnBackClicked;
            btnDonate.Clicked += BtnDonateClicked;
            listStops.ItemTapped += ListStopsItemTapped;
        }

        protected override void OnDisappearing()
        {
            btnBack.Clicked -= BtnBackClicked;
            btnDonate.Clicked -= BtnDonateClicked;
            listStops.ItemTapped -= ListStopsItemTapped;

            listStops.SelectedItem = null;
            base.OnDisappearing();
        }

        ~FavouriteStopsPage()
        {
            if (vm != null)
                vm.Dispose();
        }
    }
}