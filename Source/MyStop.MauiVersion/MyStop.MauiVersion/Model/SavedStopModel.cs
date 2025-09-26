namespace MyStop.MauiVersion.Model;

public class SavedStopModel : StopModel
{
    bool editMode;
    public bool EditMode
    {
        get => editMode;
        set
        {
            editMode = value;
            NotifyPropertyChanged(nameof(EditMode));
        }
    }

    bool hasTag;
    public bool HasTag
    {
        get => hasTag;
        set
        {
            hasTag = value;
            NotifyPropertyChanged(nameof(HasTag));
        }
    }
}