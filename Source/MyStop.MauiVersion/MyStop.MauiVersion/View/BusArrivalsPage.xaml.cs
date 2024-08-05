using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class BusArrivalsPage : ContentPage
{
    Random randomBusModel;
    BusArrivalsViewModel vm;
    bool _keepTicking;

    public BusArrivalsPage()
    {
        InitializeComponent();
    }

    public BusArrivalsPage(Stop _stop)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = vm = new BusArrivalsViewModel(_stop);

        randomBusModel = new Random();

        //if (App.IsNight)
        //{
        //    imgFooter.Source = ImageSource.FromFile("bg_arrivals_night.png");
        //    imgBus.Source = ImageSource.FromFile("img_bus_side_long_night.png");
        //    imgTopList.Source = ImageSource.FromFile("img_gradient_top_night.png");
        //    imgBottomList.Source = ImageSource.FromFile("img_gradient_bottom_night.png");
        //}

        //Animate();
    }
}