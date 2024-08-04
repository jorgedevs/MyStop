using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class FavouriteStopsPage : ContentPage
{
    public FavouriteStopsPage()
    {
        InitializeComponent();
        BindingContext = new FavouritesStopsViewModel();
    }
}