using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Rtm.Models
{
    public class BusStop
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LatLng { get; set; }


        public List<Departure> Departures { get; set; }


        public BusStop()
        {
            Departures = new List<Departure>();
        }

        public void SetLatLng() => LatLng = $"{Latitude},{Longitude}";

    }

}
