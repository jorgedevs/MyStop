using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class BusArrivalsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IGtfsService _gtfsService;
    private readonly ISQLiteService _sqliteService;

    public StopModel Stop { get; set; }

    public ScheduleModel Schedule { get; set; }

    public ObservableCollection<ScheduleModel> ArrivalTimes { get; set; }

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

        ArrivalTimes = new ObservableCollection<ScheduleModel>();

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
        //IsFavouriteBusStop = await App.StopManager.IsStop(StopNumber);
        IsFavouriteBusStop = false;

        if (IsFavouriteBusStop)
            FavoriteIcon = "icon_favourites_remove.png";
        else
            FavoriteIcon = "icon_favourites_add.png";

        await GetBusArrivalsTimes();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("BusStopNumber"))
        {
            string? stopCode = query["BusStopNumber"] as string;

            var stop = new StopModel()
            {
                StopNo = "50023",
                Name = "WB DAVIE ST NS HAMILTON ST"
            };

            //var stop = _sqliteService.GetStopInfo(stopCode!);
            Stop = stop;
            StopNumber = Stop.StopNo!;
            StopInfo = Stop.Name!;
        }
    }

    private async Task GetBusArrivalsTimes()
    {
        //if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
        //{
        //    var bufferList = await RestClient.Instance.GetBusArrivalsTimes(StopNumber);
        //    if (bufferList.Count > 0)
        //    {
        //        ArrivalTimes.Clear();
        //        foreach (var item in bufferList)
        //            ArrivalTimes.Add(item);
        //    }
        //}

        ArrivalTimes.Clear();
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 3 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 10 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 15 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 20 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 24 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 28 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 33 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 40 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 48 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 60 });
        ArrivalTimes.Add(new ScheduleModel() { RouteNo = "006", Destination = "Davie", ExpectedCountdown = 65 });
    }

    private async void ToggleSaveBusStop()
    {
        var stop = new SavedStopModel()
        {
            Name = Stop.Name,
            Routes = Stop.Routes,
            StopNo = Stop.StopNo,
            Tag = Stop.Tag
        };

        if (IsFavouriteBusStop)
        {
            await _sqliteService.RemoveStop(stop);
            IsFavouriteBusStop = false;
            FavoriteIcon = "icon_favourites_add.png";
        }
        else
        {
            await _sqliteService.SaveStop(stop);
            IsFavouriteBusStop = true;
            FavoriteIcon = "icon_favourites_remove.png";
        }
    }
}