using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Utils;
using System.Collections.ObjectModel;

namespace MyStop.MauiVersion.ViewModel;

public class BusArrivalsViewModel : BaseViewModel
{
    public Stop Stop { get; set; }

    public Schedule Schedule { get; set; }

    public ObservableCollection<Schedule> ArrivalTimes { get; set; }

    bool isFavouriteBusStop;
    public bool IsFavouriteBusStop
    {
        get => isFavouriteBusStop;
        set { isFavouriteBusStop = value; OnPropertyChanged(nameof(IsFavouriteBusStop)); }
    }

    bool isUpdatingArrivalTimes;
    public bool IsUpdatingArrivalTimes
    {
        get => isUpdatingArrivalTimes;
        set { isUpdatingArrivalTimes = value; OnPropertyChanged(nameof(IsUpdatingArrivalTimes)); }
    }

    string stopInfo;
    public string StopInfo
    {
        get => stopInfo;
        set { stopInfo = value; OnPropertyChanged(nameof(StopInfo)); }
    }

    string stopNumber;
    public string StopNumber
    {
        get => stopNumber;
        set { stopNumber = value; OnPropertyChanged(nameof(StopNumber)); }
    }

    string favoriteIcon;
    public string FavoriteIcon
    {
        get => favoriteIcon;
        set { favoriteIcon = value; OnPropertyChanged(nameof(FavoriteIcon)); }
    }

    public Command FavouriteCommand { get; set; }

    public Command RefreshCommand { get; set; }

    public BusArrivalsViewModel(Stop stop)
    {
        ArrivalTimes = new ObservableCollection<Schedule>();

        IsFavouriteBusStop = false;
        StopNumber = stop.StopNo!;
        StopInfo = stop.Name!;
        Stop = stop;

        FavouriteCommand = new Command(ToggleSaveBusStop);
        RefreshCommand = new Command(async () => 
        {
            await GetBusArrivalsTimes();
            IsUpdatingArrivalTimes = false;
        });

        _ = Initialize();
    }

    private async Task Initialize()
    {
        IsFavouriteBusStop = await App.StopManager.IsStop(StopNumber);

        if (IsFavouriteBusStop)
            FavoriteIcon = "icon_favourites_remove.png";
        else
            FavoriteIcon = "icon_favourites_add.png";

        await GetBusArrivalsTimes();
    }

    private async Task GetBusArrivalsTimes()
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

    private async void ToggleSaveBusStop()
    {
        var stop = new Stop()
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
            FavoriteIcon = "icon_favourites_add.png";
        }
        else
        {
            await App.StopManager.AddStop(stop);
            IsFavouriteBusStop = true;
            FavoriteIcon = "icon_favourites_remove.png";
        }
    }
}