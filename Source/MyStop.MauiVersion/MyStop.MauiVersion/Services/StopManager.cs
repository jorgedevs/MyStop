using SQLite;

namespace MyStop.MauiVersion.Utils;

public class StopManager
{
    private SQLiteAsyncConnection conn;

    public StopManager()
    {

    }

    //public async Task Init()
    //{
    //    if (conn is not null)
    //        return;

    //    try
    //    {
    //        conn = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
    //        //await conn.DeleteAllAsync<Stop>();
    //        var result = await conn.CreateTableAsync<Stop>();

    //        if (result == CreateTableResult.Created)
    //        {
    //            await conn.InsertAllAsync(new List<Stop>
    //            {
    //                new Stop { Tag = "", Name = "NB BURRARD ST NS DAVIE ST", StopNo = "50075", Routes = "002, 032, 044, N22" },
    //                new Stop { Tag = "", Name = "BURRARD STN BAY 1", StopNo = "50043", Routes = "002, 032, 044, N22" },
    //            });
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }
    //}

    //public async Task<List<Stop>> GetStops()
    //{
    //    return await conn.Table<Stop>().ToListAsync();
    //}

    //public async Task<bool> IsStop(string stopNo)
    //{
    //    return await conn.Table<Stop>().Where(i => i.StopNo == stopNo).CountAsync() > 0;
    //}

    //public async Task<int> AddStop(Stop stop)
    //{
    //    return await conn.InsertAsync(stop);
    //}

    //public async Task<int> DeleteStop(Stop stop)
    //{
    //    return await conn.DeleteAsync(stop);
    //}

    //public int UpdateStops(List<Stop> stops)
    //{
    //    foreach (var stop in stops)
    //        conn.Update(stop);

    //    return 0;
    //}
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
