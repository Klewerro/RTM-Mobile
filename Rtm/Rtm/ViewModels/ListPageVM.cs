using Rtm.Models;
using Rtm.Repositories;
using Rtm.Services;
using Rtm.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Rtm.ViewModels
{
    public class ListPageVM : ViewModelBase
    {
        private RtmService _rtmService;
        private IBusStopRepository _busStopRepository;

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
            _busStopRepository = new BusStopRepository();

            DownloadBusStopsIfEmpty();
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

        public ICommand DownloadBusStopsCommand => new Command(async () =>
        {
            IsBusy = true;
            var result = await _rtmService.GetAllBusStops();
            BusStopsAll = result.AsReadOnly();
            BusStops = result;
            
            IsBusy = false;
        });

        private async Task DownloadBusStopsIfEmpty()
        {
            var repositoryStops = _busStopRepository.GetAll();
            if (!_busStopRepository.GetAll().Any())
            {
                var result = await _rtmService.GetAllBusStops();
                var test2 = _busStopRepository.GetAll();
                _busStopRepository.AddRange(result);
                var test3 = _busStopRepository.GetAll();
                BusStopsAll = result.AsReadOnly();
                BusStops = result;
            }
            else
            {
                BusStopsAll = repositoryStops.AsReadOnly();
                BusStops = repositoryStops;
            }
        }

    }
}
