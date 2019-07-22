using Rtm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Rtm.Services
{
    public class RtmService
    {
        private HttpClient _httpClient;

        public RtmService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<BusStop> GetBusStop()
        {
            var response = await _httpClient.GetAsync("http://einfo.erzeszow.pl/Home/GetTimetableReal?busStopId=1150");

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
    }
}
