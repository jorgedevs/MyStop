using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.Toasts;
using Xamarin.Forms;

namespace MyStop.Forms.Droid
{
    [Activity(Label = "MyStop", Icon = "@mipmap/icon_launcher", RoundIcon = "@mipmap/icon_round_launcher", Theme = "@style/Theme.Icon", 
        LaunchMode = LaunchMode.SingleTask, ConfigurationChanges = ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {                
		public static Activity Activity { get; set; }

		protected override void OnCreate(Bundle bundle)
        {
            Window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            base.OnCreate(bundle);

			MainActivity.Activity = this;
            App.ScreenWidth = Resources.DisplayMetrics.WidthPixels; 
            App.ScreenHeight = Resources.DisplayMetrics.HeightPixels; 

            global::Xamarin.Forms.Forms.Init(this, bundle);

            DependencyService.Register<ToastNotification>();
            ToastNotification.Init(this, new PlatformOptions() { SmallIconDrawable = Android.Resource.Drawable.IcDialogInfo });

            LoadApplication(new App());

            App.AndroidIcon = this.ApplicationInfo.Icon;
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
