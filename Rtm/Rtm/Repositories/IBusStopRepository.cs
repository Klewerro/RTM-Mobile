using Rtm.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rtm.Repositories
{
    public interface IBusStopRepository
    {
        List<BusStop> GetAll();
        List<BusStop> GetAllFavorites();
        void Add(BusStop busStop);
        void AddRange(IEnumerable<BusStop> busStops);
        void Remove(int id);
        bool IsInFavorites(int id);
    }
}
