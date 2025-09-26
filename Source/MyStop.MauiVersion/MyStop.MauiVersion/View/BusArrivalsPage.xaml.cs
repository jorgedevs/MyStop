using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class BusArrivalsPage : ContentPage
{
    public BusArrivalsPage(BusArrivalsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        if (App.IsNight)
        {
            imgFooter.Source = ImageSource.FromFile("bg_arrivals_night.png");
            imgBus.Source = ImageSource.FromFile("img_bus_side_night.png");
            imgTopList.Source = ImageSource.FromFile("img_gradient_top_night.png");
            imgBottomList.Source = ImageSource.FromFile("img_gradient_bottom_night.png");
        }
    }
}