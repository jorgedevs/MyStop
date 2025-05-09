using MyStop.MauiVersion.Services;
using MyStop.MauiVersion.View;
using System.Diagnostics;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class MainViewModel : BaseViewModel
{
    private readonly IGtfsService _gtfsService;
    private readonly ISQLiteService _sqliteService;
    private readonly IGtfsLiveService _gtfsLiveService;

    public const string INVALID_CODE_ENTERED = "INVALID_CODE_ENTERED";
    public const string STOP_NOT_FOUND = "STOP_NOT_FOUND";
    public const string STOP_FOUND = "STOP_FOUND";

    bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set { isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
    }

    string busStopNumber;
    public string BusStopNumber
    {
        get => busStopNumber;
        set { busStopNumber = value; OnPropertyChanged(nameof(BusStopNumber)); }
    }

    public ICommand GetStopInfoCommand { get; set; }

    public ICommand GoToFavoriteStops { get; set; }

    public MainViewModel(
        IGtfsService gtfsService,
        ISQLiteService sqliteService,
        IGtfsLiveService gtfsLiveService)
    {
        _gtfsService = gtfsService;
        _sqliteService = sqliteService;
        _gtfsLiveService = gtfsLiveService;

        BusStopNumber = "";

        GetStopInfoCommand = new Command(
           async () => await GetStopInfoCommandExecute(),
           () => !IsBusy);

        GoToFavoriteStops = new Command(
            async () => await Shell.Current.GoToAsync(nameof(FavouriteStopsPage)),
            () => !IsBusy);

        //_ = Initialize();
    }

    public async Task Tests()
    {
        await _gtfsLiveService.TripUpdate();
        await _gtfsLiveService.PositionUpdate();
        await _gtfsLiveService.ServiceAlert();
    }

    private async Task Initialize()
    {
        string gtfsUrl = "https://gtfs-static.translink.ca/gtfs/google_transit.zip";
        string localPath = Path.Combine(FileSystem.AppDataDirectory, "GTFS");

        await _gtfsService.DownloadAndExtractGtfsAsync(gtfsUrl, localPath);

        string gtfsFolder = Path.Combine(FileSystem.AppDataDirectory, "GTFS", "unzipped");

        var tasks = new List<Task>
        {
            Task.Run(async () =>
            {
                var agency = _gtfsService.ParseAgency(Path.Combine(gtfsFolder, "agency.txt"));
                await _sqliteService.SaveAgency(agency);
            }),
            Task.Run(async () =>
            {
                var calendar = _gtfsService.ParseCalendar(Path.Combine(gtfsFolder, "calendar.txt"));
                await _sqliteService.SaveCalendars(calendar);
            }),
            Task.Run(async () =>
            {
                var calendarDates = _gtfsService.ParseCalendarDates(Path.Combine(gtfsFolder, "calendar_dates.txt"));
                await _sqliteService.SaveCalendarDates(calendarDates);
            }),
            Task.Run(async () =>
            {
                var directionNamesExceptions = _gtfsService.ParseDirectionNamesExceptions(Path.Combine(gtfsFolder, "direction_names_exceptions.txt"));
                await _sqliteService.SaveDirectionNamesExceptions(directionNamesExceptions);
            }),
            Task.Run(async () =>
            {
                var directions = _gtfsService.ParseDirections(Path.Combine(gtfsFolder, "directions.txt"));
                await _sqliteService.SaveDirections(directions);
            }),
            Task.Run(async () =>
            {
                var feedInfo = _gtfsService.ParseFeedInfo(Path.Combine(gtfsFolder, "feed_info.txt"));
                await _sqliteService.SaveFeedInfo(feedInfo);
            }),
            Task.Run(async () =>
            {
                var routeNamesExceptions = _gtfsService.ParseRouteNamesExceptions(Path.Combine(gtfsFolder, "route_names_exceptions.txt"));
                await _sqliteService.SaveRouteNamesExceptions(routeNamesExceptions);
            }),
            Task.Run(async () =>
            {
                var routes = _gtfsService.ParseRoutes(Path.Combine(gtfsFolder, "routes.txt"));
                await _sqliteService.SaveRoutes(routes);
            }),
            Task.Run(async () =>
            {
                var shapes = _gtfsService.ParseShapes(Path.Combine(gtfsFolder, "shapes.txt"));
                await _sqliteService.SaveShapes(shapes);
            }),
            Task.Run(async () =>
            {
                var signUpPeriods = _gtfsService.ParseSignupPeriods(Path.Combine(gtfsFolder, "signup_periods.txt"));
                await _sqliteService.SaveSignupPeriods(signUpPeriods);
            }),
            Task.Run(async () =>
            {
                var stopOrderExceptions = _gtfsService.ParseStopOrderExceptions(Path.Combine(gtfsFolder, "stop_order_exceptions.txt"));
                await _sqliteService.SaveStopOrderExceptions(stopOrderExceptions);
            }),
            Task.Run(async () =>
            {
                var stops = _gtfsService.ParseStops(Path.Combine(gtfsFolder, "stops.txt"));
                await _sqliteService.SaveStops(stops);
            }),
            Task.Run(async () =>
            {
                var stopTimes = _gtfsService.ParseStopTimes(Path.Combine(gtfsFolder, "stop_times.txt"));
                await _sqliteService.SaveStopTimes(stopTimes);
            }),
            Task.Run(async () =>
            {
                var transfers = _gtfsService.ParseTransfers(Path.Combine(gtfsFolder, "transfers.txt"));
                await _sqliteService.SaveTransfers(transfers);
            }),
            Task.Run(async () =>
            {
                var trips = _gtfsService.ParseTrips(Path.Combine(gtfsFolder, "trips.txt"));
                await _sqliteService.SaveTrips(trips);
            })
        };

        await Task.WhenAll(tasks);
    }

    private async Task GetStopInfoCommandExecute()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        try
        {
            if (BusStopNumber.Length == 0 || BusStopNumber.Contains("."))
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Validation Error", "Please enter a valid stop code.", "OK");
                }
                else
                {
                    Debug.WriteLine("MainPage is null. Cannot display alert.");
                }
            }
            else
            {
                bool stopFound = _sqliteService.IsStop(BusStopNumber);

                if (stopFound)
                {
                    await Shell.Current.GoToAsync($"{nameof(BusArrivalsPage)}?BusStopNumber={BusStopNumber}");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Validation Error", "Stop not found", "OK");
                }
                //var stop = _sqliteService.GetStopInfo(BusStopNumber);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}