using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;
using Prism.Mvvm;
using SQLite;

namespace Rztm.Models
{
    public class BusStop : BindableBase
    {
        private string _description;
        private bool _isFavorite;
        private List<Departure> _departures;
        private double _distance;
        private string _customName;
        private string _nameToDisplay;
        private string _name;

        [PrimaryKey]
        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                if (string.IsNullOrEmpty(CustomName))
                    NameToDisplay = _name;
            }
        }

        public string CustomName
        {
            get => _customName;
            set
            {
                SetProperty(ref _customName, value);
                if (!string.IsNullOrEmpty(_customName))
                    NameToDisplay = _customName;
                else
                    NameToDisplay = Name;
            }
        }

        [Ignore]
        public string NameToDisplay
        {
            get => _nameToDisplay;
            set => SetProperty(ref _nameToDisplay, value);
        }

        [Ignore]
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string LatLng { get; set; }

        public bool IsFavorite { get => _isFavorite; set => SetProperty(ref _isFavorite, value); }

        [Ignore]
        public List<Departure> Departures { get => _departures; set => SetProperty(ref _departures, value); }

        [Ignore]
        public double Distance { get => _distance; set => SetProperty(ref _distance, value); }


        public BusStop()
        {
            Departures = new List<Departure>();
        }

        public void SetLatLng() => LatLng = $"{Latitude},{Longitude}";

    }

}
