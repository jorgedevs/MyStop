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

    bool isFavouriteBusStop;
    public bool IsFavouriteBusStop
    {
        get => isFavouriteBusStop;
        set { isFavouriteBusStop = value; OnPropertyChanged(nameof(IsFavouriteBusStop)); }
    }

    public Command FavouriteCommand { get; set; }
    public Command CancelAlertCommand { get; set; }
    public Command ConfirmAlertCommand { get; set; }

    public BusArrivalsViewModel()
    {
        IsFavouriteBusStop = false;
        StopNumber = "50043";
        StopInfo = "PRODUCTION STATION BAY 1";
        IsFavouriteBusStop = true;

        ArrivalTimes =
        [
            new Schedule() { RouteNo = "002", Destination = "MACDONALD - 16 AVE", ExpectedCountdown = 0 },
            new Schedule() { RouteNo = "005", Destination = "ROBSON", ExpectedCountdown = 3 },
            new Schedule() { RouteNo = "002", Destination = "MACDONALD - 16 AVE", ExpectedCountdown = 5 },
            new Schedule() { RouteNo = "005", Destination = "ROBSON", ExpectedCountdown = 7 },
            new Schedule() { RouteNo = "002", Destination = "MACDONALD - 16 AVE", ExpectedCountdown = 10 },
        ];
    }

    public BusArrivalsViewModel(Stop stop)
    {
        ArrivalTimes = new ObservableCollection<Schedule>();

        isBusAlertActive = false;
        IsBusAlertVisible = false;
        AlertTime = "5";
        IsFavouriteBusStop = false;
        StopNumber = stop.StopNo;
        StopInfo = stop.Name;
        Stop = stop;

        //StopFound = App.StopMan.IsStop(StopNumber);

        FavouriteCommand = new Command(FavouriteCommandExecute);
        //CancelAlertCommand = new Command(CancelAlertCommandExecute);
        //ConfirmAlertCommand = new Command(ConfirmAlertCommandExecute);

        _ = GetBusArrivalsTimes();
    }

    public async Task GetBusArrivalsTimes()
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

    void FavouriteCommandExecute()
    {
        Stop stop = new Stop()
        {
            Name = Stop.Name,
            Routes = Stop.Routes,
            StopNo = Stop.StopNo,
            Tag = Stop.Tag
        };

        if (IsFavouriteBusStop)
        {
            App.StopManager.DeleteStop(stop);
            IsFavouriteBusStop = false;
            MessagingCenter.Send(this, "REMOVE_FAVOURITE_STOP", stop);
        }
        else
        {
            App.StopManager.AddStop(stop);
            IsFavouriteBusStop = true;
            MessagingCenter.Send(this, "ADD_FAVOURITE_STOP", stop);
        }
    }
}