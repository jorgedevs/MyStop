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

    string favoriteIcon;
    public string FavoriteIcon
    {
        get => favoriteIcon;
        set { favoriteIcon = value; OnPropertyChanged(nameof(FavoriteIcon)); }
    }

    public Command FavouriteCommand { get; set; }
    public Command CancelAlertCommand { get; set; }
    public Command ConfirmAlertCommand { get; set; }

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

        FavouriteCommand = new Command(FavouriteCommandExecute);
        //CancelAlertCommand = new Command(CancelAlertCommandExecute);
        //ConfirmAlertCommand = new Command(ConfirmAlertCommandExecute);

        _ = Initialize();
    }

    public async Task Initialize()
    {
        IsFavouriteBusStop = await App.StopManager.IsStop(StopNumber);

        if (IsFavouriteBusStop)
            FavoriteIcon = "icon_favourites_remove.png";
        else
            FavoriteIcon = "icon_favourites_add.png";

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

    async void FavouriteCommandExecute()
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
            await App.StopManager.DeleteStop(stop);
            IsFavouriteBusStop = false;
            MessagingCenter.Send(this, "REMOVE_FAVOURITE_STOP", stop);
            FavoriteIcon = "icon_favourites_add.png";
        }
        else
        {
            await App.StopManager.AddStop(stop);
            IsFavouriteBusStop = true;
            MessagingCenter.Send(this, "ADD_FAVOURITE_STOP", stop);
            FavoriteIcon = "icon_favourites_remove.png";
        }
    }
}