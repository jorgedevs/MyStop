using MyStop.Forms.Droid;
using Xamarin.Forms;
using System.IO;
using SQLite;

[assembly: Dependency(typeof(SQLite_Android))]
namespace MyStop.Forms.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var fileName = "MyStopDatabase.db3";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, fileName);
            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}
