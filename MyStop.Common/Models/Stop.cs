using SQLite;

namespace MyStop.Models
{
    [Table("Stop")]
    public class Stop : BaseModel
    {	
        string stopNo;
		[PrimaryKey]
		public string StopNo
        {
            get { return stopNo; }
            set { stopNo = value; NotifyPropertyChanged("StopNo"); }
        }

        string name;
		[Unique]
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }
		
        string routes;
        public string Routes
        {
            get { return routes; }
            set { routes = value; NotifyPropertyChanged("Routes"); }
        }

        string tag;
        public string Tag
        {
            get { return tag; }
            set { tag = value; NotifyPropertyChanged("Tag"); }
        }
    }
}
