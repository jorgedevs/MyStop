namespace MyStop.MauiVersion.View;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();

        ApplyTheme(App.IsNight);

        // Subscribe to theme changes
        if (App.ThemeService != null)
        {
            App.ThemeService.ThemeChanged += OnThemeChanged;
        }
    }

    private void OnThemeChanged(object? sender, bool isNight)
    {
        MainThread.BeginInvokeOnMainThread(() => ApplyTheme(isNight));
    }

    private void ApplyTheme(bool isNight)
    {
        if (isNight)
        {
            imgFooter.Source = ImageSource.FromFile("bg_donate_city_night.png");
        }
        else
        {
            imgFooter.Source = ImageSource.FromFile("bg_donate_city.png");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyTheme(App.IsNight);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Unsubscribe to prevent memory leaks
        if (App.ThemeService != null)
        {
            App.ThemeService.ThemeChanged -= OnThemeChanged;
        }
    }
}