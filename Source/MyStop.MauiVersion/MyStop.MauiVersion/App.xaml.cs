using MyStop.MauiVersion.Services;

namespace MyStop.MauiVersion;

public partial class App : Application
{
    private static IThemeService? _themeService;

    /// <summary>
    /// Gets whether the current theme is night mode.
    /// </summary>
    public static bool IsNight => _themeService?.IsNight ?? false;

    /// <summary>
    /// Gets the theme service instance for subscribing to theme changes.
    /// </summary>
    public static IThemeService? ThemeService => _themeService;

    public App(IThemeService themeService)
    {
        InitializeComponent();

        _themeService = themeService;

        // Initialize theme asynchronously
        _ = InitializeThemeAsync();

        MainPage = new AppShell();
    }

    private async Task InitializeThemeAsync()
    {
        try
        {
            await _themeService!.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing theme: {ex.Message}");
        }
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        _themeService?.StopMonitoring();
    }

    protected override void OnResume()
    {
        base.OnResume();
        _ = _themeService?.RefreshThemeAsync();
    }
}