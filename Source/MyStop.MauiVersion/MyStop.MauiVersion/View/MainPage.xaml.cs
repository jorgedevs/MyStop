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

        //_ = Task.Run(async () =>
        //{
        //    await imgCloud1!.TranslateTo(500, 0, (uint)(animationLength / 4) * 1000, null);
        //    await imgCloud1!.TranslateTo(-500, 0, 0, null);
        //    await imgCloud1!.TranslateTo(0, 0, (uint)(animationLength / 4) * 1000, null);
        //    await Task.Delay((animationLength / 3) * 1000);
        //});

        //_ = Task.Run(async () =>
        //{
        //    await imgCloud2!.TranslateTo(500, 0, (uint)(animationLength / 3) * 1000, null);
        //    await imgCloud2!.TranslateTo(-200, 0, 0, null);
        //    await Task.Delay((animationLength / 3) * 1000);
        //});

        //_ = Task.Run(async () =>
        //{
        //    await imgCloud3!.TranslateTo(500, 0, (uint)(animationLength / 2) * 1000, null);
        //    await imgCloud3!.TranslateTo(-200, 0, 0, null);
        //    await Task.Delay((animationLength / 3) * 1000);
        //});

        _ = Task.Run(async () =>
        {
            for (int i = 0; i < animationLength; i++)
            {
                imgLamp?.TranslateTo(0, 0, 0, null);
                await imgLamp!.ScaleTo(1, 0, null);

                imgLamp?.TranslateTo(-80, 0, 2000, null);
                await imgLamp!.ScaleTo(0.55, 2000, null);
            }
        });

        _ = Task.Run(async () =>
        {
            for (int i = 0; i < animationLength; i++)
            {
                if (i % 2 == 0)
                {
                    await imgBusFront!.TranslateTo(0, 20, 1000, null);
                }
                else
                {
                    await imgBusFront!.TranslateTo(-5, 20, 1000, null);
                }
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