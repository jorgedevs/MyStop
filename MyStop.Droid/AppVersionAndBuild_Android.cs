using MyStop.Forms.Droid;
using Android.Content.PM;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppVersionAndBuild_Android))]
namespace MyStop.Forms.Droid
{
    public class AppVersionAndBuild_Android : IAppVersionAndBuild
    {
        PackageInfo _appInfo;

        public AppVersionAndBuild_Android()
        {
            var context = Android.App.Application.Context;
            _appInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
        }

        public string GetVersionNumber()
        {
            return _appInfo.VersionName;
        }

        public string GetBuildNumber()
        {
            return _appInfo.VersionCode.ToString();
        }
    }
}