using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rztm.Repositories
{
    public interface IBusStopRepository
    {
        event EventHandler BusStopsDeletedEvent;

        Task<BusStop> GetAsync(int id);
        Task<BusStop> GetAsync(string name);
        Task<List<BusStop>> GetAllAsync();
        Task<List<BusStop>> GetAllFavoritesAsync();
        Task AddAsync(BusStop busStop);
        Task AddRangeAsync(IEnumerable<BusStop> busStops);
        Task DeleteAsync(int id);
        Task DeleteAllAsync();
        Task AddToFavoritesAsync(BusStop busStop);
        Task RemoveFromFavoritesAsync(BusStop busStop);
        Task RenameAsync(BusStop busStop, string newName);
    }
}
