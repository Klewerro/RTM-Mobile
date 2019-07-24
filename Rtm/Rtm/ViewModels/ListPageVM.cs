using Rtm.Models;
using Rtm.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Rtm.ViewModels
{
    public class ListPageVM : ViewModelBase
    {
        private RtmService _rtmService;
        private List<BusStop> _busStops;
        private string _searchText;

        public List<BusStop> BusStops
        {
            get => _busStops;
            set
            {
                _busStops = value;
                OnPropertyChanged("BusStops");
            }
        }

        public ReadOnlyCollection<BusStop> BusStopsAll { get; set; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }


        public ListPageVM()
        {
            _rtmService = new RtmService();
            RefreshCommand?.Execute(null);
        }


        public ICommand RefreshCommand => new Command(async () =>
        {
            IsBusy = true;
            var result = await _rtmService.GetAllBusStops();
            BusStopsAll = result.AsReadOnly();
            BusStops = result;
            IsBusy = false;
        });

        public ICommand SearchCommand => new Command(async () =>
        {
            BusStops = BusStopsAll.Where(b => b.Name.ToLower().Contains(SearchText.ToLower())).ToList();
        });

    }
}
