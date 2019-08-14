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
    public class FavoritesPageVM : ViewModelBase
    {
        private readonly IBusStopRepository _busStopRepository;
        private List<BusStop> _favoritesBusStops;

        public List<BusStop> FavoritesBusStops { get => _favoritesBusStops; set => SetProperty(ref _favoritesBusStops, value); }


        public FavoritesPageVM(INavigationService navigationService, IBusStopRepository busStopRepository) 
            : base(navigationService)
        {
            _busStopRepository = busStopRepository;
            FavoritesBusStops = new List<BusStop>();
        }


        public ICommand ItemTappedCommand => new DelegateCommand<BusStop>(async busStop =>
        {
            var parameters = new NavigationParameters();
            parameters.Add("busStopIp", busStop.Id);
            await NavigationService.NavigateAsync(nameof(BusStopPage), parameters);
        });

        public ICommand AppearingCommand => new DelegateCommand(() =>
        {
            IsBusy = true;
            FavoritesBusStops = _busStopRepository.GetAllFavorites();
            IsBusy = false;
        });

    }
}
