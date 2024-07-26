using MyStop.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;

namespace MyStop.ViewModels
{
    public class FavouritesStopsViewModel : BaseViewModel
    {
        public ObservableCollection<FavouriteStop> ItemList { get; set; }

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

        public Command ToggleEditCommand { get; set; }
        public Command CancelEditCommand { get; set; }
        public Command SaveChangesCommand { get; set; }

        public FavouritesStopsViewModel()
        {
            ItemList = new ObservableCollection<FavouriteStop>();

            EditIcon = "icon_edit";

            ToggleEditCommand = new Command(ToggleEditCommandExecute);
            CancelEditCommand = new Command(CancelEditCommandExecute);
            SaveChangesCommand = new Command(SaveChangesCommandExecute);

            MessagingCenter.Subscribe<BusArrivalsViewModel, Stop>(this, "ADD_FAVOURITE_STOP", (sender, arg) =>
            {
                ItemList.Add(new FavouriteStop()
                {
                    Tag = arg.Tag,
                    Name = arg.Name,
                    StopNo = arg.StopNo,
                    Routes = arg.Routes,
                    HasTag = !string.IsNullOrEmpty(arg.Tag),
                    EditMode = false,
                });
            });
            MessagingCenter.Subscribe<BusArrivalsViewModel, Stop>(this, "REMOVE_FAVOURITE_STOP", (sender, arg) =>
            {
                foreach(var item in ItemList)
                {
                    if (item.StopNo == arg.StopNo)
                    {
                        ItemList.Remove(item);
                        break;
                    }
                }
            });

            if (App.DesignTime)
            { 
                ItemList.Add(new FavouriteStop() { Tag = "", Name = "NB BURRARD ST NS DAVIE ST", StopNo = "50075", Routes = "002, 032, 044, N22", HasTag = !string.IsNullOrEmpty(""), EditMode = false });
                ItemList.Add(new FavouriteStop() { Tag = "", Name = "BURRARD STN BAY 1",         StopNo = "50043", Routes = "002, 032, 044, N22", HasTag = !string.IsNullOrEmpty(""), EditMode = false });
            }
            else
            {
                LoadStops();
            }
        }

        void LoadStops()
        {
            var stops = App.StopMan.GetStops();

            if (stops == null)
                return;

            foreach (var stop in stops) 
            { 
                ItemList.Add(new FavouriteStop()
                {
                    Tag = stop.Tag,
                    Name = stop.Name,
                    StopNo = stop.StopNo,
                    Routes = stop.Routes,
                    HasTag = !string.IsNullOrEmpty(stop.Tag),
                    EditMode = false,
                });
            }
        }

        public void ToggleEditCommandExecute()
        {
            EnableEdit = !EnableEdit;
            EditIcon = EnableEdit ? "icon_save" : "icon_edit";

            if(!EnableEdit)
            {
                List<Stop> stops = new List<Stop>();

                foreach(var item in ItemList)
                {
                    stops.Add(new Stop()
                    {
                        Tag = item.Tag,
                        Name = item.Name,
                        Routes = item.Routes,
                        StopNo = item.StopNo
                    });
                }
                App.StopMan.UpdateStops(stops);
            }

            foreach (var stop in ItemList)
            {
                stop.EditMode = EnableEdit;
            }
        }

        public void CancelEditCommandExecute()
        {
            IsEditVisible = false;
        }

        public void SaveChangesCommandExecute()
        {
            foreach(var stop in ItemList)
            {
                if(stop.StopNo == StopNumber)
                {
                    stop.Tag = string.IsNullOrEmpty(TagName)? string.Empty : TagName.ToUpper();
                    stop.HasTag = !string.IsNullOrEmpty(TagName);
                    break;
                }
            }

            TagName = string.Empty;
            IsEditVisible = false;
        }

        public void Dispose()
        {
            MessagingCenter.Unsubscribe<BusArrivalsViewModel, Stop>(this, "ADD_FAVOURITE_STOP");
            MessagingCenter.Unsubscribe<BusArrivalsViewModel, Stop>(this, "REMOVE_FAVOURITE_STOP");
        }
    }
}