using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        MessagingCenter.Subscribe<MainViewModel, Stop>(this, MainViewModel.STOP_FOUND, async (sender, arg) =>
        {
            //await Navigation.PushAsync(new BusArrivalsPage(arg), true);
        });
    }
}