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

    // Total number of files to process during GTFS loading
    private const int TotalGtfsFiles = 14;

    bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set { isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
    }

    bool isLoadingGtfs;
    public bool IsLoadingGtfs
    {
        get => isLoadingGtfs;
        set { isLoadingGtfs = value; OnPropertyChanged(nameof(IsLoadingGtfs)); }
    }

    double loadingProgress;
    public double LoadingProgress
    {
        get => loadingProgress;
        set { loadingProgress = value; OnPropertyChanged(nameof(LoadingProgress)); }
    }

    string loadingStatusText;
    public string LoadingStatusText
    {
        get => loadingStatusText;
        set { loadingStatusText = value; OnPropertyChanged(nameof(LoadingStatusText)); }
    }

    string currentFileName;
    public string CurrentFileName
    {
        get => currentFileName;
        set { currentFileName = value; OnPropertyChanged(nameof(CurrentFileName)); }
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
        LoadingStatusText = "Checking data...";
        CurrentFileName = "";
        LoadingProgress = 0;
        IsLoadingGtfs = true; // Start with loading screen visible for seamless splash transition

        GetStopInfoCommand = new Command(
            async () => await GetStopInfoCommandExecute());

        GoToFavoriteStops = new Command(
            async () => await Shell.Current.GoToAsync(nameof(FavouriteStopsPage)));

        _ = Task.Run(async () => await LoadGtfsDataAsync());
    }

    private void UpdateProgress(int fileNumber, string fileName)
    {
        LoadingProgress = (double)fileNumber / TotalGtfsFiles;
        CurrentFileName = fileName;
        LoadingStatusText = $"Processing {fileNumber} of {TotalGtfsFiles}...";
        Debug.WriteLine($"{fileNumber}) Processing {fileName}");
    }

    private async Task LoadGtfsDataAsync()
    {
        try
        {
            Debug.WriteLine("Checking if GTFS data already exists...");
            LoadingStatusText = "Checking existing data...";
            CurrentFileName = "";

            // Check if data already exists in the database
            bool hasData = await _sqliteService.HasGtfsDataAsync();
            if (hasData)
            {
                Debug.WriteLine("GTFS data already exists in database. Skipping download.");
                IsLoadingGtfs = false;
                return;
            }

            // Show loading overlay
            IsLoadingGtfs = true;
            LoadingProgress = 0;

            Debug.WriteLine("Starting GTFS data loading on background thread...");
            LoadingStatusText = "Downloading GTFS data...";
            CurrentFileName = "google_transit.zip";

            string gtfsUrl = "https://gtfs-static.translink.ca/gtfs/google_transit.zip";
            string localPath = Path.Combine(FileSystem.AppDataDirectory, "GTFS");
            string gtfsFolder = Path.Combine(localPath, "unzipped");

            // Download and extract GTFS data
            await _gtfsService.DownloadAndExtractGtfsAsync(gtfsUrl, localPath);
            Debug.WriteLine("GTFS data downloaded and extracted.");

            // Parse and save each file type with progress updates
            UpdateProgress(1, "agency.txt");
            var agency = _gtfsService.ParseAgency(Path.Combine(gtfsFolder, "agency.txt"));
            await _sqliteService.SaveAgency(agency);

            UpdateProgress(2, "calendar.txt");
            var calendar = _gtfsService.ParseCalendar(Path.Combine(gtfsFolder, "calendar.txt"));
            await _sqliteService.SaveCalendars(calendar);

            UpdateProgress(3, "calendar_dates.txt");
            var calendarDates = _gtfsService.ParseCalendarDates(Path.Combine(gtfsFolder, "calendar_dates.txt"));
            await _sqliteService.SaveCalendarDates(calendarDates);

            UpdateProgress(4, "direction_names_exceptions.txt");
            var directionNamesExceptions = _gtfsService.ParseDirectionNamesExceptions(Path.Combine(gtfsFolder, "direction_names_exceptions.txt"));
            await _sqliteService.SaveDirectionNamesExceptions(directionNamesExceptions);

            UpdateProgress(5, "feed_info.txt");
            var feedInfo = _gtfsService.ParseFeedInfo(Path.Combine(gtfsFolder, "feed_info.txt"));
            await _sqliteService.SaveFeedInfo(feedInfo);

            UpdateProgress(6, "route_names_exceptions.txt");
            var routeNamesExceptions = _gtfsService.ParseRouteNamesExceptions(Path.Combine(gtfsFolder, "route_names_exceptions.txt"));
            await _sqliteService.SaveRouteNamesExceptions(routeNamesExceptions);

            UpdateProgress(7, "routes.txt");
            var routes = _gtfsService.ParseRoutes(Path.Combine(gtfsFolder, "routes.txt"));
            await _sqliteService.SaveRoutes(routes);

            UpdateProgress(8, "shapes.txt");
            var shapes = _gtfsService.ParseShapes(Path.Combine(gtfsFolder, "shapes.txt"));
            await _sqliteService.SaveShapes(shapes);

            UpdateProgress(9, "signup_periods.txt");
            var signUpPeriods = _gtfsService.ParseSignupPeriods(Path.Combine(gtfsFolder, "signup_periods.txt"));
            await _sqliteService.SaveSignupPeriods(signUpPeriods);

            UpdateProgress(10, "stop_order_exceptions.txt");
            var stopOrderExceptions = _gtfsService.ParseStopOrderExceptions(Path.Combine(gtfsFolder, "stop_order_exceptions.txt"));
            await _sqliteService.SaveStopOrderExceptions(stopOrderExceptions);

            UpdateProgress(11, "stops.txt");
            var stops = _gtfsService.ParseStops(Path.Combine(gtfsFolder, "stops.txt"));
            await _sqliteService.SaveStops(stops);

            UpdateProgress(12, "stop_times.txt");
            var stopTimes = _gtfsService.ParseStopTimes(Path.Combine(gtfsFolder, "stop_times.txt"));
            await _sqliteService.SaveStopTimes(stopTimes);

            UpdateProgress(13, "transfers.txt");
            var transfers = _gtfsService.ParseTransfers(Path.Combine(gtfsFolder, "transfers.txt"));
            await _sqliteService.SaveTransfers(transfers);

            UpdateProgress(14, "trips.txt");
            var trips = _gtfsService.ParseTrips(Path.Combine(gtfsFolder, "trips.txt"));
            await _sqliteService.SaveTrips(trips);

            // Complete
            LoadingProgress = 1.0;
            LoadingStatusText = "Complete!";
            CurrentFileName = "";
            Debug.WriteLine("GTFS data loading completed successfully!");

            // Hide loading overlay
            IsLoadingGtfs = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading GTFS data: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            LoadingStatusText = "Error loading data";
            CurrentFileName = ex.Message;
            IsLoadingGtfs = false;
        }
    }

    private async Task GetStopInfoCommandExecute()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        try
        {
            if (string.IsNullOrWhiteSpace(BusStopNumber) || BusStopNumber.Contains("."))
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
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Validation Error", "Stop not found", "OK");
                    }
                }
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