using Android.App;
using Android.OS;
using Android.Content.PM;

namespace MyStop.Forms.Droid
{
    [Activity(Label = "MyStop", Icon = "@mipmap/icon_launcher", RoundIcon = "@mipmap/icon_round_launcher", Theme = "@style/Theme.Splash", 
		MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
	public class SplashScreen : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
            Window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
			base.OnCreate (bundle);
			this.StartActivity(typeof(MainActivity));
			Finish ();
		}
	}
}