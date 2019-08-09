using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private BusStop busStop;
        private RtmService _rtmService;

        public BusStop BusStop
        {
            get => busStop;
            set
            {
                busStop = value;
                OnPropertyChanged("BusStop");
            }
        }

        public BusStopPageVM()
        {
            _rtmService = new RtmService();
            _busStopRepository = new BusStopRepository();
        }

        public void OnAppearing()
        {
            var isInFavorites = _busStopRepository.IsInFavorites(BusStop.Id);
            if (isInFavorites)
                Console.WriteLine("Is in favorites");
            else
                Console.WriteLine("Not in favorites");
            DownloadCommand?.Execute(null);
        }

        public ICommand DownloadCommand => new Command(async () =>
        {
            IsBusy = true;
            BusStop = await _rtmService.GetBusStop(BusStop.Id);
            IsBusy = false;
        });

        public ICommand AddToFavoritesCommand => new Command(() => 
        {
            _busStopRepository.Add(BusStop);
        });
    }
}
