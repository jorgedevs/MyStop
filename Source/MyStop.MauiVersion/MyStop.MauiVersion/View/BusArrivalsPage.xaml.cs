using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.ViewModel;

namespace MyStop.MauiVersion.View;

public partial class BusArrivalsPage : ContentPage
{
    bool _keepTicking;
    Random randomBusModel = new Random();
    private readonly BusArrivalsViewModel _viewModel;

    public BusArrivalsPage(BusArrivalsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        ApplyTheme(App.IsNight);

        // Subscribe to theme changes
        if (App.ThemeService != null)
        {
            App.ThemeService.ThemeChanged += OnThemeChanged;
        }

        // Wire up alert popup events
        alertPopup.AlertConfirmed += OnAlertConfirmed;
        alertPopup.AlertCancelled += OnAlertCancelled;
    }

    private void OnThemeChanged(object? sender, bool isNight)
    {
        MainThread.BeginInvokeOnMainThread(() => ApplyTheme(isNight));
    }

    private void ApplyTheme(bool isNight)
    {
        if (isNight)
        {
            imgFooter.Source = ImageSource.FromFile("bg_arrivals_night.png");
            imgBus.Source = ImageSource.FromFile("img_bus_side_night.png");
            imgBottomList.Source = ImageSource.FromFile("img_gradient_bottom_night.png");
        }
        else
        {
            imgFooter.Source = ImageSource.FromFile("bg_arrivals.png");
            imgBus.Source = ImageSource.FromFile("img_bus_side.png");
            imgBottomList.Source = ImageSource.FromFile("img_gradient_bottom.png");
        }
    }

    public bool Tick()
    {
        if (_keepTicking)
        {
            // Refresh arrival times every 15 seconds
            _viewModel.RefreshArrivalTimes();
            _ = Animate();
        }
        return _keepTicking;
    }

    public async Task Animate()
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

        // Refresh theme when page appears
        ApplyTheme(App.IsNight);

        _keepTicking = true;

        Dispatcher.StartTimer(
            new TimeSpan(0, 0, 20),
            () => Tick()
        );

        _ = Animate();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop auto-refresh when page is not visible
        _keepTicking = false;

        // Unsubscribe to prevent memory leaks
        if (App.ThemeService != null)
        {
            App.ThemeService.ThemeChanged -= OnThemeChanged;
        }

        // Unsubscribe from alert popup
        alertPopup.AlertConfirmed -= OnAlertConfirmed;
        alertPopup.AlertCancelled -= OnAlertCancelled;
    }

    private async void OnArrivalItemTapped(object? sender, EventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is ScheduleModel schedule)
        {
            if (schedule.HasAlert)
            {
                // Ask if user wants to cancel alert
                bool cancel = await DisplayAlert(
                    "Alert Active",
                    "This bus has an active alert. What would you like to do?",
                    "Cancel Alert",
                    "Keep Alert");

                if (cancel)
                {
                    await _viewModel.CancelAlertAsync(schedule);
                    await DisplayAlert("Alert Cancelled", "The alert has been removed.", "OK");
                }
            }
            else
            {
                // Show alert configuration popup
                alertPopup.Show(schedule);
            }
        }
    }

    private async void OnAlertConfirmed(object? sender, AlertConfigurationEventArgs e)
    {
        await _viewModel.CreateAlertAsync(
            e.Schedule,
            e.AlertMinutesBefore,
            e.IsContinuous);
    }

    private void OnAlertCancelled(object? sender, EventArgs e)
    {
        // User cancelled - nothing to do
    }
}