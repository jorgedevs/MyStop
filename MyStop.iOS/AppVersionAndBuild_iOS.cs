using MyStop.iOS;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppVersionAndBuild_iOS))]
namespace MyStop.iOS
{
    public class AppVersionAndBuild_iOS : IAppVersionAndBuild
    {
        public string GetVersionNumber()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        }

        public string GetBuildNumber()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
        }
    }
}