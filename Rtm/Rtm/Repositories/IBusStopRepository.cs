﻿using Rtm.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rtm.Repositories
{
    public interface IBusStopRepository
    {
        event EventHandler BusStopsDeletedEvent;

        BusStop Get(int id);
        List<BusStop> GetAll();
        List<BusStop> GetAllFavorites();
        void Add(BusStop busStop);
        void AddRange(IEnumerable<BusStop> busStops);
        void Delete(int id);
        void DeleteAll();
        void AddToFavorites(BusStop busStop);
        void RemoveFromFavorites(BusStop busStop);
    }
}
