using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rztm.DependencyInterfaces;
using Rztm.Models;
using SQLite;
using Rztm.Database;
using System.Threading.Tasks;

namespace Rztm.Repositories
{
    public class BusStopRepository : IBusStopRepository
    {
        private readonly SQLiteAsyncConnection _connection;

        public event EventHandler BusStopsDeletedEvent;

        public BusStopRepository(ILocalDatabase localDatabase)
        {
            _connection = localDatabase.GetConnection();
        }

        ~BusStopRepository()
        {
            _connection.CloseAsync();
        }

        public Task AddAsync(BusStop busStop)
        {
            busStop.Departures = null;
            return _connection.InsertAsync(busStop);
        }

        public Task AddRangeAsync(IEnumerable<BusStop> busStops)
            => _connection.InsertAllAsync(busStops);

        public Task<BusStop> GetAsync(int id)
            => _connection.GetAsync<BusStop>(id);

        public Task<BusStop> GetAsync(string name)
            => _connection.GetAsync<BusStop>(b => b.Name.Equals(name));

        public Task<List<BusStop>> GetAllAsync()
            => _connection.Table<BusStop>()
                .OrderBy(b => b.Name)
                .ToListAsync();

        public Task<List<BusStop>> GetAllFavoritesAsync()
            => _connection.Table<BusStop>()
                .Where(b => b.IsFavorite)
                .OrderBy(b => b.Name)
                .ToListAsync();

        public Task DeleteAsync(int id)
            => _connection.Table<BusStop>().DeleteAsync(b => b.Id == id);

        public async Task DeleteAllAsync()
        {
            await _connection.DeleteAllAsync<BusStop>();
            BusStopsDeletedEvent?.Invoke(this, EventArgs.Empty);
        }

        public Task AddToFavoritesAsync(BusStop busStop)
        {
            busStop.IsFavorite = true;
            return _connection.UpdateAsync(busStop);
        }

        public Task RemoveFromFavoritesAsync(BusStop busStop)
        {
            busStop.IsFavorite = false;
            return _connection.UpdateAsync(busStop);
        }

        public Task RenameAsync(BusStop busStop, string newName)
        {
            busStop.CustomName = newName;
            return _connection.UpdateAsync(busStop);
        }
    }
}
