using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rztm.Services
{
    public interface IRtmService
    {
        Task<BusStop> GetBusStopAsync(int id);
        Task<List<BusStop>> GetAllBusStopsAsync();
        Task<List<(int, string)>> GetRouteListAsync();
        Task<List<(int routeId, string number)>> GetBusStopRouteListAsync(int busStopId);
        Task<List<(int busStopId, string busStopName)>> GetNextBusStopsAsync(int busStopId, int routeId);
    }
}
