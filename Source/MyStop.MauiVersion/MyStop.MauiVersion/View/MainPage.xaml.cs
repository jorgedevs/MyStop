using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class MainPage : ContentPage
{
    int animationLength = 60;
    bool isAnimating;
    bool keepTicking;

    // Reference screen heights (in device-independent pixels)
    private const double Pixel5Height = 732;
    private const double Pixel8ProHeight = 932;

    // Translation values for reference devices
    private const double Pixel5TranslateX = -85;
    private const double Pixel5TranslateY = -20;
    private const double Pixel8ProTranslateX = -80;
    private const double Pixel8ProTranslateY = 25;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        ApplyTheme(App.IsNight);

        if (App.ThemeService != null)
        {
            App.ThemeService.ThemeChanged += OnThemeChanged;
        }

        _ = Animate();
    }

    private (double translateX, double translateY) GetLampTranslation()
    {
        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

        // Clamp the screen height to our reference range
        var clampedHeight = Math.Clamp(screenHeight, Pixel5Height, Pixel8ProHeight);

        // Calculate interpolation factor (0 = Pixel 5, 1 = Pixel 8 Pro)
        var t = (clampedHeight - Pixel5Height) / (Pixel8ProHeight - Pixel5Height);

        // Linearly interpolate between the two reference values
        var translateX = Pixel5TranslateX + (Pixel8ProTranslateX - Pixel5TranslateX) * t;
        var translateY = Pixel5TranslateY + (Pixel8ProTranslateY - Pixel5TranslateY) * t;

        return (translateX, translateY);
    }

    private void OnThemeChanged(object? sender, bool isNight)
    {
        MainThread.BeginInvokeOnMainThread(() => ApplyTheme(isNight));
    }

    private void ApplyTheme(bool isNight)
    {
        if (isNight)
        {
            imgLamp.Source = ImageSource.FromFile("img_lamp_night.png");
            imgFooter.Source = ImageSource.FromFile("bg_home_night.png");
            imgBusFront.Source = ImageSource.FromFile("img_bus_front_night.png");
        }
        else
        {
            imgLamp.Source = ImageSource.FromFile("img_lamp.png");
            imgFooter.Source = ImageSource.FromFile("bg_home.png");
            imgBusFront.Source = ImageSource.FromFile("img_bus_front.png");
        }
    }

    public bool Tick()
    {
        _ = Animate();

        return true;
    }

    async Task Animate()
    {
        if (imgCloud1 == null || imgCloud2 == null || imgCloud3 == null)
            return;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var (translateX, translateY) = GetLampTranslation();

                for (int i = 0; i < animationLength; i++)
                {
                    if (imgLamp == null) return;
                    
                    imgLamp.TranslationX = 0;
                    imgLamp.TranslationY = 0;
                    imgLamp.Scale = 1;

                    imgLamp.TranslateTo(translateX, translateY, 2000);
                    await imgLamp.ScaleTo(0.55, 2000);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
            }
        });

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                for (int i = 0; i < animationLength; i++)
                {
                    if (imgBusFront == null) return;
                    
                    if (i % 2 == 0)
                    {
                        await imgBusFront.TranslateTo(0, 10, 1000);
                    }
                    else
                    {
                        await imgBusFront.TranslateTo(-5, 10, 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
            }
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        ApplyTheme(App.IsNight);

        keepTicking = true;

        Dispatcher.StartTimer(
            new TimeSpan(0, 0, animationLength),
            Tick);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (App.ThemeService != null)
        {
            App.ThemeService.ThemeChanged -= OnThemeChanged;
        }
    }
}