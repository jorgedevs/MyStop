using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using MyStop.Models;
using SQLite;

namespace MyStop
{
    public class StopManager
    {
        private readonly SQLiteConnection conn;

        public StopManager()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            conn.CreateTable<Stop>();
        }

        public List<Stop> GetStops() 
        {
            return (from t in conn.Table<Stop>() select t).ToList();
        }

        public bool IsStop(string stopNo)
        {
            return (GetStops().Where(s => s.StopNo == stopNo).ToList().Count > 0);
        }

        public int AddStop(Stop stop)
        {
            return conn.Insert(stop);
        }

        public int DeleteStop(Stop stop)
        {
            return conn.Delete(stop);
        }

        public int UpdateStops(List<Stop> stops)
        {
            foreach (var stop in stops)
                conn.Update(stop);
            
            return 0;
        }
    }
}