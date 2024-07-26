namespace MyStop.Models
{
    public class Schedule : BaseModel
    {
        int expectedCountdown;
        public int ExpectedCountdown
        {
            get => expectedCountdown;
            set { expectedCountdown = value; NotifyPropertyChanged(nameof(ExpectedCountdown)); }
        }

        string routeNo;
        public string RouteNo
        {
            get => routeNo;
            set { routeNo = value; NotifyPropertyChanged(nameof(RouteNo)); }
        }

        string destination;
        public string Destination
        {
            get => destination;
            set { destination = value; NotifyPropertyChanged(nameof(Destination)); }
        }

        string scheduleStatus;
        public string ScheduleStatus
        {
            get => scheduleStatus;
            set { scheduleStatus = value; NotifyPropertyChanged(nameof(ScheduleStatus)); }
        }
    }
}