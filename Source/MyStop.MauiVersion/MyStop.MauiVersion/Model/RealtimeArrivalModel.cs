namespace MyStop.MauiVersion.Model;

/// <summary>
/// Represents a real-time arrival update from GTFS realtime feed.
/// </summary>
public class RealtimeArrivalModel
{
    /// <summary>
    /// The trip ID from the GTFS static data.
    /// </summary>
    public string? TripId { get; set; }

    /// <summary>
    /// The route ID from the GTFS static data.
    /// </summary>
    public string? RouteId { get; set; }

    /// <summary>
    /// The stop ID for this arrival.
    /// </summary>
    public string? StopId { get; set; }

    /// <summary>
    /// The stop sequence number within the trip.
    /// </summary>
    public int StopSequence { get; set; }

    /// <summary>
    /// Predicted arrival time as Unix timestamp (seconds since epoch).
    /// </summary>
    public long? ArrivalTime { get; set; }

    /// <summary>
    /// Delay in seconds (positive = late, negative = early).
    /// </summary>
    public int? ArrivalDelay { get; set; }

    /// <summary>
    /// Predicted departure time as Unix timestamp (seconds since epoch).
    /// </summary>
    public long? DepartureTime { get; set; }

    /// <summary>
    /// Delay in seconds for departure.
    /// </summary>
    public int? DepartureDelay { get; set; }

    /// <summary>
    /// The headsign/destination for this trip.
    /// </summary>
    public string? Headsign { get; set; }

    /// <summary>
    /// The route short name (bus number).
    /// </summary>
    public string? RouteShortName { get; set; }

    /// <summary>
    /// Indicates if this is realtime data or static schedule.
    /// </summary>
    public bool IsRealtime { get; set; }

    /// <summary>
    /// Schedule relationship (SCHEDULED, ADDED, UNSCHEDULED, CANCELED).
    /// </summary>
    public string? ScheduleRelationship { get; set; }
}
