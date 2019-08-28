using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rztm.Services
{
    public interface IRtmService
    {
        Task<BusStop> GetBusStop(int id);
        Task<List<BusStop>> GetAllBusStops();
    }
}
