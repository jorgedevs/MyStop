using MyStop.MauiVersion.Services;
using MyStop.MauiVersion.Services.Interfaces;
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

        BusStopNumber = "50023"; //string.Empty;

        GetStopInfoCommand = new Command(
            async () => await GetStopInfoCommandExecute());

        GoToFavoriteStops = new Command(
            async () => await Shell.Current.GoToAsync(nameof(FavouriteStopsPage)));

        _ = Task.Run(async () => await LoadGtfsDataAsync());
    }

    public void Tests()
    {
        Task.Run(async () =>
        {
            await _gtfsLiveService.TripUpdate();
            await _gtfsLiveService.PositionUpdate();
            await _gtfsLiveService.ServiceAlert();
        });
    }

    private async Task LoadGtfsDataAsync()
    {
        try
        {
            Debug.WriteLine("Starting GTFS data loading on background thread...");

            string gtfsUrl = "https://gtfs-static.translink.ca/gtfs/google_transit.zip";
            string localPath = Path.Combine(FileSystem.AppDataDirectory, "GTFS");
            string gtfsFolder = Path.Combine(localPath, "unzipped");

            // Download and extract GTFS data
            await _gtfsService.DownloadAndExtractGtfsAsync(gtfsUrl, localPath);
            Debug.WriteLine("GTFS data downloaded and extracted.");

            // Parse and save each file type
            var agency = _gtfsService.ParseAgency(Path.Combine(gtfsFolder, "agency.txt"));
            await _sqliteService.SaveAgency(agency);
            Debug.WriteLine("1) Saved agency");

            var calendar = _gtfsService.ParseCalendar(Path.Combine(gtfsFolder, "calendar.txt"));
            await _sqliteService.SaveCalendars(calendar);
            Debug.WriteLine("2) Saved calendars");

            var calendarDates = _gtfsService.ParseCalendarDates(Path.Combine(gtfsFolder, "calendar_dates.txt"));
            await _sqliteService.SaveCalendarDates(calendarDates);
            Debug.WriteLine("3) Saved calendar dates");

            var directionNamesExceptions = _gtfsService.ParseDirectionNamesExceptions(Path.Combine(gtfsFolder, "direction_names_exceptions.txt"));
            await _sqliteService.SaveDirectionNamesExceptions(directionNamesExceptions);
            Debug.WriteLine("4) Saved direction names exceptions");

            var feedInfo = _gtfsService.ParseFeedInfo(Path.Combine(gtfsFolder, "feed_info.txt"));
            await _sqliteService.SaveFeedInfo(feedInfo);
            Debug.WriteLine("5) Saved feed info");

            var routeNamesExceptions = _gtfsService.ParseRouteNamesExceptions(Path.Combine(gtfsFolder, "route_names_exceptions.txt"));
            await _sqliteService.SaveRouteNamesExceptions(routeNamesExceptions);
            Debug.WriteLine("6) Saved route names exceptions");

            var routes = _gtfsService.ParseRoutes(Path.Combine(gtfsFolder, "routes.txt"));
            await _sqliteService.SaveRoutes(routes);
            Debug.WriteLine("7) Saved routes");

            var shapes = _gtfsService.ParseShapes(Path.Combine(gtfsFolder, "shapes.txt"));
            await _sqliteService.SaveShapes(shapes);
            Debug.WriteLine("8) Saved shapes");

            var signUpPeriods = _gtfsService.ParseSignupPeriods(Path.Combine(gtfsFolder, "signup_periods.txt"));
            await _sqliteService.SaveSignupPeriods(signUpPeriods);
            Debug.WriteLine("9) Saved signup periods");

            var stopOrderExceptions = _gtfsService.ParseStopOrderExceptions(Path.Combine(gtfsFolder, "stop_order_exceptions.txt"));
            await _sqliteService.SaveStopOrderExceptions(stopOrderExceptions);
            Debug.WriteLine("10) Saved stop order exceptions");

            var stops = _gtfsService.ParseStops(Path.Combine(gtfsFolder, "stops.txt"));
            await _sqliteService.SaveStops(stops);
            Debug.WriteLine("11) Saved stops");

            var stopTimes = _gtfsService.ParseStopTimes(Path.Combine(gtfsFolder, "stop_times.txt"));
            await _sqliteService.SaveStopTimes(stopTimes);
            Debug.WriteLine("12) Saved stop times");

            var transfers = _gtfsService.ParseTransfers(Path.Combine(gtfsFolder, "transfers.txt"));
            await _sqliteService.SaveTransfers(transfers);
            Debug.WriteLine("13) Saved transfers");

            var trips = _gtfsService.ParseTrips(Path.Combine(gtfsFolder, "trips.txt"));
            await _sqliteService.SaveTrips(trips);
            Debug.WriteLine("14) Saved trips");

            Debug.WriteLine("GTFS data loading completed successfully!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading GTFS data: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private async Task Initialize()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;

        string gtfsUrl = "https://gtfs-static.translink.ca/gtfs/google_transit.zip";
        string localPath = Path.Combine(FileSystem.AppDataDirectory, "GTFS");

        await _gtfsService.DownloadAndExtractGtfsAsync(gtfsUrl, localPath);

        string gtfsFolder = Path.Combine(FileSystem.AppDataDirectory, "GTFS", "unzipped");

        //var tasks = new List<Task>
        //{
        //    Task.Run(async () =>
        //    {
        var agency = _gtfsService.ParseAgency(Path.Combine(gtfsFolder, "agency.txt"));
        await _sqliteService.SaveAgency(agency);
        Debug.WriteLine("(1 - Saving agencies...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var calendar = _gtfsService.ParseCalendar(Path.Combine(gtfsFolder, "calendar.txt"));
        await _sqliteService.SaveCalendars(calendar);
        Debug.WriteLine("(2 - Saving calendars...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var calendarDates = _gtfsService.ParseCalendarDates(Path.Combine(gtfsFolder, "calendar_dates.txt"));
        await _sqliteService.SaveCalendarDates(calendarDates);
        Debug.WriteLine("(3 - Saving calendar dates...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var directionNamesExceptions = _gtfsService.ParseDirectionNamesExceptions(Path.Combine(gtfsFolder, "direction_names_exceptions.txt"));
        await _sqliteService.SaveDirectionNamesExceptions(directionNamesExceptions);
        Debug.WriteLine("(4 - Saving direction names exceptions...");
        //    }),
        //    Task.Run(async () =>
        //    {
        //var directions = _gtfsService.ParseDirections(Path.Combine(gtfsFolder, "directions.txt"));
        //await _sqliteService.SaveDirections(directions);
        //Debug.WriteLine("(5 - Saving directions...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var feedInfo = _gtfsService.ParseFeedInfo(Path.Combine(gtfsFolder, "feed_info.txt"));
        await _sqliteService.SaveFeedInfo(feedInfo);
        Debug.WriteLine("(6 - Saving feed info...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var routeNamesExceptions = _gtfsService.ParseRouteNamesExceptions(Path.Combine(gtfsFolder, "route_names_exceptions.txt"));
        await _sqliteService.SaveRouteNamesExceptions(routeNamesExceptions);
        Debug.WriteLine("(7 - Saving route names exceptions...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var routes = _gtfsService.ParseRoutes(Path.Combine(gtfsFolder, "routes.txt"));
        await _sqliteService.SaveRoutes(routes);
        Debug.WriteLine("(8 - Saving routes...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var shapes = _gtfsService.ParseShapes(Path.Combine(gtfsFolder, "shapes.txt"));
        await _sqliteService.SaveShapes(shapes);
        Debug.WriteLine("(9 - Saving shapes...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var signUpPeriods = _gtfsService.ParseSignupPeriods(Path.Combine(gtfsFolder, "signup_periods.txt"));
        await _sqliteService.SaveSignupPeriods(signUpPeriods);
        Debug.WriteLine("(10 - Saving signup periods...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var stopOrderExceptions = _gtfsService.ParseStopOrderExceptions(Path.Combine(gtfsFolder, "stop_order_exceptions.txt"));
        await _sqliteService.SaveStopOrderExceptions(stopOrderExceptions);
        Debug.WriteLine("(11 - Saving stop order exceptions...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var stops = _gtfsService.ParseStops(Path.Combine(gtfsFolder, "stops.txt"));
        await _sqliteService.SaveStops(stops);
        Debug.WriteLine("(12 - Saving stops...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var stopTimes = _gtfsService.ParseStopTimes(Path.Combine(gtfsFolder, "stop_times.txt"));
        await _sqliteService.SaveStopTimes(stopTimes);
        Debug.WriteLine("(13 - Saving stop times...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var transfers = _gtfsService.ParseTransfers(Path.Combine(gtfsFolder, "transfers.txt"));
        await _sqliteService.SaveTransfers(transfers);
        Debug.WriteLine("(14 - Saving transfers...");
        //    }),
        //    Task.Run(async () =>
        //    {
        var trips = _gtfsService.ParseTrips(Path.Combine(gtfsFolder, "trips.txt"));
        await _sqliteService.SaveTrips(trips);
        Debug.WriteLine("(15 - Saving trips...");
        //    })
        //};

        //await Task.WhenAll(tasks);

        IsBusy = false;
    }

    private async Task GetStopInfoCommandExecute()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        try
        {
            //if (BusStopNumber.Length == 0 || BusStopNumber.Contains("."))
            //{
            //    if (Application.Current?.MainPage != null)
            //    {
            //        await Application.Current.MainPage.DisplayAlert("Validation Error", "Please enter a valid stop code.", "OK");
            //    }
            //    else
            //    {
            //        Debug.WriteLine("MainPage is null. Cannot display alert.");
            //    }
            //}
            //else
            //{
            //    bool stopFound = _sqliteService.IsStop(BusStopNumber);

            //    if (stopFound)
            //    {
            await Shell.Current.GoToAsync($"{nameof(BusArrivalsPage)}?BusStopNumber={BusStopNumber}");
            //    }
            //    else
            //    {
            //        await Application.Current.MainPage.DisplayAlert("Validation Error", "Stop not found", "OK");
            //    }
            //}
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