using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private readonly IPageDialogService _pageDialogService;
        private readonly IBusStopRepository _busStopRepository;
        private readonly IRtmService _rtmService;
        private BusStop _busStop;       

        public BusStop BusStop
        {
            get => _busStop;
            set => SetProperty(ref _busStop, value);
        }


        public BusStopPageVM(INavigationService navigationService, 
            IPageDialogService pageDialogService,
            IBusStopRepository busStopRepository, 
            IRtmService rtmService) : base (navigationService)
        {
            _pageDialogService = pageDialogService;
            _busStopRepository = busStopRepository;
            _rtmService = rtmService;
            BusStop = new BusStop();
        }

        public ICommand RefreshCommand => new DelegateCommand(async () =>
        {
            IsBusy = true;
            BusStop = await DownloadBusStop();
            IsBusy = false;
        });

        public ICommand AddToFavoritesCommand => new DelegateCommand(() => 
            _busStopRepository.AddToFavorites(BusStop));

        public ICommand RemoveFromFavoritesCommand => new DelegateCommand(() => 
            _busStopRepository.RemoveFromFavorites(BusStop));

        public ICommand ChangeCommand => new DelegateCommand(() =>
        {
            BusStop.IsFavorite = !BusStop.IsFavorite;
        });

        public ICommand FavoritesToggleCommand => new DelegateCommand(() => 
        {
            if (BusStop.IsFavorite)
            {
                _busStopRepository.RemoveFromFavorites(BusStop);
            }
            else
            {
                _busStopRepository.AddToFavorites(BusStop);
            }
        });

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            BusStop.Id = (int)parameters["busStopIp"];
            if (BusStop.Id == 0)
                return;

            BusStop = _busStopRepository.Get(BusStop.Id);
            var busStopFromApi = await DownloadBusStop();
            BusStop.AddDownloadedData(busStopFromApi);                              
        }


        private async Task<BusStop> DownloadBusStop()
        {
            IsBusy = true;
            try
            {
                var responseBusStop = await ApiCall(_rtmService.GetBusStop(BusStop.Id));
                return responseBusStop;
            }
            catch (Exceptions.ConnectionException ex)
            {
                await _pageDialogService.DisplayAlertAsync("No internet connection", "Please check your connection", "Ok");
                await NavigationService.GoBackAsync();
            }
            finally
            {
                IsBusy = false;
            }

            return new BusStop();
        }
    }
}
