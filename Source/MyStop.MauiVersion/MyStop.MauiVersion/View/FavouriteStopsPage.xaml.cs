using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class FavouriteStopsPage : ContentPage
{
    FavouriteStopsViewModel vm;

    public FavouriteStopsPage(FavouriteStopsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = vm = viewModel;

        if (App.IsNight)
        {
            imgBus.Source = ImageSource.FromFile("img_bus_side_night.png");
            imgFooter.Source = ImageSource.FromFile("bg_terminal_night.png");
            imgTopList.Source = ImageSource.FromFile("img_gradient_top_night.png");
            imgBottomList.Source = ImageSource.FromFile("img_gradient_bottom_night.png");
        }
    }

    private async void ListStopsItemTapped(object sender, ItemTappedEventArgs e)
    {
        listStops.SelectedItem = null;

        if (e.Item != null)
        {
            //if (((FavouriteStop)e.Item).EditMode)
            //{
            //    vm.StopNumber = ((FavouriteStop)e.Item).StopNo!;
            //    vm.TagName = ((FavouriteStop)e.Item).Tag;
            //    vm.IsEditVisible = true;
            //}
            //else
            //{
            //    await Navigation.PushAsync(new BusArrivalsPage(e.Item as Stop));
            //}
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //vm.LoadStops();
    }
}