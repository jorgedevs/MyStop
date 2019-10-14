using MyStop.Models;
using MyStop.ViewModels;
using MyStop.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyStop
{
	public partial class MainPage : ContentPage
	{
        int animationLength = 60;
        bool isAnimating;
        bool keepTicking;

        public MainPage ()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = new MainViewModel();

            if (App.IsNight)
            {
                imgLamp.Source = ImageSource.FromFile("img_lamp_night.png");
                imgFooter.Source = ImageSource.FromFile("bg_home_night.png");
                imgBusFront.Source = ImageSource.FromFile("img_bus_front_night.png");
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

            //if (App.ShowWhatsNew)
            //viewWhatsNew.IsVisible = true;            
        }

        public bool Tick()
        {
            Animate();

            return true;
        }

        async Task Animate()
        {
            if (imgCloud1 == null || imgCloud2 == null || imgCloud3 == null)
                return;

            Task.Run(async() => 
            {
                await imgCloud1?.TranslateTo(500, 0, (uint)(animationLength / 3) * 1000, null);
                await imgCloud1?.TranslateTo(-200, 0, 0, null);
                await Task.Delay((animationLength / 3) * 1000);
                await Task.Delay((animationLength / 3) * 1000);
            });

            Task.Run(async () =>
            {
                await Task.Delay((animationLength / 3) * 1000);
                await imgCloud2?.TranslateTo(500, 0, (uint)(animationLength / 3) * 1000, null);
                await imgCloud2?.TranslateTo(-200, 0, 0, null);
                await Task.Delay((animationLength / 3) * 1000);
            });

            Task.Run(async () =>
            {
                await Task.Delay((animationLength / 3) * 1000);
                await Task.Delay((animationLength / 3) * 1000);
                await imgCloud3?.TranslateTo(500, 0, (uint)(animationLength / 3) * 1000, null);
                await imgCloud3?.TranslateTo(-200, 0, 0, null);
            });

            Task.Run(async () =>
            {
                for (int i = 0; i < animationLength; i++)
                {
                    imgLamp?.TranslateTo(0, 0, 0, null);
                    await imgLamp?.ScaleTo(1, 0, null);

                    imgLamp?.TranslateTo(-80, 0, 2000, null);
                    await imgLamp?.ScaleTo(0.55, 2000, null);
                }
            });

            Task.Run(async () =>
            {
                for(int i = 0; i < animationLength; i++)
                {
                    if(i % 2 == 0)
                        await imgBusFront?.TranslateTo(0, 0, 1000, null);
                    else
                        await imgBusFront?.TranslateTo(-5, 0, 1000, null);
                }
            });
        }

        async void BtnMyStopsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FavouriteStopsPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            btnMyStops.Clicked += BtnMyStopsClicked;

            MessagingCenter.Subscribe<WhatsNewView>(this, "CLOSE_VIEW", (obj) =>
            {
                viewWhatsNew.IsVisible = false;
                App.ShowWhatsNew = false;
            });
            MessagingCenter.Subscribe<MainViewModel>(this, MainViewModel.INVALID_CODE_ENTERED, async (sender) =>
            {
                await DisplayAlert("Error", "Invalid code entered, try again?", "Ok");
            });
            MessagingCenter.Subscribe<MainViewModel>(this, MainViewModel.STOP_NOT_FOUND, async (sender) =>
            {
                await DisplayAlert("Alert", "Bus stop not found, try again", "Ok");
            });
            MessagingCenter.Subscribe<MainViewModel, Stop>(this, MainViewModel.STOP_FOUND, async (sender, arg) =>
            {
                await Navigation.PushAsync(new BusArrivalsPage(arg), true);
            });
        }

        protected override void OnDisappearing()
        {
            btnMyStops.Clicked -= BtnMyStopsClicked;

            MessagingCenter.Unsubscribe<WhatsNewView>(this, "CLOSE_VIEW");
            MessagingCenter.Unsubscribe<MainViewModel>(this, MainViewModel.INVALID_CODE_ENTERED);
            MessagingCenter.Unsubscribe<MainViewModel>(this, MainViewModel.STOP_NOT_FOUND);
            MessagingCenter.Unsubscribe<MainViewModel, Stop>(this, MainViewModel.STOP_FOUND);
            
            base.OnDisappearing();
        }
    }
}
