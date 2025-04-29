using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class FavouriteStopsPage : ContentPage
{
    FavouriteStopsViewModel vm;

    public FavouriteStopsPage(FavouriteStopsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = vm = viewModel;
    }

    private async void ListStopsItemTapped(object sender, ItemTappedEventArgs e)
    {
        listStops.SelectedItem = null;

        if (e.Item != null)
        {
            if (((FavouriteStop)e.Item).EditMode)
            {
                vm.StopNumber = ((FavouriteStop)e.Item).StopNo!;
                vm.TagName = ((FavouriteStop)e.Item).Tag;
                vm.IsEditVisible = true;
            }
            else
            {
                await Navigation.PushAsync(new BusArrivalsPage(e.Item as Stop));
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _ = vm.LoadStops();
    }
}