using Foundation;
using Plugin.LocalNotification;
using UIKit;

namespace MyStop.MauiVersion
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            LocalNotificationCenter.Current.NotificationActionTapped += Current_NotificationActionTapped;
            return base.FinishedLaunching(application, launchOptions);
        }

        private void Current_NotificationActionTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            // Handle notification tap - could navigate to specific bus stop
        }
    }
}
