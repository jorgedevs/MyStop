using SQLite;

namespace MyStop.MauiVersion.Model;

[Table("Stop")]
public class StopModel : BaseModel
{
    string? stopNo;
    [PrimaryKey]
    public string? StopNo
    {
        get => stopNo;
        set
        {
            stopNo = value;
            NotifyPropertyChanged(nameof(StopNo));
        }
    }

    string? name;
    [Unique]
    public string? Name
    {
        get => name;
        set
        {
            name = value;
            NotifyPropertyChanged(nameof(Name));
        }
    }

    string? routes;
    public string? Routes
    {
        get => routes;
        set
        {
            routes = value;
            NotifyPropertyChanged(nameof(Routes));
        }
    }

    string? tag;
    public string? Tag
    {
        get => tag;
        set
        {
            tag = value;
            NotifyPropertyChanged(nameof(Tag));
        }
    }
}