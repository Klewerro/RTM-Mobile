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

        public BusStopRepository()
        {
            ILocalDatabase localDatabase = new LocalDatabase();
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

        public void AddToFavorites(int busStopId)
        {
            var busStop = _connection.Get<BusStop>(busStopId);
            busStop.IsFavorite = true;
            _connection.Update(busStop);
        }

        public void AddRange(IEnumerable<BusStop> busStops)
        {
            //_connection.Table<BusStop>().Delete();
            _connection.InsertAll(busStops);
        }

        public List<BusStop> GetAll()
            => _connection.Table<BusStop>().ToList();

        public List<BusStop> GetAllFavorites()
            => _connection.Table<BusStop>().Where(b => b.IsFavorite).ToList();

        public void Remove(int id)
            => _connection.Table<BusStop>().Delete(b => b.Id == id);

        public bool IsInFavorites(int id)
            => _connection.Table<BusStop>().Any(b => b.Id == id);
    }
}
