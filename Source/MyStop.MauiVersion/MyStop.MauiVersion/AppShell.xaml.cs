using MyStop.MauiVersion.View;

namespace MyStop.MauiVersion;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("AboutPage", typeof(AboutPage));
        Routing.RegisterRoute("BusArrivalsPage", typeof(BusArrivalsPage));
        Routing.RegisterRoute("FavouriteStopsPage", typeof(FavouriteStopsPage));
    }
}
