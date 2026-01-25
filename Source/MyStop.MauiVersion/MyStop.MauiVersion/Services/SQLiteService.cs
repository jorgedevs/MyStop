using MyStop.MauiVersion.CSVs;
using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services.Interfaces;
using SQLite;

namespace MyStop.MauiVersion.Services;

public class SQLiteService : ISQLiteService
{
    private SQLiteAsyncConnection connection;

    public SQLiteService()
    {
        connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        _ = Initialize();
    }

    public async Task Initialize()
    {
        if (connection is null)
        {
            return;
        }

        try
        {
            await connection.CreateTableAsync<Agency>();
            await connection.CreateTableAsync<Calendar>();
            await connection.CreateTableAsync<CalendarDate>();
            await connection.CreateTableAsync<DirectionNamesException>();
            await connection.CreateTableAsync<Direction>();
            await connection.CreateTableAsync<FeedInfo>();
            await connection.CreateTableAsync<RouteNamesException>();
            await connection.CreateTableAsync<Route>();
            await connection.CreateTableAsync<Shape>();
            await connection.CreateTableAsync<SignupPeriod>();
            await connection.CreateTableAsync<StopOrderException>();
            await connection.CreateTableAsync<Stop>();
            await connection.CreateTableAsync<StopTime>();
            await connection.CreateTableAsync<Transfer>();
            await connection.CreateTableAsync<Trip>();

            await connection.CreateTableAsync<SavedStopModel>();
            await connection.CreateTableAsync<BusAlertModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task SaveAgency(Agency agency)
    {
        await connection.InsertOrReplaceAsync(agency);
    }

    public async Task SaveCalendars(List<Calendar> calendars)
    {
        await connection.DeleteAllAsync<Calendar>();
        await connection.InsertAllAsync(calendars, true);
    }

    public async Task SaveCalendarDates(List<CalendarDate> calendarDates)
    {
        await connection.DeleteAllAsync<CalendarDate>();
        await connection.InsertAllAsync(calendarDates, true);
    }

    public async Task SaveDirectionNamesExceptions(List<DirectionNamesException> directionNamesExceptions)
    {
        await connection.DeleteAllAsync<DirectionNamesException>();
        await connection.InsertAllAsync(directionNamesExceptions);
    }

    public async Task SaveDirections(List<Direction> directions)
    {
        await connection.DeleteAllAsync<Direction>();
        await connection.InsertAllAsync(directions, true);
    }

    public async Task SaveFeedInfo(FeedInfo feedInfo)
    {
        await connection.InsertOrReplaceAsync(feedInfo);
    }

    public async Task SaveRouteNamesExceptions(List<RouteNamesException> routeNamesExceptions)
    {
        await connection.DeleteAllAsync<RouteNamesException>();
        await connection.InsertAllAsync(routeNamesExceptions);
    }

    public async Task SaveRoutes(List<Route> routes)
    {
        await connection.DeleteAllAsync<Route>();
        await connection.InsertAllAsync(routes, true);
    }

    public async Task SaveShapes(List<Shape> shapes)
    {
        await connection.DeleteAllAsync<Shape>();
        await connection.InsertAllAsync(shapes, true);
    }

    public async Task SaveSignupPeriods(List<SignupPeriod> signupPeriods)
    {
        await connection.DeleteAllAsync<SignupPeriod>();
        await connection.InsertAllAsync(signupPeriods, true);
    }

    public async Task SaveStopOrderExceptions(List<StopOrderException> stopOrderExceptions)
    {
        await connection.DeleteAllAsync<StopOrderException>();
        await connection.InsertAllAsync(stopOrderExceptions, true);
    }

    public async Task SaveStops(List<Stop> stops)
    {
        await connection.DeleteAllAsync<Stop>();
        await connection.InsertAllAsync(stops, true);
    }

    public async Task SaveStopTimes(List<StopTime> stopTimes)
    {
        try
        {
            await connection.DeleteAllAsync<StopTime>();
            await connection.InsertAllAsync(stopTimes, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task SaveTransfers(List<Transfer> transfers)
    {
        try
        {
            await connection.DeleteAllAsync<Transfer>();
            await connection.InsertAllAsync(transfers, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task SaveTrips(List<Trip> trips)
    {
        try
        {
            await connection.DeleteAllAsync<Trip>();
            await connection.InsertAllAsync(trips, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public bool IsStop(string stopCode)
    {
        if (string.IsNullOrEmpty(stopCode))
            return false;

        return connection.Table<Stop>().Where(i => i.stop_code == stopCode).CountAsync().Result > 0;
    }

    public Stop GetStopInfo(string stopCode)
    {
        return connection.Table<Stop>().FirstOrDefaultAsync(i => i.stop_code == stopCode).Result;
    }

    public bool IsSavedStop(string stopCode)
    {
        if (string.IsNullOrEmpty(stopCode))
        {
            return false;
        }

        return connection.Table<SavedStopModel>().Where(i => i.StopNo == stopCode).CountAsync().Result > 0;
    }

    public async Task<List<StopModel>> GetStops()
    {
        return await connection.Table<StopModel>().ToListAsync();
    }

    public async Task SaveStop(SavedStopModel stop)
    {
        if (stop == null)
        {
            return;
        }

        await connection.InsertOrReplaceAsync(stop);
    }

    public async Task RemoveStop(SavedStopModel stop)
    {
        if (stop == null)
        {
            return;
        }
        await connection.DeleteAsync(stop);
    }

    public List<SavedStopModel> GetSavedStops()
    {
        return connection.Table<SavedStopModel>().ToListAsync().Result;
    }

    public async Task<bool> HasGtfsDataAsync()
    {
        try
        {
            var stopCount = await connection.Table<Stop>().CountAsync();
            return stopCount > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<StopTime>> GetStopTimesForStopAsync(string stopId)
    {
        return await connection.Table<StopTime>()
            .Where(st => st.stop_id == stopId)
            .ToListAsync();
    }

    public async Task<Trip> GetTripAsync(string tripId)
    {
        return await connection.Table<Trip>()
            .FirstOrDefaultAsync(t => t.trip_id == tripId);
    }

    public async Task<Route> GetRouteAsync(string routeId)
    {
        return await connection.Table<Route>()
            .FirstOrDefaultAsync(r => r.route_id == routeId);
    }

    public async Task<List<Calendar>> GetCalendarsAsync()
    {
        return await connection.Table<Calendar>().ToListAsync();
    }

    public async Task<List<CalendarDate>> GetCalendarDatesAsync()
    {
        return await connection.Table<CalendarDate>().ToListAsync();
    }

    public async Task<HashSet<string>> GetActiveServiceIdsAsync(DateTime date)
    {
        var activeServices = new HashSet<string>();

        var dateString = date.ToString("yyyyMMdd");

        var dayOfWeek = date.DayOfWeek;

        var calendars = await GetCalendarsAsync();

        foreach (var calendar in calendars)
        {
            if (string.Compare(dateString, calendar.start_date) >= 0 &&
                string.Compare(dateString, calendar.end_date) <= 0)
            {
                bool runsToday = dayOfWeek switch
                {
                    DayOfWeek.Monday => calendar.monday == "1",
                    DayOfWeek.Tuesday => calendar.tuesday == "1",
                    DayOfWeek.Wednesday => calendar.wednesday == "1",
                    DayOfWeek.Thursday => calendar.thursday == "1",
                    DayOfWeek.Friday => calendar.friday == "1",
                    DayOfWeek.Saturday => calendar.saturday == "1",
                    DayOfWeek.Sunday => calendar.sunday == "1",
                    _ => false
                };

                if (runsToday && !string.IsNullOrEmpty(calendar.service_id))
                {
                    activeServices.Add(calendar.service_id);
                }
            }
        }

        var calendarDates = await GetCalendarDatesAsync();

        foreach (var calendarDate in calendarDates)
        {
            if (calendarDate.date == dateString && !string.IsNullOrEmpty(calendarDate.service_id))
            {
                if (calendarDate.exception_type == "1")
                {
                    activeServices.Add(calendarDate.service_id);
                }
                else if (calendarDate.exception_type == "2")
                {
                    activeServices.Remove(calendarDate.service_id);
                }
            }
        }

        return activeServices;
    }

    public async Task<List<string>> GetRouteNumbersForStopAsync(string stopCode)
    {
        var routeNumbers = new HashSet<string>();

        try
        {
            var stopInfo = GetStopInfo(stopCode);
            if (stopInfo == null || string.IsNullOrEmpty(stopInfo.stop_id))
                return [];

            var stopTimes = await GetStopTimesForStopAsync(stopInfo.stop_id);
            if (stopTimes.Count == 0)
                return [];

            var tripIds = stopTimes
                .Take(100)
                .Select(st => st.trip_id)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            if (tripIds.Count == 0)
                return [];

            var trips = await connection.Table<Trip>()
                .Where(t => tripIds.Contains(t.trip_id))
                .ToListAsync();

            var routeIds = trips
                .Select(t => t.route_id)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            if (routeIds.Count == 0)
                return [];

            var routes = await connection.Table<Route>()
                .Where(r => routeIds.Contains(r.route_id))
                .ToListAsync();

            foreach (var route in routes)
            {
                if (!string.IsNullOrEmpty(route.route_short_name))
                {
                    routeNumbers.Add(route.route_short_name);
                }
            }
        }
        catch
        {
        }

        return routeNumbers.OrderBy(r => r).ToList();
    }

    // Bus Alert methods
    public async Task CreateBusAlertsTableAsync()
    {
        await connection.CreateTableAsync<BusAlertModel>();
    }

    public async Task SaveBusAlertAsync(BusAlertModel alert)
    {
        await connection.InsertAsync(alert);
    }

    public async Task UpdateBusAlertAsync(BusAlertModel alert)
    {
        await connection.UpdateAsync(alert);
    }

    public async Task<BusAlertModel?> GetBusAlertAsync(int alertId)
    {
        return await connection.Table<BusAlertModel>()
            .Where(a => a.Id == alertId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<BusAlertModel>> GetActiveAlertsForStopAsync(string stopCode)
    {
        return await connection.Table<BusAlertModel>()
            .Where(a => a.StopCode == stopCode && a.IsActive)
            .ToListAsync();
    }

    public async Task CleanupExpiredAlertsAsync()
    {
        var expiredCutoff = DateTime.Now.AddHours(-2);

        var expiredAlerts = await connection.Table<BusAlertModel>()
            .Where(a => a.ScheduledTime < expiredCutoff && a.IsActive)
            .ToListAsync();

        foreach (var alert in expiredAlerts)
        {
            alert.IsActive = false;
            await connection.UpdateAsync(alert);
        }
    }
}

public static class Constants
{
    public const string DatabaseFilename = "MyStopDatabase.db3";

    public const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}