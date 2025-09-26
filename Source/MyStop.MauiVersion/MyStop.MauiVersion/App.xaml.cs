namespace MyStop.MauiVersion;

public partial class App : Application
{
    public static bool IsNight { get; set; }

    public App()
    {
        InitializeComponent();

        IsNight = true;
        App.Current.Resources["SkyColor"] = Color.FromArgb("#133B4F");

        MainPage = new AppShell();
    }
}