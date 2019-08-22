using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rtm.Models;
using Rtm.Repositories;
using Rtm.Views;

namespace Rtm.ViewModels
{
    public class FavoritesPageVM : ListPageViewModelBase
    {
        private readonly IBusStopRepository _busStopRepository;


        public FavoritesPageVM(INavigationService navigationService, IBusStopRepository busStopRepository) 
            : base(navigationService)
        {
            _busStopRepository = busStopRepository;
            _busStopRepository.BusStopsDeletedEvent += (s, e) => BusStops = new List<BusStop>();
        }


        public ICommand AppearingCommand => new DelegateCommand(() =>
        {
            IsBusy = true;
            BusStops = _busStopRepository.GetAllFavorites();
            BusStopsAll = BusStops.AsReadOnly();
            IsBusy = false;
        });

    }
}
