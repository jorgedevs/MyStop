using TransitRealtime;

namespace MyStop.MauiVersion.Services;

public interface IGtfsLiveService
{
    public Task TripUpdate();

    public Task PositionUpdate();

    public Task ServiceAlert();
}

public class GtfsLiveService : IGtfsLiveService
{
    const string API_KEY = "API_KEY";

    public async Task TripUpdate()
    {
        try
        {
            using var httpClient = new HttpClient();
            using var stream = await httpClient.GetStreamAsync($"https://gtfsapi.translink.ca/v3/gtfsrealtime?apikey={API_KEY}");

            var feed = FeedMessage.Parser.ParseFrom(stream);

            foreach (var entity in feed.Entity)
            {
                if (entity.TripUpdate != null)
                {
                    var tripUpdate = entity.TripUpdate;
                    Console.WriteLine($"Trip ID: {tripUpdate.Trip.TripId}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}"); //Exception of type 'Android.OS.NetworkOnMainThreadException' was thrown.
        }
    }
    public async Task PositionUpdate()
    {
        try
        {
            using var httpClient = new HttpClient();
            using var stream = await httpClient.GetStreamAsync($"https://gtfsapi.translink.ca/v3/gtfsposition?apikey={API_KEY}");

            var feed = FeedMessage.Parser.ParseFrom(stream);

            foreach (var entity in feed.Entity)
            {
                if (entity.TripUpdate != null)
                {
                    var tripUpdate = entity.TripUpdate;
                    Console.WriteLine($"Trip ID: {tripUpdate.Trip.TripId}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task ServiceAlert()
    {
        try
        {
            using var httpClient = new HttpClient();
            using var stream = await httpClient.GetStreamAsync($"https://gtfsapi.translink.ca/v3/gtfsalerts?apikey={API_KEY}");

            var feed = FeedMessage.Parser.ParseFrom(stream);

            foreach (var entity in feed.Entity)
            {
                if (entity.TripUpdate != null)
                {
                    var tripUpdate = entity.TripUpdate;
                    Console.WriteLine($"Trip ID: {tripUpdate.Trip.TripId}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
