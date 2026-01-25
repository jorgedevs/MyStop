using MyStop.MauiVersion.Model;

namespace MyStop.MauiVersion.Services.Interfaces;

public interface IAlertService
{
    /// <summary>
    /// Request notification permissions from the OS
    /// </summary>
    Task<bool> RequestPermissionsAsync();

    /// <summary>
    /// Create a new bus arrival alert
    /// </summary>
    Task<BusAlertModel?> CreateAlertAsync(
        ScheduleModel schedule,
        string stopCode,
        int alertMinutesBefore,
        bool isContinuous = false);

    /// <summary>
    /// Cancel an active alert
    /// </summary>
    Task CancelAlertAsync(int alertId);

    /// <summary>
    /// Get all active alerts for a specific stop
    /// </summary>
    Task<List<BusAlertModel>> GetActiveAlertsForStopAsync(string stopCode);

    /// <summary>
    /// Clean up expired alerts (housekeeping)
    /// </summary>
    Task CleanupExpiredAlertsAsync();

    /// <summary>
    /// Check if a specific schedule has an active alert
    /// </summary>
    Task<BusAlertModel?> GetAlertForScheduleAsync(string stopCode, string routeNo, int countdown);
}
