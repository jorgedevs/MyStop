using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();

        MessagingCenter.Subscribe<MainViewModel, Stop>(this, MainViewModel.STOP_FOUND, async (sender, arg) =>
        {
            await Navigation.PushAsync(new BusArrivalsPage(arg), true);
        });
    }

    private async void ImageButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FavouriteStopsPage(), true);
    }
}