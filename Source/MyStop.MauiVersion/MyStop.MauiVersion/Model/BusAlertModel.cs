using SQLite;

namespace MyStop.MauiVersion.Model;

/// <summary>
/// Represents an active bus arrival alert set by the user
/// </summary>
[Table("BusAlerts")]
public class BusAlertModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Unique identifier for the notification (to cancel it later)
    /// </summary>
    public int NotificationId { get; set; }

    /// <summary>
    /// Stop code for this alert
    /// </summary>
    public string StopCode { get; set; } = string.Empty;

    /// <summary>
    /// Route number (bus number)
    /// </summary>
    public string RouteNo { get; set; } = string.Empty;

    /// <summary>
    /// Destination/headsign
    /// </summary>
    public string Destination { get; set; } = string.Empty;

    /// <summary>
    /// Minutes before arrival to alert (e.g., 3 = alert when 3 min away)
    /// </summary>
    public int AlertMinutesBefore { get; set; }

    /// <summary>
    /// Original expected countdown when alert was set
    /// </summary>
    public int OriginalCountdown { get; set; }

    /// <summary>
    /// When the alert was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the notification should fire
    /// </summary>
    public DateTime ScheduledTime { get; set; }

    /// <summary>
    /// Whether to send continuous notifications every minute until arrival
    /// </summary>
    public bool IsContinuous { get; set; }

    /// <summary>
    /// Whether this alert has been triggered/expired
    /// </summary>
    public bool IsActive { get; set; }
}
