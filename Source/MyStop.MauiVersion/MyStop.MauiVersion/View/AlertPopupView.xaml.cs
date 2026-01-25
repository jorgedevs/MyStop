using MyStop.MauiVersion.Model;

namespace MyStop.MauiVersion.View;

public partial class AlertPopupView : ContentView
{
    public event EventHandler<AlertConfigurationEventArgs>? AlertConfirmed;
    public event EventHandler? AlertCancelled;

    private ScheduleModel? _schedule;
    private List<int> _availableAlertTimes;

    public AlertPopupView()
    {
        InitializeComponent();
        _availableAlertTimes = new List<int>();
    }

    public void Show(ScheduleModel schedule)
    {
        _schedule = schedule;

        // Set route info
        labelRouteInfo.Text = $"Route {schedule.RouteNo} - {schedule.Destination} - Arriving in {schedule.ExpectedCountdown} minutes";
        //labelArrivalInfo.Text = $"Arriving in {schedule.ExpectedCountdown} minutes";

        // Populate alert time options
        PopulateAlertTimes(schedule.ExpectedCountdown);

        // Animate in
        this.IsVisible = true;
        this.Opacity = 0;
        this.FadeTo(1, 250, Easing.CubicOut);
    }

    private void PopulateAlertTimes(int countdown)
    {
        _availableAlertTimes.Clear();
        pickerAlertTime.Items.Clear();

        // Offer alert times: Arriving now, 1 min, 2 min, 3 min, 5 min, 10 min
        var options = new[] { 0, 1, 2, 3, 5, 10 };

        foreach (var minutes in options)
        {
            if (minutes <= countdown)
            {
                _availableAlertTimes.Add(minutes);

                string label = minutes == 0
                    ? "Arriving now"
                    : $"{minutes} minute{(minutes != 1 ? "s" : "")} away";

                pickerAlertTime.Items.Add(label);
            }
        }

        // Default to 3 minutes if available, otherwise first option
        int defaultIndex = _availableAlertTimes.IndexOf(3);
        pickerAlertTime.SelectedIndex = defaultIndex >= 0 ? defaultIndex : 0;
    }

    private void OnAlertTimeChanged(object? sender, EventArgs e)
    {
        // User selected different alert time
    }

    private void OnBackdropTapped(object? sender, EventArgs e)
    {
        Hide();
        AlertCancelled?.Invoke(this, EventArgs.Empty);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Hide();
        AlertCancelled?.Invoke(this, EventArgs.Empty);
    }

    private void OnConfirmClicked(object? sender, EventArgs e)
    {
        if (_schedule == null || pickerAlertTime.SelectedIndex < 0)
            return;

        int selectedMinutes = _availableAlertTimes[pickerAlertTime.SelectedIndex];
        bool isContinuous = switchContinuous.IsToggled;

        AlertConfirmed?.Invoke(this, new AlertConfigurationEventArgs
        {
            Schedule = _schedule,
            AlertMinutesBefore = selectedMinutes,
            IsContinuous = isContinuous
        });

        Hide();
    }

    private async void Hide()
    {
        await this.FadeTo(0, 200, Easing.CubicIn);
        this.IsVisible = false;
    }
}

public class AlertConfigurationEventArgs : EventArgs
{
    public required ScheduleModel Schedule { get; set; }
    public int AlertMinutesBefore { get; set; }
    public bool IsContinuous { get; set; }
}
