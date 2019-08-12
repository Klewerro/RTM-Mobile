using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
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
        }

        public ICommand SearchCommand => new DelegateCommand(async () =>
        {
            BusStops = BusStopsAll.Where(b => b.Name.ToLower().Contains(SearchText.ToLower())).ToList();
        });

        public ICommand ItemTappedCommand => new DelegateCommand<BusStop>(async busStop =>
        {
            var parameters = new NavigationParameters();
            parameters.Add("busStopIp", busStop.Id);
            await NavigationService.NavigateAsync(nameof(BusStopPage), parameters);
        });

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await DownloadBusStopsIfEmpty();
        }


        private async Task DownloadBusStopsIfEmpty()
        {
            await Task.Delay(500);


            var repositoryStops = _busStopRepository.GetAll();
            if (!repositoryStops.Any())
            {
                var dialogResponse = await _pageDialogService.DisplayAlertAsync("Missing bus stop database",
                    "Local database containing bus stops missing. Do you want download it now?",
                    "Yes", "No");
                if (dialogResponse)
                {
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
                        await _pageDialogService.DisplayAlertAsync("No internet connection", "Please check your connection", "Ok");
                    } 
                    finally
                    {
                        IsBusy = false;
                    }
                }
            }
            else
            {
                BusStopsAll = repositoryStops.AsReadOnly();
                BusStops = repositoryStops;
            }
        }

    }
}
