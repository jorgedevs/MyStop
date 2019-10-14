namespace MyStop.Models
{
    public class Schedule : BaseModel
    {
        private string routeNo;
        public string RouteNo
        {
            get { return routeNo; }
            set { routeNo = value; NotifyPropertyChanged("RouteNo"); }
        }

        private string destination;
        public string Destination
        {
            get { return destination; }
            set { destination = value; NotifyPropertyChanged("Destination"); }
        }

        private int expectedCountdown;
        public int ExpectedCountdown
        {
            get { return expectedCountdown; }
            set { expectedCountdown = value; NotifyPropertyChanged("ExpectedCountdown"); }
        }

        private string scheduleStatus;
        public string ScheduleStatus
        {
            get { return scheduleStatus; }
            set { scheduleStatus = value; NotifyPropertyChanged("ScheduleStatus"); }
        }
    }
}
