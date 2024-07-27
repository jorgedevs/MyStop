using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Utils;
using System.Collections.ObjectModel;

namespace MyStop.MauiVersion.ViewModel;

public class BusArrivalsViewModel : BaseViewModel
{
    public Stop Stop { get; set; }
    public Schedule Schedule { get; set; }
    public ObservableCollection<Schedule> ArrivalTimes { get; set; }

    bool isBusAlertActive;
    public bool IsBusAlertActive
    {
        get => isBusAlertActive;
        set { isBusAlertActive = value; OnPropertyChanged(nameof(IsBusAlertActive)); }
    }

    bool isBusAlertVisible;
    public bool IsBusAlertVisible
    {
        get => isBusAlertVisible;
        set { isBusAlertVisible = value; OnPropertyChanged(nameof(IsBusAlertVisible)); }
    }

    string alertTime;
    public string AlertTime
    {
        get => alertTime;
        set { alertTime = value; OnPropertyChanged(nameof(AlertTime)); }
    }

    string stopNumber;
    public string StopNumber
    {
        get => stopNumber;
        set { stopNumber = value; OnPropertyChanged(nameof(StopNumber)); }
    }

    string stopInfo;
    public string StopInfo
    {
        get => stopInfo;
        set { stopInfo = value; OnPropertyChanged(nameof(StopInfo)); }
    }

    bool stopFound;
    public bool StopFound
    {
        get => stopFound;
        set { stopFound = value; OnPropertyChanged(nameof(StopFound)); }
    }

    public Command FavouriteCommand { get; set; }
    public Command CancelAlertCommand { get; set; }
    public Command ConfirmAlertCommand { get; set; }

    public BusArrivalsViewModel()
    {
        StopFound = false;
        StopNumber = "50043";
        StopInfo = "PRODUCTION STATION BAY 1";
        StopFound = true;

        ArrivalTimes =
        [
            new Schedule() { RouteNo = "002", Destination = "MACDONALD - 16 AVE", ExpectedCountdown = 0 },
            new Schedule() { RouteNo = "005", Destination = "ROBSON", ExpectedCountdown = 3 },
            new Schedule() { RouteNo = "002", Destination = "MACDONALD - 16 AVE", ExpectedCountdown = 5 },
            new Schedule() { RouteNo = "005", Destination = "ROBSON", ExpectedCountdown = 7 },
            new Schedule() { RouteNo = "002", Destination = "MACDONALD - 16 AVE", ExpectedCountdown = 10 },
        ];
    }

    public BusArrivalsViewModel(Stop _stop)
    {
        ArrivalTimes = new ObservableCollection<Schedule>();

        isBusAlertActive = false;
        IsBusAlertVisible = false;
        AlertTime = "5";
        StopFound = false;
        StopNumber = _stop.StopNo;
        StopInfo = _stop.Name;
        Stop = _stop;

        //StopFound = App.StopMan.IsStop(StopNumber);

        //FavouriteCommand = new Command(FavouriteCommandExecute);
        //CancelAlertCommand = new Command(CancelAlertCommandExecute);
        //ConfirmAlertCommand = new Command(ConfirmAlertCommandExecute);

        GetData();
    }

    public async Task GetData()
    {
        if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        {
            var bufferList = await RestClient.Instance.GetBusArrivalsTimes(StopNumber);
            if (bufferList.Count > 0)
            {
                ArrivalTimes.Clear();
                foreach (var item in bufferList)
                    ArrivalTimes.Add(item);
            }
        }
    }

    //void FavouriteCommandExecute()
    //{
    //    Stop stop = new Stop()
    //    {
    //        Name = Stop.Name,
    //        Routes = Stop.Routes,
    //        StopNo = Stop.StopNo,
    //        Tag = Stop.Tag
    //    };

    //    if (StopFound)
    //    {
    //        App.StopMan.DeleteStop(stop);
    //        StopFound = false;
    //        MessagingCenter.Send(this, "REMOVE_FAVOURITE_STOP", stop);
    //    }
    //    else
    //    {
    //        App.StopMan.AddStop(stop);
    //        StopFound = true;
    //        MessagingCenter.Send(this, "ADD_FAVOURITE_STOP", stop);
    //    }
    //}
}