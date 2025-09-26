using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class MainPage : ContentPage
{
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
    }
}