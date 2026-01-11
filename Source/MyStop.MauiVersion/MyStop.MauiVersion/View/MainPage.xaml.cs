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

        if (App.IsNight)
        {
            imgLamp.Source = ImageSource.FromFile("img_lamp_night.png");
            imgFooter.Source = ImageSource.FromFile("bg_home_night.png");
            imgBusFront.Source = ImageSource.FromFile("img_bus_front_night.png");
        }

        _ = Animate();
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
                    imgLamp.Scale = 1;

                    await imgLamp.TranslateTo(-80, 0, 2000);
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
                        await imgBusFront.TranslateTo(0, 20, 1000);
                    }
                    else
                    {
                        await imgBusFront.TranslateTo(-5, 20, 1000);
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

        keepTicking = true;

        Dispatcher.StartTimer(
            new TimeSpan(0, 0, animationLength),
            Tick);
    }
}