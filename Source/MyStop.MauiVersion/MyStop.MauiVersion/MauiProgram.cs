using Microsoft.Extensions.Logging;
using MyStop.MauiVersion.Services;
using MyStop.MauiVersion.Services.Interfaces;
using MyStop.MauiVersion.View;
using MyStop.MauiVersion.ViewModel;
using Plugin.LocalNotification;

namespace MyStop.MauiVersion;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Services
        builder.Services.AddSingleton<IThemeService, ThemeService>();
        builder.Services.AddSingleton<IGtfsService, GtfsService>();
        builder.Services.AddSingleton<ISQLiteService, SQLiteService>();
        builder.Services.AddSingleton<IGtfsLiveService, GtfsLiveService>();
        builder.Services.AddSingleton<IAlertService, AlertService>();

        // ViewModels
        builder.Services.AddTransient<BaseViewModel>();
        builder.Services.AddTransient<BusArrivalsViewModel>();
        builder.Services.AddTransient<FavouriteStopsViewModel>();
        builder.Services.AddTransient<MainViewModel>();

        // Pages
        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<BusArrivalsPage>();
        builder.Services.AddTransient<FavouriteStopsPage>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
