using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services.Interfaces;
using System.Diagnostics;
using TransitRealtime;

namespace MyStop.MauiVersion.Services;

public interface IGtfsLiveService
{
    /// <summary>
    /// Gets realtime trip updates for a specific stop.
    /// </summary>
    /// <param name="stopId">The GTFS stop_id to get arrivals for.</param>
    /// <returns>List of realtime arrival information for the stop.</returns>
    Task<List<RealtimeArrivalModel>> GetRealtimeArrivalsForStopAsync(string stopId);

    /// <summary>
    /// Gets all current trip updates from the realtime feed.
    /// </summary>
    Task<Dictionary<string, TripUpdateInfo>> GetAllTripUpdatesAsync();

    Task TripUpdate();
    Task PositionUpdate();
    Task ServiceAlert();
}

/// <summary>
/// Contains parsed trip update information from GTFS realtime.
/// </summary>
public class TripUpdateInfo
{
    public string? TripId { get; set; }
    public string? RouteId { get; set; }
    public string? Headsign { get; set; }
    public List<StopTimeUpdateInfo> StopTimeUpdates { get; set; } = new();
}

/// <summary>
/// Contains stop time update information from a trip update.
/// </summary>
public class StopTimeUpdateInfo
{
    public string? StopId { get; set; }
    public int StopSequence { get; set; }
    public long? ArrivalTime { get; set; }
    public int? ArrivalDelay { get; set; }
    public long? DepartureTime { get; set; }
    public int? DepartureDelay { get; set; }
    public string? ScheduleRelationship { get; set; }
}

public class GtfsLiveService : IGtfsLiveService
{
    private readonly ISQLiteService _sqliteService;

    const string API_KEY = "API_KEY"; // TODO: Move to secure configuration

    // Cache for trip updates to avoid frequent API calls
    private Dictionary<string, TripUpdateInfo>? _tripUpdatesCache;
    private DateTime _lastCacheUpdate = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromSeconds(30);
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    public GtfsLiveService(ISQLiteService sqliteService)
    {
        _sqliteService = sqliteService;
    }

    public async Task<List<RealtimeArrivalModel>> GetRealtimeArrivalsForStopAsync(string stopId)
    {
        var arrivals = new List<RealtimeArrivalModel>();

        try
        {
            var tripUpdates = await GetAllTripUpdatesAsync();

            foreach (var tripUpdate in tripUpdates.Values)
            {
                // Find stop time updates for this stop
                var stopTimeUpdate = tripUpdate.StopTimeUpdates
                    .FirstOrDefault(stu => stu.StopId == stopId);

                if (stopTimeUpdate != null)
                {
                    // Get route info from database
                    string? routeShortName = null;
                    string? headsign = tripUpdate.Headsign;

                    if (!string.IsNullOrEmpty(tripUpdate.RouteId))
                    {
                        var route = await _sqliteService.GetRouteAsync(tripUpdate.RouteId);
                        routeShortName = route?.route_short_name ?? route?.route_id;
                    }

                    if (string.IsNullOrEmpty(headsign) && !string.IsNullOrEmpty(tripUpdate.TripId))
                    {
                        var trip = await _sqliteService.GetTripAsync(tripUpdate.TripId);
                        headsign = trip?.trip_headsign;
                    }

                    arrivals.Add(new RealtimeArrivalModel
                    {
                        TripId = tripUpdate.TripId,
                        RouteId = tripUpdate.RouteId,
                        StopId = stopId,
                        StopSequence = stopTimeUpdate.StopSequence,
                        ArrivalTime = stopTimeUpdate.ArrivalTime,
                        ArrivalDelay = stopTimeUpdate.ArrivalDelay,
                        DepartureTime = stopTimeUpdate.DepartureTime,
                        DepartureDelay = stopTimeUpdate.DepartureDelay,
                        Headsign = headsign,
                        RouteShortName = routeShortName,
                        IsRealtime = true,
                        ScheduleRelationship = stopTimeUpdate.ScheduleRelationship
                    });
                }
            }

            Debug.WriteLine($"Found {arrivals.Count} realtime arrivals for stop {stopId}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting realtime arrivals: {ex.Message}");
        }

        return arrivals;
    }

