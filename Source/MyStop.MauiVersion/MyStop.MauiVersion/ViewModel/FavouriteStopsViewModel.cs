using MyStop.MauiVersion.CSVs;
using MyStop.MauiVersion.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MyStop.MauiVersion.ViewModel;

public class FavouriteStopsViewModel : BaseViewModel
{
    public ObservableCollection<Stop> ItemList { get; set; }

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

    public FavouriteStopsViewModel()
    {
        ItemList = new ObservableCollection<Stop>();

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

    public async Task LoadStops()
    {
        //var stops = await App.StopManager.GetStops();

        //if (stops == null)
        //    return;

        //ItemList.Clear();
        //foreach (var stop in stops)
        //{
        //    ItemList.Add(new FavouriteStop()
        //    {
        //        Tag = stop.Tag,
        //        Name = stop.Name,
        //        StopNo = stop.StopNo,
        //        Routes = stop.Routes,
        //        HasTag = !string.IsNullOrEmpty(stop.Tag),
        //        EditMode = false,
        //    });
        //}
    }
}