using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rztm.Helpers;
using Rztm.Models;
using Rztm.Repositories;
using Rztm.Services;
using Rztm.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Rztm.ViewModels
{
    public class ListPageVM : ListPageViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IRtmService _rtmService;
        private readonly IBusStopRepository _busStopRepository;
        private bool _areBusStopsDownloaded;

        public bool AreBusStopsDownloaded
        {
            get => _areBusStopsDownloaded;
            set
            {
                Preferences.Set("busStopsDownloaded", value);
                SetProperty(ref _areBusStopsDownloaded, value);
            }
        }


        public ListPageVM(INavigationService navigationService, 
            IPageDialogService pageDialogService,
            IBusStopRepository busStopRepository,
            IRtmService rtmService) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _busStopRepository = busStopRepository;
            _rtmService = rtmService;
            BusStops = new List<BusStop>();
            _busStopRepository.BusStopsDeletedEvent += (s, e) =>
            {
                BusStops = new List<BusStop>();
                AreBusStopsDownloaded = false;
            };
        }

        
        public ICommand DownloadBusStopsCommand => new DelegateCommand(async () => 
            await DownloadBusStops());

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            AreBusStopsDownloaded = Preferences.Get("busStopsDownloaded", false);
            await DownloadBusStopsAutomaticallyIfEmpty();
        }


        private async Task DownloadBusStopsAutomaticallyIfEmpty()
        {
            await Task.Delay(500);

            var repositoryStops = _busStopRepository.GetAll();

            if (!repositoryStops.Any() && IsInternetAccess)
            {
                var dialogResponse = await _pageDialogService.DisplayAlertAsync("Missing bus stop database",
                    "Local database containing bus stops missing. Do you want download it now?",
                    "Yes", "No");
                if (dialogResponse)
                {
                    await DownloadBusStops();
                }
            }
            else
            {
                BusStopsAll = repositoryStops.AsReadOnly();
                BusStops = string.IsNullOrEmpty(SearchText) ? repositoryStops : SearchForBusStops(SearchText);
            }
        }

        private async Task DownloadBusStops()
        {
            if (!IsInternetAccess)
                return;
            IsBusy = true;
            try
            {
                var result = await ApiCall(_rtmService.GetAllBusStops());
                _busStopRepository.AddRange(result);
                BusStopsAll = result.AsReadOnly();
                BusStops = result;
                AreBusStopsDownloaded = true;
            }
            catch (Exceptions.ConnectionException ex)
            {
                ConnectionErrorRetry(async () => await DownloadBusStops());
                AreBusStopsDownloaded = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
