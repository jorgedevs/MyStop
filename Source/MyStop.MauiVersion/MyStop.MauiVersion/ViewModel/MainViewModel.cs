using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Utils;
using MyStop.MauiVersion.View;
using System.Diagnostics;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class MainViewModel : BaseViewModel
{
    public const string INVALID_CODE_ENTERED = "INVALID_CODE_ENTERED";
    public const string STOP_NOT_FOUND = "STOP_NOT_FOUND";
    public const string STOP_FOUND = "STOP_FOUND";
    Stop stop;

    bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set { isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
    }

    string busStopNumber;
    public string BusStopNumber
    {
        get => busStopNumber;
        set { busStopNumber = value; OnPropertyChanged(nameof(BusStopNumber)); }
    }

    public ICommand GetStopInfoCommand { get; set; }

    public ICommand GoToFavoriteStops { get; set; }

    public MainViewModel()
    {
        BusStopNumber = "";

        GetStopInfoCommand = new Command(
           async () => await GetStopInfoCommandExecute(),
           () => !IsBusy);

        GoToFavoriteStops = new Command(
            async () => await Shell.Current.GoToAsync(nameof(FavouriteStopsPage)),
            () => !IsBusy);
    }

    private async Task GetStopInfoCommandExecute()
    {
        if (IsBusy)
            return;
        IsBusy = true;

        try
        {
            if (BusStopNumber.Length == 0 || BusStopNumber.Contains("."))
                MessagingCenter.Send(this, INVALID_CODE_ENTERED);
            else
            {
                stop = await RestClient.Instance.GetBusStopInfo(BusStopNumber);
                if (stop.Name != null)
                    MessagingCenter.Send(this, STOP_FOUND, stop);
                else
                    MessagingCenter.Send(this, STOP_NOT_FOUND);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error: " + ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}