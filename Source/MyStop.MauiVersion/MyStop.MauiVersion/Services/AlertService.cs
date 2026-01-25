using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services.Interfaces;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

namespace MyStop.MauiVersion.Services;

public class AlertService : IAlertService
{
    private readonly ISQLiteService _sqliteService;
    private static int _nextNotificationId = 1000;

    public AlertService(ISQLiteService sqliteService)
    {
        _sqliteService = sqliteService;
        _ = Initialize();
    }

    private async Task Initialize()
    {
        // Ensure alerts table exists
        await _sqliteService.CreateBusAlertsTableAsync();

        // Clean up expired alerts on startup
        await CleanupExpiredAlertsAsync();
    }

    public async Task<bool> RequestPermissionsAsync()
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        return await LocalNotificationCenter.Current.AreNotificationsEnabled();
    }

    public async Task<BusAlertModel?> CreateAlertAsync(
        ScheduleModel schedule,
        string stopCode,
        int alertMinutesBefore,
        bool isContinuous = false)
    {
        // Validate
        if (schedule.ExpectedCountdown < alertMinutesBefore)
        {
            return null; // Can't alert if bus arrives before alert time
        }

        // Request permissions
        if (!await RequestPermissionsAsync())
        {
            return null;
        }

        var alert = new BusAlertModel
        {
            NotificationId = _nextNotificationId++,
            StopCode = stopCode,
            RouteNo = schedule.RouteNo ?? "?",
            Destination = schedule.Destination ?? "Unknown",
            AlertMinutesBefore = alertMinutesBefore,
            OriginalCountdown = schedule.ExpectedCountdown,
            CreatedAt = DateTime.Now,
            IsContinuous = isContinuous,
            IsActive = true
        };

        if (isContinuous)
        {
            // Schedule multiple notifications from alertMinutesBefore down to 0
            await ScheduleContinuousNotificationsAsync(alert, schedule.ExpectedCountdown);
        }
        else
        {
            // Schedule single notification
            int delayMinutes = schedule.ExpectedCountdown - alertMinutesBefore;
            alert.ScheduledTime = DateTime.Now.AddMinutes(delayMinutes);

            await ScheduleNotificationAsync(
                alert.NotificationId,
                alert.RouteNo,
                alert.Destination,
                alertMinutesBefore,
                delayMinutes);
        }

        // Save to database
        await _sqliteService.SaveBusAlertAsync(alert);

        return alert;
    }

    private async Task ScheduleNotificationAsync(
        int notificationId,
        string routeNo,
        string destination,
        int minutesAway,
        int delayMinutes)
    {
        var notification = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = $"Bus {routeNo} - {destination}",
            Description = minutesAway == 0
                ? "🚌 Your bus is arriving now!"
                : $"🚌 Arriving in {minutesAway} minute{(minutesAway != 1 ? "s" : "")}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddMinutes(delayMinutes)
            },
            Android = new AndroidOptions
            {
                IconSmallName = new AndroidIcon("appicon"),
                ChannelId = "mystop_alerts",
                Priority = AndroidPriority.High,
                VibrationPattern = [0, 500, 250, 500]
            },
            iOS = new iOSOptions
            {
                PlayForegroundSound = true
            }
        };

        await LocalNotificationCenter.Current.Show(notification);
    }

    private async Task ScheduleContinuousNotificationsAsync(
        BusAlertModel alert,
        int totalMinutes)
    {
        // Schedule notifications for each minute from alertMinutesBefore down to 0
        for (int minutesAway = alert.AlertMinutesBefore; minutesAway >= 0; minutesAway--)
        {
            int delayMinutes = totalMinutes - minutesAway;
            int notifId = alert.NotificationId + (alert.AlertMinutesBefore - minutesAway);

            await ScheduleNotificationAsync(
                notifId,
                alert.RouteNo,
                alert.Destination,
                minutesAway,
                delayMinutes);
        }

        // Update scheduled time to the first notification
        alert.ScheduledTime = DateTime.Now.AddMinutes(totalMinutes - alert.AlertMinutesBefore);
    }

    public async Task CancelAlertAsync(int alertId)
    {
        var alert = await _sqliteService.GetBusAlertAsync(alertId);
        if (alert == null) return;

        if (alert.IsContinuous)
        {
            // Cancel all continuous notifications
            for (int i = 0; i <= alert.AlertMinutesBefore; i++)
            {
                LocalNotificationCenter.Current.Cancel(alert.NotificationId + i);
            }
        }
        else
        {
            LocalNotificationCenter.Current.Cancel(alert.NotificationId);
        }

        alert.IsActive = false;
        await _sqliteService.UpdateBusAlertAsync(alert);
    }

    public async Task<List<BusAlertModel>> GetActiveAlertsForStopAsync(string stopCode)
    {
        return await _sqliteService.GetActiveAlertsForStopAsync(stopCode);
    }

    public async Task<BusAlertModel?> GetAlertForScheduleAsync(
        string stopCode,
        string routeNo,
        int countdown)
    {
        var alerts = await GetActiveAlertsForStopAsync(stopCode);

        // Match by route and approximate countdown (within 2 minutes tolerance)
        return alerts.FirstOrDefault(a =>
            a.RouteNo == routeNo &&
            Math.Abs(a.OriginalCountdown - countdown) <= 2);
    }

    public async Task CleanupExpiredAlertsAsync()
    {
        await _sqliteService.CleanupExpiredAlertsAsync();
    }
}
