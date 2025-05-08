using MyStop.MauiVersion.Utils;

namespace MyStop.MauiVersion;

public partial class App : Application
{
    public static StopManager StopManager { get; set; }

    public App()
    {
        InitializeComponent();

        StopManager = new StopManager();
        //_ = StopManager.Init();

        MainPage = new AppShell();
    }
}