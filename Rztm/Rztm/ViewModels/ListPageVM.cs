using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rztm.Helpers;
using Rztm.Helpers.Resources;
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
            IDialogService dialogService,
            IBusStopRepository busStopRepository,
            IRtmService rtmService) : base(navigationService, dialogService)
        {
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
            await DownloadBusStopsAsync());

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            AreBusStopsDownloaded = Preferences.Get("busStopsDownloaded", false);
            await DownloadBusStopsAutomaticallyIfEmptyAsync();
        }


        private async Task DownloadBusStopsAutomaticallyIfEmptyAsync()
        {
            await Task.Delay(500);

            var repositoryStops = await _busStopRepository.GetAllAsync();

            if (!repositoryStops.Any() && IsInternetAccess)
            {
                var dialogResponse = await DialogService.DisplayAlertAsync(StringResources.MissingBusStopDatabase,
                    StringResources.LocalDatabaseContainingBusStopsMissing_DoYouWantDownloadItNow,
                    StringResources.Yes, StringResources.No);
                if (dialogResponse)
                {
                    await DownloadBusStopsAsync();
                }
            }
            else
            {
                BusStopsAll = repositoryStops.AsReadOnly();
                BusStops = string.IsNullOrEmpty(SearchText) ? repositoryStops : SearchForBusStops(SearchText);
            }
        }

        private async Task DownloadBusStopsAsync()
        {
            if (!IsInternetAccess)
                return;
            IsBusy = true;
            try
            {
                var result = await ApiCall(_rtmService.GetAllBusStopsAsync());
                await _busStopRepository.AddRangeAsync(result);
                BusStopsAll = result.AsReadOnly();
                BusStops = result;
                AreBusStopsDownloaded = true;
            }
            catch (Exceptions.ConnectionException)
            {
                ConnectionErrorRetry(async () => await DownloadBusStopsAsync());
                AreBusStopsDownloaded = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
