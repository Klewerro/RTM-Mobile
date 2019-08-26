using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Rtm.Models;
using Rtm.Views;

namespace Rtm.ViewModels
{
    public class ListPageViewModelBase : ViewModelBase
    {
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


        public ListPageViewModelBase(INavigationService navigationService) : base(navigationService)
        {
            BusStops = new List<BusStop>();
        }


        public ICommand ItemTappedCommand => new DelegateCommand<BusStop>(async busStop =>
        {
            if (!IsInternetAccess)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("busStopIp", busStop.Id);
            await NavigationService.NavigateAsync(nameof(BusStopPage), parameters);
        });

        public ICommand SearchCommand => new DelegateCommand(async () =>
        {
            BusStops = BusStopsAll.Where(b => b.Name.ToLower().Contains(SearchText.ToLower())).ToList();    //Todo: change to BusStopsForSearch 
        });

        public ICommand SearchTextChangedCommand => new DelegateCommand(() =>
        {
            BusStops = SearchForBusStops(SearchText);
        });


        protected List<BusStop> SearchForBusStops(string searchText)
        {
            var query = from busStop in BusStopsAll
                        where busStop.Name.ToLower().StartsWith(searchText.ToLower())
                            || (!string.IsNullOrEmpty(busStop.CustomName) && busStop.CustomName.ToLower().StartsWith(searchText.ToLower()))
                        orderby busStop.Name, busStop.CustomName
                        select busStop;
            return query.ToList();
        }

    }
}
