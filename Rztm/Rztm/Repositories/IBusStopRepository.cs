using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.Repositories
{
    public interface IBusStopRepository
    {
        event EventHandler BusStopsDeletedEvent;

        BusStop Get(int id);
        BusStop Get(string name);
        List<BusStop> GetAll();
        List<BusStop> GetAllFavorites();
        void Add(BusStop busStop);
        void AddRange(IEnumerable<BusStop> busStops);
        void Delete(int id);
        void DeleteAll();
        void AddToFavorites(BusStop busStop);
        void RemoveFromFavorites(BusStop busStop);
        void Rename(BusStop busStop, string newName);
    }
}
