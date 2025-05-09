using MyStop.MauiVersion.CSVs;

namespace MyStop.MauiVersion.Services;

public interface IGtfsService
{
    public Task DownloadAndExtractGtfsAsync(string gtfsUrl, string targetFolderPath);

    public Agency ParseAgency(string filePath);

    public List<Calendar> ParseCalendar(string filePath);

    public List<CalendarDate> ParseCalendarDates(string filePath);

    public List<DirectionNamesException> ParseDirectionNamesExceptions(string filePath);

    public List<Direction> ParseDirections(string filePath);

    public FeedInfo ParseFeedInfo(string filePath);

    public List<RouteNamesException> ParseRouteNamesExceptions(string filePath);

    public List<Route> ParseRoutes(string filePath);

    public List<Shape> ParseShapes(string filePath);

    public List<SignupPeriod> ParseSignupPeriods(string filePath);

    public List<StopOrderException> ParseStopOrderExceptions(string filePath);

    public List<Stop> ParseStops(string filePath);

    public List<StopTime> ParseStopTimes(string filePath);

    public List<Transfer> ParseTransfers(string filePath);

    public List<Trip> ParseTrips(string filePath);
}