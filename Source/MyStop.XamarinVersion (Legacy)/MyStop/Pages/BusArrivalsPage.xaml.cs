using Microsoft.AppCenter.Analytics;
using MyStop.Models;
using MyStop.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyStop
{
    public partial class BusArrivalsPage : ContentPage
    {
        Random randomBusModel;
        BusArrivalsViewModel vm;
        bool _keepTicking;

        public BusArrivalsPage()
        {
            InitializeComponent();
            BindingContext = new BusArrivalsViewModel();
            Analytics.TrackEvent("NAV - Bus Arrivals");

            if (App.IsNight)
            {
                imgFooter.Source = ImageSource.FromFile("bg_arrivals_night.png");
                imgBus.Source = ImageSource.FromFile("img_bus_side_night.png");
                imgTopList.Source = ImageSource.FromFile("img_gradient_top_night.png");
                imgBottomList.Source = ImageSource.FromFile("img_gradient_bottom_night.png");
            }
        }

        public BusArrivalsPage(Stop _stop)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = vm = new BusArrivalsViewModel(_stop);

            randomBusModel = new Random();

            if (App.IsNight)
            {
                imgFooter.Source = ImageSource.FromFile("bg_arrivals_night.png");
                imgBus.Source = ImageSource.FromFile("img_bus_side_long_night.png");
                imgTopList.Source = ImageSource.FromFile("img_gradient_top_night.png");
                imgBottomList.Source = ImageSource.FromFile("img_gradient_bottom_night.png");
            }

            grdContent.Padding = new Thickness(0, Device.RuntimePlatform == Device.Android ? App.StatusBarHeight + 5 : 40, 0, 0);

            Animate();
        }

        #region Arrival Times Loop
        public bool Tick()
        {
            if (_keepTicking)
            { 
                vm.GetData();
                Animate();
            }
            return _keepTicking;
        }

        public async Task Animate()
        {
            if (imgBus == null)
                return;

            imgBus.IsVisible = true;
            imgBus.TranslationX = this.Width + 10;
            await imgBus?.TranslateTo(0, 0, 3500, Easing.CubicOut);
            await imgBus?.TranslateTo(0, 0, 2000, null);
            await imgBus?.TranslateTo(-(this.Width + 10), 0, 3500, Easing.CubicIn);

            string busImageSource = "";
            busImageSource = (randomBusModel.Next(2) == 0)? "img_bus_side" : "img_bus_side_long";
            if (App.IsNight)
                busImageSource += "_night";

            imgBus.Source = ImageSource.FromFile(busImageSource);
        }
        #endregion

        #region Event Handlers
        void BtnBackClicked(object sender, EventArgs e)
        {
            imgBus.IsVisible = false;
            Navigation.PopAsync();
        }

        void ListScheduleItemTapped(object sender, ItemTappedEventArgs e)
        {
            listSchedule.SelectedItem = null;

            vm.Schedule = (Schedule) e.Item;
            vm.IsBusAlertVisible = true;
        }

        async void ListScheduleRefreshing(object sender, EventArgs e)
        {
            await vm.GetData();
            listSchedule.IsRefreshing = false;
        }
        #endregion

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.StartTimer(new TimeSpan(0, 0, 15), Tick);
            _keepTicking = true;

            btnBack.Clicked += BtnBackClicked;
            listSchedule.ItemTapped += ListScheduleItemTapped;
            listSchedule.Refreshing += ListScheduleRefreshing;
        }

        protected override void OnDisappearing()
        {
            _keepTicking = false;

            btnBack.Clicked -= BtnBackClicked;
            listSchedule.ItemTapped -= ListScheduleItemTapped;
            listSchedule.Refreshing -= ListScheduleRefreshing;            

            base.OnDisappearing();
        }
    }
}