    public async Task<Dictionary<string, TripUpdateInfo>> GetAllTripUpdatesAsync()
    {
        await _cacheLock.WaitAsync();
        try
        {
            // Return cached data if still valid
            if (_tripUpdatesCache != null && DateTime.Now - _lastCacheUpdate < _cacheExpiry)
            {
                return _tripUpdatesCache;
            }

            // Run network and parsing on background thread to avoid Android NetworkOnMainThreadException
            var tripUpdates = await Task.Run(async () =>
            {
                var updates = new Dictionary<string, TripUpdateInfo>();

                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                // Download the data as bytes first
                var bytes = await httpClient.GetByteArrayAsync(
                    $"https://gtfsapi.translink.ca/v3/gtfsrealtime?apikey={API_KEY}");

                // Parse the protobuf message from bytes (synchronous but now on background thread)
                var feed = FeedMessage.Parser.ParseFrom(bytes);

                foreach (var entity in feed.Entity)
                {
                    if (entity.TripUpdate != null)
                    {
                        var tripUpdate = entity.TripUpdate;
                        var tripId = tripUpdate.Trip?.TripId;

                        if (string.IsNullOrEmpty(tripId))
                            continue;

                        var info = new TripUpdateInfo
                        {
                            TripId = tripId,
                            RouteId = tripUpdate.Trip?.RouteId,
                            StopTimeUpdates = new List<StopTimeUpdateInfo>()
                        };

                        foreach (var stu in tripUpdate.StopTimeUpdate)
                        {
                            info.StopTimeUpdates.Add(new StopTimeUpdateInfo
                            {
                                StopId = stu.StopId,
                                StopSequence = (int)stu.StopSequence,
                                ArrivalTime = stu.Arrival?.Time,
                                ArrivalDelay = stu.Arrival?.Delay,
                                DepartureTime = stu.Departure?.Time,
                                DepartureDelay = stu.Departure?.Delay,
                                ScheduleRelationship = stu.ScheduleRelationship.ToString()
                            });
                        }

                        updates[tripId] = info;
                    }
                }

                return updates;
            });

            // Update cache
            _tripUpdatesCache = tripUpdates;
            _lastCacheUpdate = DateTime.Now;

            Debug.WriteLine($"Fetched {tripUpdates.Count} trip updates from realtime feed");

            return tripUpdates;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error fetching trip updates: {ex.Message}");

            // Return cached data if available, even if expired
            return _tripUpdatesCache ?? new Dictionary<string, TripUpdateInfo>();
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task TripUpdate()
    {
        try
        {
            var updates = await GetAllTripUpdatesAsync();
            foreach (var update in updates.Values)
            {
                Debug.WriteLine($"Trip ID: {update.TripId}, Route: {update.RouteId}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task PositionUpdate()
    {
        try
        {
            await Task.Run(async () =>
            {
                using var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(
                    $"https://gtfsapi.translink.ca/v3/gtfsposition?apikey={API_KEY}");

                var feed = FeedMessage.Parser.ParseFrom(bytes);

                foreach (var entity in feed.Entity)
                {
                    if (entity.Vehicle != null)
                    {
                        var vehicle = entity.Vehicle;
                        Debug.WriteLine($"Vehicle ID: {vehicle.Vehicle?.Id}, " +
                            $"Trip: {vehicle.Trip?.TripId}, " +
                            $"Position: {vehicle.Position?.Latitude}, {vehicle.Position?.Longitude}");
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task ServiceAlert()
    {
        try
        {
            await Task.Run(async () =>
            {
                using var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(
                    $"https://gtfsapi.translink.ca/v3/gtfsalerts?apikey={API_KEY}");

                var feed = FeedMessage.Parser.ParseFrom(bytes);

                foreach (var entity in feed.Entity)
                {
                    if (entity.Alert != null)
                    {
                        var alert = entity.Alert;
                        Debug.WriteLine($"Alert: {alert.HeaderText?.Translation?.FirstOrDefault()?.Text}");
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
    }
}
