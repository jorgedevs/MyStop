namespace MyStop.MauiVersion.Model;

public class ScheduleModel : BaseModel
{
    int expectedCountdown;
    public int ExpectedCountdown
    {
        get => expectedCountdown;
        set
        {
            expectedCountdown = value;
            NotifyPropertyChanged(nameof(ExpectedCountdown));
        }
    }

    string? routeNo;
    public string? RouteNo
    {
        get => routeNo;
        set
        {
            routeNo = value;
            NotifyPropertyChanged(nameof(RouteNo));
        }
    }

    string? destination;
    public string? Destination
    {
        get => destination;
        set
        {
            destination = value;
            NotifyPropertyChanged(nameof(Destination));
        }
    }

    string? scheduleStatus;
    public string? ScheduleStatus
    {
        get => scheduleStatus;
        set
        {
            scheduleStatus = value;
            NotifyPropertyChanged(nameof(ScheduleStatus));
        }
    }

    bool hasAlert;
    public bool HasAlert
    {
        get => hasAlert;
        set
        {
            hasAlert = value;
            NotifyPropertyChanged(nameof(HasAlert));
        }
    }

    int? alertId;
    public int? AlertId
    {
        get => alertId;
        set
        {
            alertId = value;
            NotifyPropertyChanged(nameof(AlertId));
        }
    }

    string? uniqueKey;
    public string? UniqueKey
    {
        get => uniqueKey;
        set
        {
            uniqueKey = value;
            NotifyPropertyChanged(nameof(UniqueKey));
        }
    }
}