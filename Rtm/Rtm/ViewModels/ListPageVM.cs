using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rtm.Helpers;
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
        private readonly IPageDialogService _pageDialogService;
        private readonly IRtmService _rtmService;
        private readonly IBusStopRepository _busStopRepository;

        private List<BusStop> _busStops;
        private string _searchText;

        public List<BusStop> BusStops
        {
            get => _busStops;
            set => SetProperty(ref _busStops, value);
        }

        public ReadOnlyCollection<BusStop> BusStopsAll { get; set; }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
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
            _busStopRepository.BusStopsDeletedEvent += (s, e) => BusStops = new List<BusStop>();
        }

        public ICommand SearchCommand => new DelegateCommand(async () =>
        {
            BusStops = BusStopsAll.Where(b => b.Name.ToLower().Contains(SearchText.ToLower())).ToList();
        });

        public ICommand ItemTappedCommand => new DelegateCommand<BusStop>(async busStop =>
        {
            if (!IsInternetAccess)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("busStopIp", busStop.Id);
            await NavigationService.NavigateAsync(nameof(BusStopPage), parameters);
        });

        public ICommand SearchTextChangedCommand => new DelegateCommand(() =>
        {
            BusStops = SearchForBusStops(SearchText);
        });

        public ICommand DownloadBusStopsCommand => new DelegateCommand(async () => 
            await DownloadBusStops());

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
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
            }
            catch (Exceptions.ConnectionException ex)
            {
                ConnectionErrorRetry(async () => await DownloadBusStops());
            }
            finally
            {
                IsBusy = false;
            }
        }

        private List<BusStop> SearchForBusStops(string searchText)
            => BusStopsAll.Where(b => b.Name.ToLower().Contains(searchText.ToLower())).ToList();

    }
}
