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
}

public static class Constants
{
    public const string DatabaseFilename = "MyStopDatabase.db3";

    public const SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLiteOpenFlags.SharedCache;

    public static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
}