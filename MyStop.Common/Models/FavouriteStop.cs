namespace MyStop.Models
{
    public class FavouriteStop : Stop
    {
        bool editMode;
        public bool EditMode
        {
            get { return editMode; }
            set { editMode = value; NotifyPropertyChanged("EditMode"); }
        }

        bool hasTag;
        public bool HasTag
        {
            get { return hasTag; }
            set { hasTag = value; NotifyPropertyChanged("HasTag"); }
        }
    }
}
