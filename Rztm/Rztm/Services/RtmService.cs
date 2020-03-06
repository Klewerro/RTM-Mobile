using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Rztm.Services
{
    public class RtmService : IRtmService
    {
        private HttpClient _httpClient;

        public RtmService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<BusStop> GetBusStopAsync(int id)
        {
            var response = await _httpClient.GetAsync($"http://einfo.erzeszow.pl/Home/GetTimetableReal?busStopId={id}");

            var doc = XDocument.Load(await response.Content.ReadAsStreamAsync());

            var stop = doc.Element("Schedules").Element("Stop");
            var busStop = new BusStop
            {
                Id = int.Parse(stop.Attribute("id").Value),
                Name = stop.Attribute("name").Value,
                Description = stop.Attribute("ds").Value
            };

            var rList = stop.Element("Day").Elements("R").ToList();
            foreach (var r in rList)
            {
                busStop.Departures.Add(new Departure
                {
                    Number = r.Attribute("nr").Value,
                    Direction = r.Attribute("dir").Value,
                    Time = r.Element("S").Attribute("t").Value,
                    HaveTicketMachine =  r.Attribute("vuw").Value.Equals("B")
                });
            }

            return busStop;
        }

        public async Task<List<BusStop>> GetAllBusStopsAsync()
        {
            var response = await _httpClient.GetAsync("http://einfo.erzeszow.pl/Home/GetMapBusStopList?ttId=0");
            var responseString = await response.Content.ReadAsStringAsync();
            
            var json = JsonConvert.DeserializeObject(responseString).ToString();
            var jArray = JArray.Parse(json);

            var busStops = new List<BusStop>();
            foreach (var item in jArray)
            {
                var busStop = new BusStop
                {
                    Id = (int)item[0],
                    Name = (string)item[1],
                    Latitude = (double)item[5],
                    Longitude = (double)item[4]
                };
                busStop.SetLatLng();
                busStops.Add(busStop);
            }

            return busStops.OrderBy(b => b.Name).ToList();
        }

        public async Task<List<(int, string)>> GetRouteListAsync()
        {
            var response = await _httpClient.GetStringAsync("http://einfo.erzeszow.pl/Home/GetRouteList?ttId=0");

            var json = JsonConvert.DeserializeObject(response).ToString();
            var jArray = JArray.Parse(json);

            var jValues = jArray[0][1].ToList();
            var result = new List<(int, string)>();

            for (int i = 0; i < jValues.Count; i = i + 2)
            {
                var tuple = ((int)jValues[i], jValues[i + 1].ToString());
                result.Add(tuple);               
            }

            return result;
        }

        public async Task<List<(int routeId, string number)>> GetBusStopRouteListAsync(int busStopId)
        {
            var response = await _httpClient.GetStringAsync($"http://einfo.erzeszow.pl/Home/GetBusStopRouteList?id={busStopId}&ttId=0");

            var json = JsonConvert.DeserializeObject(response).ToString();
            var jArray = JArray.Parse(json);

            var jValues = jArray[4][1].ToList();
            var result = new List<(int, string)>();

            for (int i = 0; i < jValues.Count; i = i + 2)
            {
                var tuple = ((int)jValues[i], jValues[i + 1].ToString());
                result.Add(tuple);
            }

            return result;
        }

        public async Task<List<(int busStopId, string busStopName)>> GetNextBusStopsAsync(int busStopId, int routeId)
        {
            var result = new List<(int busStopId, string busStopName)>();
            var startAdding = false;

            var finalRouteId = await GetFinalRouteIdAsync(busStopId, routeId);
            var response = await _httpClient.GetStringAsync($"http://einfo.erzeszow.pl/Home/GetBaseTripTimeTable?directionId={finalRouteId}&id_sub=0&ttId=0");

            var json = JsonConvert.DeserializeObject(response).ToString(); 
            var jArray = JArray.Parse(json);

            var jValues = jArray[0].ToList();
            foreach (var value in jValues)
            {
                if (startAdding)
                    result.Add((value[0].Value<int>(), value[1].Value<string>()));
                else if (value[0].Value<int>() == busStopId)
                    startAdding = true;
            }

            return result;
        }


        private async Task<int> GetFinalRouteIdAsync(int busStopId, int routeId)
        {
            var response = await _httpClient.GetStringAsync($"http://einfo.erzeszow.pl/Home/GetBusStopTimeTable?busStopId={busStopId}&routeId={routeId}&ttId=0");
            var json = JsonConvert.DeserializeObject(response).ToString();  //Todo: Extract parsing jArray to separate method
            var jArray = JArray.Parse(json);

            var result = jArray[2][0][1].ToObject<int>();
            return result;
        }
    }
}
