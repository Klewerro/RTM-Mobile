using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Prism.Mvvm;
using SQLite;

namespace Rtm.Models
{
    public class BusStop : BindableBase
    {
        private bool _isFavorite;

        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        [Ignore]
        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LatLng { get; set; }

        public bool IsFavorite { get => _isFavorite; set => SetProperty(ref _isFavorite, value); }

        [Ignore]
        public List<Departure> Departures { get; set; }


        public BusStop()
        {
            Departures = new List<Departure>();
        }

        public void SetLatLng() => LatLng = $"{Latitude},{Longitude}";

    }

}
