using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services;
using MyStop.MauiVersion.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class BusArrivalsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IGtfsService _gtfsService;
    private readonly ISQLiteService _sqliteService;
    private readonly IGtfsLiveService _gtfsLiveService;

    // Configuration constants
    private const int MaxStopTimesToProcess = 500;
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

    bool useRealtimeData = true;
    /// <summary>
    /// When true, uses GTFS realtime data for arrivals. Falls back to static schedule if realtime fails.
    /// </summary>
    public bool UseRealtimeData
    {
        get => useRealtimeData;
        set { useRealtimeData = value; OnPropertyChanged(nameof(UseRealtimeData)); OnPropertyChanged(nameof(DataSourceText)); }
    }

    string dataSourceText = "Realtime";
    /// <summary>
    /// Indicates the current data source being used (Realtime or Schedule).
    /// </summary>
    public string DataSourceText
    {
        get => dataSourceText;
        set { dataSourceText = value; OnPropertyChanged(nameof(DataSourceText)); }
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

    // Store stop_id for realtime queries
    private string? _stopId;

    public ICommand FavouriteCommand { get; set; }

    public ICommand RefreshCommand { get; set; }

    public ICommand ToggleDataSourceCommand { get; set; }

    /// <summary>
    /// Refreshes arrival times. Called automatically every 15 seconds and on pull-to-refresh.
    /// </summary>
    public void RefreshArrivalTimes()
    {
        _ = GetBusArrivalsTimes();
    }

    public BusArrivalsViewModel(
        IGtfsService gtfsService,
        ISQLiteService sqliteService,
        IGtfsLiveService gtfsLiveService)
    {
        _gtfsService = gtfsService;
        _sqliteService = sqliteService;
        _gtfsLiveService = gtfsLiveService;

        ArrivalTimes = new ObservableCollection<ScheduleModel>();

        IsFavouriteBusStop = false;

        FavouriteCommand = new Command(ToggleSaveBusStop);

        RefreshCommand = new Command(async () =>
        {
            await GetBusArrivalsTimes();
            IsUpdatingArrivalTimes = false;
        });

        ToggleDataSourceCommand = new Command(() =>
        {
            UseRealtimeData = !UseRealtimeData;
            _ = GetBusArrivalsTimes();
        });

        _ = Initialize();
    }

    private async Task Initialize()
    {
        IsFavouriteBusStop = false;

        if (IsFavouriteBusStop)
            FavoriteIcon = "icon_favourites_remove.png";
        else
            FavoriteIcon = "icon_favourites_add.png";
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
                _stopId = stopInfo.stop_id; // Store for realtime queries
            }
            else
            {
                var stop = new StopModel()
                {
                    StopNo = stopCode,
                    Name = "Stop information not available"
                };

                Stop = stop;
                StopNumber = Stop.StopNo!;
                StopInfo = Stop.Name!;
                _stopId = null;
            }

            // Load arrival times after stop info is set
            _ = GetBusArrivalsTimes();
        }
    }

    private async Task GetBusArrivalsTimes()
    {
        try
        {
            ArrivalTimes.Clear();

            if (Stop == null || string.IsNullOrEmpty(StopNumber))
                return;

            var arrivals = new List<ScheduleModel>();
            bool usedRealtime = false;

            // Try realtime data first if enabled
            if (UseRealtimeData && !string.IsNullOrEmpty(_stopId))
            {
                arrivals = await GetRealtimeArrivals(_stopId);
                usedRealtime = arrivals.Count > 0;
            }

            // Fall back to static schedule if no realtime data or realtime is disabled
            if (arrivals.Count == 0)
            {
                arrivals = await GetStaticScheduleArrivals();
                usedRealtime = false;
            }

            // Update data source indicator
            DataSourceText = usedRealtime ? "Realtime" : "Schedule";

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
                    ExpectedCountdown = 0,
                    ScheduleStatus = ""
                });
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting arrival times: {ex.Message}");

            ArrivalTimes.Clear();
            ArrivalTimes.Add(new ScheduleModel()
            {
                RouteNo = "--",
                Destination = "Error loading arrival times",
                ExpectedCountdown = 0,
                ScheduleStatus = ""
            });
        }
    }

    /// <summary>
    /// Gets arrivals from GTFS realtime feed.
    /// </summary>
    private async Task<List<ScheduleModel>> GetRealtimeArrivals(string stopId)
    {
        var arrivals = new List<ScheduleModel>();

        try
        {
            var realtimeArrivals = await _gtfsLiveService.GetRealtimeArrivalsForStopAsync(stopId);

            var now = DateTimeOffset.Now.ToUnixTimeSeconds();

            foreach (var rt in realtimeArrivals)
            {
                // Skip cancelled trips
                if (rt.ScheduleRelationship == "CANCELED" || rt.ScheduleRelationship == "Canceled")
                    continue;

                long? arrivalTimestamp = rt.ArrivalTime ?? rt.DepartureTime;
                if (!arrivalTimestamp.HasValue)
                    continue;

                // Calculate minutes until arrival
                var secondsUntilArrival = arrivalTimestamp.Value - now;
                if (secondsUntilArrival < -60) // Already passed (with 1 min buffer)
                    continue;

                var minutesUntilArrival = (int)(secondsUntilArrival / 60);
                if (minutesUntilArrival > MaxMinutesAhead)
                    continue;

                // Determine schedule status based on delay
                string scheduleStatus = GetScheduleStatus(rt.ArrivalDelay ?? 0);

                arrivals.Add(new ScheduleModel()
                {
                    RouteNo = rt.RouteShortName ?? "?",
                    Destination = rt.Headsign ?? "Unknown",
                    ExpectedCountdown = Math.Max(0, minutesUntilArrival),
                    ScheduleStatus = scheduleStatus
                });
            }

            Debug.WriteLine($"Got {arrivals.Count} realtime arrivals for stop {stopId}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting realtime arrivals: {ex.Message}");
        }

        return arrivals;
    }

    /// <summary>
    /// Gets arrivals from static GTFS schedule data.
    /// </summary>
    private async Task<List<ScheduleModel>> GetStaticScheduleArrivals()
    {
        var arrivals = new List<ScheduleModel>();

        try
        {
            var stopInfo = _sqliteService.GetStopInfo(StopNumber);
            if (stopInfo == null)
                return arrivals;

            // Get active service IDs for today
            var activeServices = await _sqliteService.GetActiveServiceIdsAsync(DateTime.Now);

            // Get all stop times for this stop
            var stopTimes = await _sqliteService.GetStopTimesForStopAsync(stopInfo.stop_id!);

            var now = DateTime.Now;
            var currentTimeOfDay = now.TimeOfDay;

            foreach (var stopTime in stopTimes.Take(MaxStopTimesToProcess))
            {
                if (string.IsNullOrEmpty(stopTime.arrival_time))
                    continue;

                var trip = await _sqliteService.GetTripAsync(stopTime.trip_id!);
                if (trip == null)
                    continue;

                if (!activeServices.Contains(trip.service_id!))
                    continue;

                var timeParts = stopTime.arrival_time.Split(':');
                if (timeParts.Length < 2)
                    continue;

                if (!int.TryParse(timeParts[0], out int hours) ||
                    !int.TryParse(timeParts[1], out int minutes))
                    continue;

                var adjustedHours = hours % 24;
                var arrivalTimeSpan = new TimeSpan(adjustedHours, minutes, 0);

                var diff = arrivalTimeSpan - currentTimeOfDay;
                if (diff.TotalMinutes < 0)
                    diff = diff.Add(TimeSpan.FromHours(24));

                var minutesUntilArrival = (int)diff.TotalMinutes;

                if (minutesUntilArrival > MaxMinutesAhead)
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
                    ExpectedCountdown = minutesUntilArrival,
                    ScheduleStatus = "Scheduled" // Static data doesn't have delay info
                });
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting static arrivals: {ex.Message}");
        }

        return arrivals;
    }

    /// <summary>
    /// Determines the schedule status text based on delay seconds.
    /// </summary>
    private static string GetScheduleStatus(int delaySeconds)
    {
        if (delaySeconds < -60) // More than 1 minute early
            return "Early";
        else if (delaySeconds > 300) // More than 5 minutes late
            return "Late";
        else if (delaySeconds > 60) // 1-5 minutes late
            return "Delayed";
        else
            return "On Time";
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