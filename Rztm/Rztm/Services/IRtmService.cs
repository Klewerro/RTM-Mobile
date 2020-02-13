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
        Task<List<string>> GetBusStopCoursingLines(int busStopId);


        Task<List<(int, string)>> GetRouteList();
        Task<List<(int routeId, string number)>> GetBusStopRouteList(int busStopId);
        Task<List<(int busStopId, string busStopName)>> GetNextBusStops(int busStopId, int routeId);
    }
}
