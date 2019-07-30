using Rtm.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rtm.Repositories
{
    public interface IFavoritesRepository
    {
        List<BusStop> GetAll();
        void Add(BusStop busStop);
        void Remove(int id);
        bool IsInFavorites(int id);
    }
}
