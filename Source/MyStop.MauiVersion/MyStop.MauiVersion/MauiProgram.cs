using Microsoft.Extensions.Logging;
using MyStop.MauiVersion.Services;
using MyStop.MauiVersion.View;
using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IGtfsService, GtfsService>();
        builder.Services.AddSingleton<ISQLiteService, SQLiteService>();

        builder.Services.AddTransient<BaseViewModel>();
        builder.Services.AddTransient<BusArrivalsViewModel>();
        builder.Services.AddTransient<FavouriteStopsViewModel>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<BusArrivalsPage>();
        builder.Services.AddTransient<FavouriteStopsPage>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
