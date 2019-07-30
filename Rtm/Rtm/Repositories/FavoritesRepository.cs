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
    public class FavoritesRepository : IFavoritesRepository
    {
        private readonly SQLiteConnection _connection;

        public FavoritesRepository()
        {
            ILocalDatabase localDatabase = new LocalDatabase();
            _connection = localDatabase.GetConnection();
        }

        ~FavoritesRepository()
        {
            _connection.Close();
        }

        public void Add(BusStop busStop)
        {
            busStop.Departures = null;
            _connection.Insert(busStop);
        }

        public List<BusStop> GetAll()
            => _connection.Table<BusStop>().ToList();

        public void Remove(int id)
            => _connection.Table<BusStop>().Delete(b => b.Id == id);

        public bool IsInFavorites(int id)
            => _connection.Table<BusStop>().Any(b => b.Id == id);
    }
}
