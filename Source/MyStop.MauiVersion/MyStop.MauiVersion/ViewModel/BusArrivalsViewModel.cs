using MyStop.MauiVersion.CSVs;
using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services;
using MyStop.MauiVersion.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class BusArrivalsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IGtfsService _gtfsService;
    private readonly ISQLiteService _sqliteService;

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

    public ICommand FavouriteCommand { get; set; }

    public ICommand RefreshCommand { get; set; }

    public BusArrivalsViewModel(
        IGtfsService gtfsService,
        ISQLiteService sqliteService)
    {
        _gtfsService = gtfsService;
        _sqliteService = sqliteService;

        ArrivalTimes = new ObservableCollection<Schedule>();

        IsFavouriteBusStop = false;

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
        IsFavouriteBusStop = false; //await App.StopManager.IsStop(StopNumber);

        if (IsFavouriteBusStop)
            FavoriteIcon = "icon_favourites_remove.png";
        else
            FavoriteIcon = "icon_favourites_add.png";

        //await GetBusArrivalsTimes();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("BusStopNumber"))
        {
            string? stopCode = query["BusStopNumber"] as string;

            var stop = _sqliteService.GetStopInfo(stopCode!);
            Stop = stop;
            StopNumber = Stop.stop_code!;
            StopInfo = Stop.stop_name!;
        }
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
        //var stop = new Stop()
        //{
        //    Name = Stop.Name,
        //    Routes = Stop.Routes,
        //    StopNo = Stop.StopNo,
        //    Tag = Stop.Tag
        //};

        if (IsFavouriteBusStop)
        {
            //await App.StopManager.DeleteStop(stop);
            IsFavouriteBusStop = false;
            FavoriteIcon = "icon_favourites_add.png";
        }
        else
        {
            //await App.StopManager.AddStop(stop);
            IsFavouriteBusStop = true;
            FavoriteIcon = "icon_favourites_remove.png";
        }
    }
}