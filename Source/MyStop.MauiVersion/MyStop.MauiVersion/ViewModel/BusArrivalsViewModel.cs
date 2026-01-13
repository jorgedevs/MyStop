using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services;
using MyStop.MauiVersion.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class BusArrivalsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IGtfsService _gtfsService;
    private readonly ISQLiteService _sqliteService;
    private readonly IGtfsLiveService _gtfsLiveService;

    private const int MaxStopTimesToProcess = 500;
    private const int MaxMinutesAhead = 120;
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
    public bool UseRealtimeData
    {
        get => useRealtimeData;
        set { useRealtimeData = value; OnPropertyChanged(nameof(UseRealtimeData)); OnPropertyChanged(nameof(DataSourceText)); }
    }

    string dataSourceText = "Realtime";
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

    private string? _stopId;

    public ICommand FavouriteCommand { get; set; }
    public ICommand RefreshCommand { get; set; }
    public ICommand ToggleDataSourceCommand { get; set; }

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
        FavoriteIcon = IsFavouriteBusStop ? "icon_favourites_remove.png" : "icon_favourites_add.png";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("BusStopNumber"))
        {
            string? stopCode = query["BusStopNumber"] as string;
            var stopInfo = _sqliteService.GetStopInfo(stopCode!);

            if (stopInfo != null)
            {
                Stop = new StopModel { StopNo = stopInfo.stop_code, Name = stopInfo.stop_name };
                StopNumber = Stop.StopNo!;
                StopInfo = Stop.Name!;
                _stopId = stopInfo.stop_id;
            }
            else
            {
                Stop = new StopModel { StopNo = stopCode, Name = "Stop information not available" };
                StopNumber = Stop.StopNo!;
                StopInfo = Stop.Name!;
                _stopId = null;
            }

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

            if (UseRealtimeData && !string.IsNullOrEmpty(_stopId))
            {
                arrivals = await GetRealtimeArrivals(_stopId);
                usedRealtime = arrivals.Count > 0;
            }

            if (arrivals.Count == 0)
            {
                arrivals = await GetStaticScheduleArrivals();
                usedRealtime = false;
            }

            DataSourceText = usedRealtime ? "Realtime" : "Schedule";

            var sortedArrivals = arrivals.OrderBy(a => a.ExpectedCountdown).Take(MaxArrivalsToDisplay);

            foreach (var arrival in sortedArrivals)
            {
                ArrivalTimes.Add(arrival);
            }

            if (ArrivalTimes.Count == 0)
            {
                ArrivalTimes.Add(new ScheduleModel
                {
                    RouteNo = "--",
                    Destination = "No upcoming arrivals found",
                    ExpectedCountdown = 0,
                    ScheduleStatus = ""
                });
            }
        }
        catch
        {
            ArrivalTimes.Clear();
            ArrivalTimes.Add(new ScheduleModel
            {
                RouteNo = "--",
                Destination = "Error loading arrival times",
                ExpectedCountdown = 0,
                ScheduleStatus = ""
            });
        }
    }

    private async Task<List<ScheduleModel>> GetRealtimeArrivals(string stopId)
    {
        var arrivals = new List<ScheduleModel>();

        try
        {
            var realtimeArrivals = await _gtfsLiveService.GetRealtimeArrivalsForStopAsync(stopId);
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();

            foreach (var rt in realtimeArrivals)
            {
                if (rt.ScheduleRelationship == "CANCELED" || rt.ScheduleRelationship == "Canceled")
                    continue;

                long? arrivalTimestamp = rt.ArrivalTime ?? rt.DepartureTime;
                if (!arrivalTimestamp.HasValue)
                    continue;

                var secondsUntilArrival = arrivalTimestamp.Value - now;
                if (secondsUntilArrival < -60)
                    continue;

                var minutesUntilArrival = (int)(secondsUntilArrival / 60);
                if (minutesUntilArrival > MaxMinutesAhead)
                    continue;

                var routeNo = rt.RouteShortName ?? "?";
                var destination = CleanDestination(rt.Headsign, routeNo);

                arrivals.Add(new ScheduleModel
                {
                    RouteNo = routeNo,
                    Destination = destination,
                    ExpectedCountdown = Math.Max(0, minutesUntilArrival),
                    ScheduleStatus = GetScheduleStatus(rt.ArrivalDelay ?? 0)
                });
            }
        }
        catch
        {
            // Return empty list on error
        }

        return arrivals;
    }

    private async Task<List<ScheduleModel>> GetStaticScheduleArrivals()
    {
        var arrivals = new List<ScheduleModel>();

        try
        {
            var stopInfo = _sqliteService.GetStopInfo(StopNumber);
            if (stopInfo == null)
                return arrivals;

            var activeServices = await _sqliteService.GetActiveServiceIdsAsync(DateTime.Now);
            var stopTimes = await _sqliteService.GetStopTimesForStopAsync(stopInfo.stop_id!);

            var now = DateTime.Now;
            var currentTimeOfDay = now.TimeOfDay;

            foreach (var stopTime in stopTimes.Take(MaxStopTimesToProcess))
            {
                if (string.IsNullOrEmpty(stopTime.arrival_time))
                    continue;

                var trip = await _sqliteService.GetTripAsync(stopTime.trip_id!);
                if (trip == null || !activeServices.Contains(trip.service_id!))
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
                var rawDestination = trip.trip_headsign ?? route.route_long_name ?? "Unknown";

                arrivals.Add(new ScheduleModel
                {
                    RouteNo = routeNo,
                    Destination = CleanDestination(rawDestination, routeNo),
                    ExpectedCountdown = minutesUntilArrival,
                    ScheduleStatus = "Scheduled"
                });
            }
        }
        catch
        {
            // Return empty list on error
        }

        return arrivals;
    }

    private static string GetScheduleStatus(int delaySeconds)
    {
        if (delaySeconds < -60)
            return "Early";
        else if (delaySeconds > 300)
            return "Late";
        else if (delaySeconds > 60)
            return "Delayed";
        else
            return "On Time";
    }

    private async void ToggleSaveBusStop()
    {
        var stop = new SavedStopModel
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

    private static string CleanDestination(string? destination, string? routeNo)
    {
        if (string.IsNullOrEmpty(destination))
            return "Unknown";

        if (string.IsNullOrEmpty(routeNo))
            return destination;

        var trimmed = destination.Trim();
        var route = routeNo.Trim();

        if (trimmed.StartsWith(route + " "))
            return trimmed[(route.Length + 1)..].Trim();

        if (trimmed.StartsWith(route + "-"))
            return trimmed[(route.Length + 1)..].Trim();

        if (trimmed.Length > route.Length + 1)
        {
            var firstWord = trimmed.Split(' ', '-')[0];
            if (firstWord.TrimStart('0') == route.TrimStart('0'))
            {
                var separatorIndex = trimmed.IndexOfAny([' ', '-']);
                if (separatorIndex > 0 && separatorIndex < trimmed.Length - 1)
                    return trimmed[(separatorIndex + 1)..].Trim();
            }
        }

        return trimmed;
    }
}