using MyStop.MauiVersion.CSVs;
using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services.Interfaces;
using MyStop.MauiVersion.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class FavouriteStopsViewModel : BaseViewModel
{
    private readonly IGtfsService _gtfsService;
    private readonly ISQLiteService _sqliteService;

    public ObservableCollection<SavedStopModel> ItemList { get; set; }

    bool isEditVisible;
    public bool IsEditVisible
    {
        get => isEditVisible;
        set { isEditVisible = value; OnPropertyChanged(nameof(IsEditVisible)); }
    }

    bool enableEdit;
    public bool EnableEdit
    {
        get => enableEdit;
        set { enableEdit = value; OnPropertyChanged(nameof(EnableEdit)); }
    }

    bool isEmpty;
    public bool IsEmpty
    {
        get => isEmpty;
        set { isEmpty = value; OnPropertyChanged(nameof(IsEmpty)); }
    }

    private bool _isLoadingStops;

    string tagName;
    public string TagName
    {
        get => tagName;
        set { tagName = value; OnPropertyChanged(nameof(TagName)); }
    }

    string stopNumber;
    public string StopNumber
    {
        get => stopNumber;
        set { stopNumber = value; OnPropertyChanged(nameof(StopNumber)); }
    }

    ImageSource editIcon;
    public ImageSource EditIcon
    {
        get => editIcon;
        set { editIcon = value; OnPropertyChanged(nameof(EditIcon)); }
    }

    public ICommand ToggleEditCommand { get; set; }

    public ICommand CancelEditCommand { get; set; }

    public ICommand SaveChangesCommand { get; set; }

    public ICommand GoToAboutPage { get; set; }

    public FavouriteStopsViewModel(
        IGtfsService gtfsService,
        ISQLiteService sqliteService)
    {
        _gtfsService = gtfsService;
        _sqliteService = sqliteService;

        ItemList = new ObservableCollection<SavedStopModel>();



        EditIcon = "icon_edit";

        ToggleEditCommand = new Command(ToggleEditCommandExecute);

        CancelEditCommand = new Command(CancelEditCommandExecute);

        SaveChangesCommand = new Command(SaveChangesCommandExecute);

        GoToAboutPage = new Command(async () => await Shell.Current.GoToAsync(nameof(AboutPage)));
    }

    private void ToggleEditCommandExecute()
    {
        EnableEdit = !EnableEdit;
        EditIcon = EnableEdit
            ? "icon_save"
            : "icon_edit";

        if (!EnableEdit)
        {
            List<Stop> stops = new List<Stop>();

            foreach (var item in ItemList)
            {
                //stops.Add(new Stop()
                //{
                //    Tag = item.Tag,
                //    Name = item.Name,
                //    Routes = item.Routes,
                //    StopNo = item.StopNo
                //});
            }
            //App.StopManager.UpdateStops(stops);
        }

        foreach (var stop in ItemList)
        {
            //stop.EditMode = EnableEdit;
        }
    }

    private void CancelEditCommandExecute()
    {
        IsEditVisible = false;
    }

    private void SaveChangesCommandExecute()
    {
        foreach (var stop in ItemList)
        {
            //if (stop.StopNo == StopNumber)
            //{
            //    stop.Tag = string.IsNullOrEmpty(TagName) ? string.Empty : TagName.ToUpper();
            //    stop.HasTag = !string.IsNullOrEmpty(TagName);
            //    break;
            //}
        }

        TagName = string.Empty;
        IsEditVisible = false;
    }

    public async void LoadStops()
    {
        if (_isLoadingStops)
            return;

        _isLoadingStops = true;

        try
        {
            var stops = _sqliteService.GetSavedStops();

            if (stops == null)
            {
                IsEmpty = true;
                return;
            }

            ItemList.Clear();
            foreach (var stop in stops)
            {
                var routeNumbers = await _sqliteService.GetRouteNumbersForStopAsync(stop.StopNo ?? "");
                var routesText = routeNumbers.Count > 0 ? string.Join(", ", routeNumbers) : null;

                ItemList.Add(new SavedStopModel()
                {
                    //Tag = stop.Tag,
                    Name = stop.Name,
                    StopNo = stop.StopNo,
                    Routes = routesText,
                    //HasTag = !string.IsNullOrEmpty(stop.Tag),
                    //EditMode = false,
                });
            }

            IsEmpty = ItemList.Count == 0;
        }
        finally
        {
            _isLoadingStops = false;
        }
    }
}