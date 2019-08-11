using Rtm.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rtm.Services
{
    public interface IRtmService
    {
        Task<BusStop> GetBusStop(int id);
        Task<List<BusStop>> GetAllBusStops();
    }
}
