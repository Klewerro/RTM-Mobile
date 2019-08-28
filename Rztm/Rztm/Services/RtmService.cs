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

        public async Task<BusStop> GetBusStop(int id)
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
                    Time = r.Element("S").Attribute("t").Value
                });
            }

            return busStop;
        }

        public async Task<List<BusStop>> GetAllBusStops()
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

    }
}
