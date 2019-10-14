using System;
using Xamarin.Forms;
using Plugin.Connectivity.Abstractions;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Diagnostics;
using Plugin.Settings.Abstractions;
using Plugin.Settings;
using MyStop.Common.Helpers;

namespace MyStop
{
	public partial class App : Application
	{
        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
        public static int AndroidIcon { get; set; }
        public static bool IsNight { get; set; }
        public static bool DesignTime { get; set; }

        public static StopManager StopMan { get; set; }

        static ISettings AppSettings => CrossSettings.Current;

        public static bool ShowWhatsNew
        {
            get => AppSettings.GetValueOrDefault(nameof(ShowWhatsNew), true);
            set => AppSettings.AddOrUpdateValue(nameof(ShowWhatsNew), value);
        }

        public App()
		{
            InitializeComponent();

            ////////////////////////////////////////////////
            ///                                          ///
            /// To Activate Design Time on the Previewer ///
            ///                                          ///
            ////////////////////////////////////////////////
            DesignTime = false;

            if(!DesignTime)
    			StopMan = new StopManager ();

            DateTime today = DateTime.Now;
            DateTime sunset = DaylightDates.GetDaylight(today.Month).Sunset;
            DateTime sunrise = DaylightDates.GetDaylight(today.Month).Sunrise;

            if (today.TimeOfDay >= sunrise.TimeOfDay && today.TimeOfDay <= sunset.TimeOfDay)
            {
                IsNight = false;
                App.Current.Resources["SkyColor"] = Color.FromHex("#06bfcc");
            }
            else
            {
                IsNight = true;
                App.Current.Resources["SkyColor"] = Color.FromHex("#133B4F");
            }

            MainPage = new NavigationPage(new MainPage()) 
			{ 
				BarBackgroundColor = (Color) App.Current.Resources["SkyColor"],
				BarTextColor = Color.White
			};
		}

		protected override void OnStart()
		{
            AppCenter.Start("android=cd0a1891-a433-49be-ac26-6e9131a94fcf;" +
                            "ios=d499f829-341d-4d90-98a8-0509edc7f93a", typeof(Analytics), typeof(Crashes));
		}

		protected override void OnSleep()
		{            
            Debug.WriteLine("======== Sleep ========");


		}

		protected override void OnResume()
		{
            Debug.WriteLine("======== Resume ========");


		}

		void HandleConnectivityChanged (object sender, ConnectivityChangedEventArgs e)
		{
			Type currentPage = this.MainPage.GetType();
			if (e.IsConnected && currentPage != typeof(NavigationPage)) 
			{
                this.MainPage = new NavigationPage(new MainPage()) {
                    BarBackgroundColor = (Color) App.Current.Resources["SkyColor"],
					BarTextColor = Color.White
				};
			} 
			else if (!e.IsConnected && currentPage != typeof(NoConnectivity)) 
			{
				this.MainPage = new NoConnectivity ();
			}
		}
	}
}

