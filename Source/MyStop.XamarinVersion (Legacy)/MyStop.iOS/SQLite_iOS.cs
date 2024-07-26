using System;
using MyStop.iOS;
using System.IO;
using SQLite;

[assembly: Xamarin.Forms.Dependency(typeof(SQLite_iOS))]
namespace MyStop.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            var fileName = "MyStopDatabase.db3";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, fileName);
            var connection = new SQLiteConnection(path);

            return connection;
        }
    }
}