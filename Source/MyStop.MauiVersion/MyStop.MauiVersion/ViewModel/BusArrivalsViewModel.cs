using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class BusArrivalsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IGtfsService _gtfsService;
    private readonly ISQLiteService _sqliteService;

    // Configuration constants
    private const int MaxStopTimesToProcess = 50;
    private const int MaxMinutesAhead = 120; // 2 hours
    private const int MaxArrivalsToDisplay = 15;

    public StopModel Stop { get; set; }

    public ScheduleModel Schedule { get; set; }

    public ObservableCollection<ScheduleModel> ArrivalTimes { get; set; }

    bool isFavouriteBusStop;
    public bool IsFavouriteBusStop
    {
        get => isFavouriteBusStop;
        set { isFavouriteBusStop = value; OnPropertyChanged(nameof(IsFavouriteBusStop)); }
    }

    bool isUpdatingArrivalTimes;
    public bool IsUpdatingArrivalTimes
    {
        get => isUpdatingArrivalTimes;
        set { isUpdatingArrivalTimes = value; OnPropertyChanged(nameof(IsUpdatingArrivalTimes)); }
    }

    string stopInfo;
    public string StopInfo
    {
        get => stopInfo;
        set { stopInfo = value; OnPropertyChanged(nameof(StopInfo)); }
    }

    string stopNumber;
    public string StopNumber
    {
        get => stopNumber;
        set { stopNumber = value; OnPropertyChanged(nameof(StopNumber)); }
    }

    string favoriteIcon;
    public string FavoriteIcon
    {
        get => favoriteIcon;
        set { favoriteIcon = value; OnPropertyChanged(nameof(FavoriteIcon)); }
    }

    public ICommand FavouriteCommand { get; set; }

    public ICommand RefreshCommand { get; set; }

    public BusArrivalsViewModel(
        IGtfsService gtfsService,
        ISQLiteService sqliteService)
    {
        _gtfsService = gtfsService;
        _sqliteService = sqliteService;

        ArrivalTimes = new ObservableCollection<ScheduleModel>();

        IsFavouriteBusStop = false;

        FavouriteCommand = new Command(ToggleSaveBusStop);

        RefreshCommand = new Command(async () =>
        {
            await GetBusArrivalsTimes();
            IsUpdatingArrivalTimes = false;
        });

        _ = Initialize();
    }

    private async Task Initialize()
    {
        //IsFavouriteBusStop = await App.StopManager.IsStop(StopNumber);
        IsFavouriteBusStop = false;

        if (IsFavouriteBusStop)
            FavoriteIcon = "icon_favourites_remove.png";
        else
            FavoriteIcon = "icon_favourites_add.png";

        await GetBusArrivalsTimes();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("BusStopNumber"))
        {
            string? stopCode = query["BusStopNumber"] as string;

            var stopInfo = _sqliteService.GetStopInfo(stopCode!);

            if (stopInfo != null)
            {
                var stop = new StopModel()
                {
                    StopNo = stopInfo.stop_code,
                    Name = stopInfo.stop_name
                };

                Stop = stop;
                StopNumber = Stop.StopNo!;
                StopInfo = Stop.Name!;
            }
            else
            {
                // Fallback if stop not found
                var stop = new StopModel()
                {
                    StopNo = stopCode,
                    Name = "Stop information not available"
                };

                Stop = stop;
                StopNumber = Stop.StopNo!;
                StopInfo = Stop.Name!;
            }
        }
    }

    private async Task GetBusArrivalsTimes()
    {
        try
        {
            ArrivalTimes.Clear();

            if (Stop == null || string.IsNullOrEmpty(StopNumber))
                return;

            // Get stop info to get the stop_id
            var stopInfo = _sqliteService.GetStopInfo(StopNumber);
            if (stopInfo == null)
                return;

            // Get all stop times for this stop
            var stopTimes = await _sqliteService.GetStopTimesForStopAsync(stopInfo.stop_id!);

            // Get current time
            var now = DateTime.Now;
            var currentTimeOfDay = now.TimeOfDay;

            // Process each stop time and calculate arrival
            var arrivals = new List<ScheduleModel>();

            foreach (var stopTime in stopTimes.Take(MaxStopTimesToProcess))
            {
                // Parse arrival time (format: HH:MM:SS, can be > 24:00:00 for next day)
                if (string.IsNullOrEmpty(stopTime.arrival_time))
                    continue;

                var timeParts = stopTime.arrival_time.Split(':');
                if (timeParts.Length < 2)
                    continue;

                if (!int.TryParse(timeParts[0], out int hours) ||
                    !int.TryParse(timeParts[1], out int minutes))
                    continue;

                // Handle hours >= 24 (next day)
                var adjustedHours = hours % 24;
                var arrivalTimeSpan = new TimeSpan(adjustedHours, minutes, 0);

                // Calculate minutes until arrival
                var diff = arrivalTimeSpan - currentTimeOfDay;
                if (diff.TotalMinutes < 0)
                    diff = diff.Add(TimeSpan.FromHours(24)); // Next day

                var minutesUntilArrival = (int)diff.TotalMinutes;

                // Only show arrivals within the next 2 hours
                if (minutesUntilArrival > MaxMinutesAhead)
                    continue;

                // Get trip and route info
                var trip = await _sqliteService.GetTripAsync(stopTime.trip_id!);
                if (trip == null)
                    continue;

                var route = await _sqliteService.GetRouteAsync(trip.route_id!);
                if (route == null)
                    continue;

                var routeNo = route.route_short_name ?? route.route_id ?? "?";
                var destination = trip.trip_headsign ?? route.route_long_name ?? "Unknown";

                arrivals.Add(new ScheduleModel()
                {
                    RouteNo = routeNo,
                    Destination = destination,
                    ExpectedCountdown = minutesUntilArrival
                });
            }

            // Sort by arrival time and take top results
            var sortedArrivals = arrivals.OrderBy(a => a.ExpectedCountdown).Take(MaxArrivalsToDisplay);

            foreach (var arrival in sortedArrivals)
            {
                ArrivalTimes.Add(arrival);
            }

            // If no arrivals found, show a message
            if (ArrivalTimes.Count == 0)
            {
                ArrivalTimes.Add(new ScheduleModel()
                {
                    RouteNo = "--",
                    Destination = "No upcoming arrivals found",
                    ExpectedCountdown = 0
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting arrival times: {ex.Message}");

            // Show error state
            ArrivalTimes.Clear();
            ArrivalTimes.Add(new ScheduleModel()
            {
                RouteNo = "--",
                Destination = "Error loading arrival times",
                ExpectedCountdown = 0
            });
        }
    }

    private async void ToggleSaveBusStop()
    {
        var stop = new SavedStopModel()
        {
            Name = Stop.Name,
            Routes = Stop.Routes,
            StopNo = Stop.StopNo,
            Tag = Stop.Tag
        };

        if (IsFavouriteBusStop)
        {
            await _sqliteService.RemoveStop(stop);
            IsFavouriteBusStop = false;
            FavoriteIcon = "icon_favourites_add.png";
        }
        else
        {
            await _sqliteService.SaveStop(stop);
            IsFavouriteBusStop = true;
            FavoriteIcon = "icon_favourites_remove.png";
        }
    }
}