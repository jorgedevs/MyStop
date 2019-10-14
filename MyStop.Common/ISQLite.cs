using SQLite;

namespace MyStop
{
    public interface ISQLite
    {
         SQLiteConnection GetConnection();
    }
}
