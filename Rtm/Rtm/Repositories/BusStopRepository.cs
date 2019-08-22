using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rtm.DependencyInterfaces;
using Rtm.Models;
using SQLite;
using Rtm.Database;

namespace Rtm.Repositories
{
    public class BusStopRepository : IBusStopRepository
    {
        private readonly SQLiteConnection _connection;

        public event EventHandler BusStopsDeletedEvent;

        public BusStopRepository(ILocalDatabase localDatabase)
        {
            _connection = localDatabase.GetConnection();
        }

        ~BusStopRepository()
        {
            _connection.Close();
        }

        public void Add(BusStop busStop)
        {
            busStop.Departures = null;
            _connection.Insert(busStop);
        }

        public void AddRange(IEnumerable<BusStop> busStops)
        {
            _connection.InsertAll(busStops);
        }

        public BusStop Get(int id)
            => _connection.Get<BusStop>(id);

        public List<BusStop> GetAll()
            => _connection.Table<BusStop>().ToList();

        public List<BusStop> GetAllFavorites()
            => _connection.Table<BusStop>().Where(b => b.IsFavorite).ToList();

        public void Delete(int id)
            => _connection.Table<BusStop>().Delete(b => b.Id == id);

        public void DeleteAll()
        {
            _connection.DeleteAll<BusStop>();
            BusStopsDeletedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void AddToFavorites(BusStop busStop)
        {
            busStop.IsFavorite = true;
            _connection.Update(busStop);
        }

        public void RemoveFromFavorites(BusStop busStop)
        {
            busStop.IsFavorite = false;
            _connection.Update(busStop);
        }

        public void Rename(BusStop busStop, string newName)
        {
            busStop.CustomName = newName;
            _connection.Update(busStop);
        }
    }
}
