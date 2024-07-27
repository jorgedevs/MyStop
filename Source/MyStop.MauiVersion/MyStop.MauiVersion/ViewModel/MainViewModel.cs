using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Utils;
using System.Diagnostics;

namespace MyStop.MauiVersion.ViewModel;

public class MainViewModel : BaseViewModel
{
    public const string INVALID_CODE_ENTERED = "INVALID_CODE_ENTERED";
    public const string STOP_NOT_FOUND = "STOP_NOT_FOUND";
    public const string STOP_FOUND = "STOP_FOUND";
    Stop _stop;

    bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); GetStopInfoCommand.ChangeCanExecute(); }
    }

    string _busStopNumber;
    public string BusStopNumber
    {
        get => _busStopNumber;
        set { _busStopNumber = value; OnPropertyChanged(nameof(BusStopNumber)); }
    }

    public Command GetStopInfoCommand { get; set; }

    public MainViewModel()
    {
        BusStopNumber = "";
        GetStopInfoCommand = new Command(
           async () => await GetStopInfoCommandExecute(),
           () => !IsBusy);
    }

    async Task GetStopInfoCommandExecute()
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
                _stop = await RestClient.Instance.GetBusStopInfo(BusStopNumber);
                if (_stop.Name != null)
                    MessagingCenter.Send(this, STOP_FOUND, _stop);
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