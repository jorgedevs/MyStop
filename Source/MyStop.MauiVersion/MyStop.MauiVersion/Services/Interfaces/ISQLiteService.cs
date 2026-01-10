using MyStop.MauiVersion.CSVs;
using MyStop.MauiVersion.Model;

namespace MyStop.MauiVersion.Services.Interfaces;

public interface ISQLiteService
{
    public Task SaveAgency(Agency agency);

    public Task SaveCalendars(List<Calendar> calendars);

    public Task SaveCalendarDates(List<CalendarDate> calendarDates);

    public Task SaveDirectionNamesExceptions(List<DirectionNamesException> directionNamesExceptions);

    public Task SaveDirections(List<Direction> directions);

    public Task SaveFeedInfo(FeedInfo feedInfo);

    public Task SaveRouteNamesExceptions(List<RouteNamesException> routeNamesExceptions);

    public Task SaveRoutes(List<Route> routes);

    public Task SaveShapes(List<Shape> shapes);

    public Task SaveSignupPeriods(List<SignupPeriod> signupPeriods);

    public Task SaveStopOrderExceptions(List<StopOrderException> stopOrderExceptions);

    public Task SaveStops(List<Stop> stops);

    public Task SaveStopTimes(List<StopTime> stopTimes);

    public Task SaveTransfers(List<Transfer> transfers);

    public Task SaveTrips(List<Trip> trips);

    public bool IsStop(string stopCode);

    public Stop GetStopInfo(string stopCode);

    public bool IsSavedStop(string stopCode);

    public Task SaveStop(SavedStopModel stop);

    public Task RemoveStop(SavedStopModel stop);

    public List<SavedStopModel> GetSavedStops();

    public Task<bool> HasGtfsDataAsync();

    public Task<List<StopTime>> GetStopTimesForStopAsync(string stopId);

    public Task<Trip> GetTripAsync(string tripId);

    public Task<Route> GetRouteAsync(string routeId);
}