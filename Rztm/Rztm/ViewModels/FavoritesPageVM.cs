using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rztm.Helpers;
using Rztm.Models;
using Rztm.Repositories;
using Rztm.Views;

namespace Rztm.ViewModels
{
    public class FavoritesPageVM : ListPageViewModelBase
    {
        private readonly IBusStopRepository _busStopRepository;


        public FavoritesPageVM(INavigationService navigationService,
            IDialogService dialogService,
            IBusStopRepository busStopRepository) 
            : base(navigationService, dialogService)
        {
            _busStopRepository = busStopRepository;
            _busStopRepository.BusStopsDeletedEvent += (s, e) => BusStops = new List<BusStop>();
        }


        public ICommand AppearingCommand => new DelegateCommand(async () =>
        {
            IsBusy = true;
            BusStops = await _busStopRepository.GetAllFavoritesAsync();
            BusStopsAll = BusStops.AsReadOnly();
            IsBusy = false;
        });

    }
}
