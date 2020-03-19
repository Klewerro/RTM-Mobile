using Prism.Mvvm;
using System.Collections.Generic;

namespace Rztm.Models
{
    public class Departure : BindableBase
    {
        private string _direction;
        private bool _isExpanded;
        private bool _isFetching;
        private List<string> _nextBusStopsNames;
        private string _selectedNextBusStopsName;

        public string Number { get; set; }

        public string Direction
        {
            get => _direction;
            set => SetProperty(ref _direction, value);
        }

        public string Time { get; set; }

        public bool HaveTicketMachine { get; set; }

        public int? RouteId { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public bool IsFetching
        {
            get => _isFetching;
            set => SetProperty(ref _isFetching, value);
        }

        public List<string> NextBusStopsNames
        {
            get => _nextBusStopsNames;
            set => SetProperty(ref _nextBusStopsNames, value);
        }

        public string SelectedNextBusStopsName
        {
            get => _selectedNextBusStopsName;
            set => SetProperty(ref _selectedNextBusStopsName, value);
        }

        public Departure()
        {
            NextBusStopsNames = new List<string>();
        }
    }
}
