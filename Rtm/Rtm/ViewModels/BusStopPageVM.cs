using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rtm.Helpers;
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
        private IGeolocator _locator;
        private BusStop _busStop;

        public BusStop BusStop
        {
            get => _busStop;
            set => SetProperty(ref _busStop, value);
        }


        public BusStopPageVM(INavigationService navigationService,
            IBusStopRepository busStopRepository,
            IRtmService rtmService) : base(navigationService)
        {
            _busStopRepository = busStopRepository;
            _rtmService = rtmService;
            _locator = CrossGeolocator.Current;
            BusStop = new BusStop();
        }

        public ICommand AppearingCommand => new DelegateCommand(() => 
        {
        });

        public ICommand DisappearingCommand => new DelegateCommand(() =>
        {
            _locator.PositionChanged -= GeolocatorOnPositionChanged;
            _locator.PositionError -= GeolocatorOnPositionError;
        });

        public ICommand RefreshCommand => new DelegateCommand(async () =>
        {
            IsBusy = true;
            BusStop.AddDownloadedData(await DownloadBusStop());
            IsBusy = false;
        });

        public ICommand FavoritesToggleCommand => new DelegateCommand(() =>
        {
            if (BusStop.IsFavorite)
            {
                _busStopRepository.RemoveFromFavorites(BusStop);
                DialogHelper.DisplayToast("Usunięto z ulubionych", ToastTime.Short,
                    "Cofnij", () => _busStopRepository.AddToFavorites(BusStop));
            }
            else
            {
                _busStopRepository.AddToFavorites(BusStop);
                DialogHelper.DisplayToast("Dodano do ulubionych", ToastTime.Short,
                    "Cofnij", () => _busStopRepository.RemoveFromFavorites(BusStop));
            }
        });

        public ICommand OpenInMapsCommand => new DelegateCommand(async () =>
        {
            var mapOptions = new Xamarin.Essentials.MapLaunchOptions
            {
                NavigationMode = Xamarin.Essentials.NavigationMode.Walking,
                Name = BusStop.Name
            };

            await Xamarin.Essentials.Map.OpenAsync(BusStop.Latitude, BusStop.Longitude, mapOptions);
        });

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            IsBusy = true;
            base.OnNavigatedTo(parameters);
            if (!IsInternetAccess)
                return;

            BusStop.Id = (int)parameters["busStopIp"];
            if (BusStop.Id == 0)
                return;

            BusStop = _busStopRepository.Get(BusStop.Id);

            if (parameters.ContainsKey("distance"))
                BusStop.Distance = (double)parameters["distance"];

            await PrepareBusStopUsingApiData();
            _locator.PositionChanged += GeolocatorOnPositionChanged;
            _locator.PositionError += GeolocatorOnPositionError;

            var currentPosition = await _locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            BusStop.Distance = currentPosition.CalculateDistance(BusStop.ConvertBusStopToPositon(), GeolocatorUtils.DistanceUnits.Kilometers);
            IsBusy = false;
        }

        private void GeolocatorOnPositionError(object sender, PositionErrorEventArgs e)
            => DialogHelper.DisplayToast("Błąd lokalizacji", ToastTime.Short);

        private void GeolocatorOnPositionChanged(object sender, PositionEventArgs e)
        {
            var position = e.Position;
            BusStop.Distance = position.CalculateDistance(BusStop.ConvertBusStopToPositon(), GeolocatorUtils.DistanceUnits.Kilometers);
        }

        private async Task PrepareBusStopUsingApiData()
        {
            var busStopFromApi = await DownloadBusStop();
            BusStop.AddDownloadedData(busStopFromApi);
        }

        private async Task<BusStop> DownloadBusStop()
        {
            if (!IsInternetAccess)
                return BusStop;

            IsBusy = true;
            try
            {
                var responseBusStop = await ApiCall(_rtmService.GetBusStop(BusStop.Id));
                return responseBusStop;
            }
            catch (Exceptions.ConnectionException)
            {
                ConnectionErrorRetry(async () => await PrepareBusStopUsingApiData());
            }
            finally
            {
                IsBusy = false;
            }

            return new BusStop();
        }
    }
}
