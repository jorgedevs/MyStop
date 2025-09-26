using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}