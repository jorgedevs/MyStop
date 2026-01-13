using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class MainPage : ContentPage
{
    int animationLength = 60;
    bool isAnimating;
    bool keepTicking;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        ApplyTheme(App.IsNight);

        // Subscribe to theme changes
        if (App.ThemeService != null)
        {
            App.ThemeService.ThemeChanged += OnThemeChanged;
        }

        _ = Animate();
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

        // Run animations on the main thread using MainThread.BeginInvokeOnMainThread
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                for (int i = 0; i < animationLength; i++)
                {
                    if (imgLamp == null) return;
                    
                    imgLamp.TranslationX = 0;
                    imgLamp.TranslationY = 0;
                    imgLamp.Scale = 1;

                    imgLamp.TranslateTo(-80, 25, 2000);
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

        // Refresh theme when page appears
        ApplyTheme(App.IsNight);

        keepTicking = true;

        Dispatcher.StartTimer(
            new TimeSpan(0, 0, animationLength),
            Tick);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Unsubscribe when page disappears to prevent memory leaks
        if (App.ThemeService != null)
        {
            App.ThemeService.ThemeChanged -= OnThemeChanged;
        }
    }
}