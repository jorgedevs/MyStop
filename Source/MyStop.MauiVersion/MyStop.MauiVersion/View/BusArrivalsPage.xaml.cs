using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class BusArrivalsPage : ContentPage
{
    public BusArrivalsPage(BusArrivalsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}