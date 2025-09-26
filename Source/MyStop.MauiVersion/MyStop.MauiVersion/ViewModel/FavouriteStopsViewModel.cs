using MyStop.MauiVersion.CSVs;
using MyStop.MauiVersion.Model;
using MyStop.MauiVersion.Services;
using MyStop.MauiVersion.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class FavouriteStopsViewModel : BaseViewModel
{
    private readonly IGtfsService _gtfsService;
    private readonly ISQLiteService _sqliteService;

    public ObservableCollection<SavedStop> ItemList { get; set; }

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

        ItemList =
        [
            new SavedStop()
            {
                stop_code = "50024",
                stop_name = "WB DAVIE ST FS RICHARDS ST",
                stop_desc = "006"
            },
            new SavedStop()
            {
                stop_code = "50025",
                stop_name = "WB DAVIE ST FS GRANVILLE ST",
                stop_desc = "006"
            },
            new SavedStop()
            {
                stop_code = "20026",
                stop_name = "WB DAVIE ST FS HOWE ST",
                stop_desc = "006"
            },
            new SavedStop()
            {
                stop_code = "20027",
                stop_name = "WB DAVIE ST FS HELMCKEN ST",
                stop_desc = "006"
            },
            new SavedStop()
            {
                stop_code = "20028",
                stop_name = "WB DAVIE ST FS NELSON ST",
                stop_desc = "006"
            },
        ];

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

    public void LoadStops()
    {
        var stops = _sqliteService.GetSavedStops();

        if (stops == null)
            return;

        ItemList.Clear();
        foreach (var stop in stops)
        {
            ItemList.Add(new SavedStop()
            {
                //Tag = stop.Tag,
                stop_name = stop.stop_name,
                stop_code = stop.stop_code,
                //Routes = stop.Routes,
                //HasTag = !string.IsNullOrEmpty(stop.Tag),
                //EditMode = false,
            });
        }
    }
}