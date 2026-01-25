using Android.App;
using Android.Content;
using Android.Content.PM;
using Plugin.LocalNotification;

namespace MyStop.MauiVersion;

[Activity(Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ScreenOrientation = ScreenOrientation.Portrait,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
    ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize |
    ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        LocalNotificationCenter.NotifyNotificationTapped(intent);
    }
}