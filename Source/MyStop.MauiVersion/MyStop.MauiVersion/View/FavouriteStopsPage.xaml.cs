using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class FavouriteStopsPage : ContentPage
{
    bool _keepTicking;
    Random randomBusModel = new Random();

    public FavouriteStopsPage(FavouriteStopsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

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

    public bool Tick()
    {
        if (_keepTicking)
        {
            //vm.GetData();
            Animate();
        }
        return _keepTicking;
    }

    async Task Animate()
    {
        if (imgBus == null)
            return;

        imgBus.IsVisible = true;
        imgBus.TranslationX = this.Width + 10;
        await imgBus!.TranslateTo(0, 0, 3500, Easing.CubicOut);
        await imgBus!.TranslateTo(0, 0, 2000, null);
        await imgBus!.TranslateTo(-(this.Width + 10), 0, 3500, Easing.CubicIn);

        string busImageSource = "";
        busImageSource = (randomBusModel.Next(2) == 0) ? "img_bus_side" : "img_bus_side_long";
        if (App.IsNight)
            busImageSource += "_night";

        imgBus.Source = ImageSource.FromFile(busImageSource);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _keepTicking = true;

        Dispatcher.StartTimer(
            new TimeSpan(0, 0, 15),
            () => Tick()
        );

        _ = Animate();

        //vm.LoadStops();
    }
}