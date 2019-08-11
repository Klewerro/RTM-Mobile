using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Navigation;
using Rtm.Models;
using Rtm.Repositories;
using Rtm.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace Rtm.ViewModels
{
    public class BusStopPageVM : ViewModelBase
    {
        private readonly IBusStopRepository _busStopRepository;
        private readonly IRtmService _rtmService;
        private BusStop _busStop;       

        public BusStop BusStop
        {
            get => _busStop;
            set => SetProperty(ref _busStop, value);
        }

        public BusStop BusStopFromRepository { get; set; }

        public BusStopPageVM(INavigationService navigationService, 
            IBusStopRepository busStopRepository, 
            IRtmService rtmService) : base (navigationService)
        {
            _busStopRepository = busStopRepository;
            _rtmService = rtmService;
            BusStop = new BusStop();
        }

        public ICommand DownloadCommand => new DelegateCommand(async () =>
        {
            IsBusy = true;
            BusStop = await _rtmService.GetBusStop(BusStop.Id);
            IsBusy = false;
        });

        public ICommand AddToFavoritesCommand => new DelegateCommand(() => 
            _busStopRepository.AddToFavorites(BusStop));

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            IsBusy = true;
            var busStopId = (int) parameters["busStopIp"];
            BusStop = await _rtmService.GetBusStop(busStopId);
            BusStopFromRepository = _busStopRepository.Get(busStopId);
            
            CheckIfIsInFavorites();
            IsBusy = false;
        }


        private void CheckIfIsInFavorites()
        {
            if (BusStopFromRepository.IsFavorite)
                Console.WriteLine("Is in favorites");
            else
                Console.WriteLine("Not in favorites");
            DownloadCommand?.Execute(null);
        }
    }
}
