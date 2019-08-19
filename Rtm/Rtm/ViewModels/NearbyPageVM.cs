using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Rtm.Helpers;
using Rtm.Models;
using Rtm.Repositories;
using Rtm.Views;

namespace Rtm.ViewModels
{
    public class NearbyPageVM : ViewModelBase
    {
        private readonly IBusStopRepository _busStopRepository;
        private readonly IGeolocator _locator;
        private List<BusStop> _busStops;
        private Position _position;
        private int _rangePickerIndex;
        private int _unitPickerIndex;
        private readonly double[] _ranges;

        public List<BusStop> BusStops
        {
            get => _busStops;
            set => SetProperty(ref _busStops, value);
        }

        public ReadOnlyCollection<BusStop> BusStopsAll { get; set; }

        public Position Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public int RangePickerIndex
        {
            get => _rangePickerIndex;
            set => SetProperty(ref _rangePickerIndex, value);
        }

        
        public int UnitPickerIndex  //Todo: Move to section advanced
        {
            get => _unitPickerIndex;
            set => SetProperty(ref _unitPickerIndex, value);
        }


        public NearbyPageVM(INavigationService navigationService, IBusStopRepository busStopRepository) : base(navigationService)
        {
            _busStopRepository = busStopRepository;
            BusStops = new List<BusStop>();
            Position = new Position();
            RangePickerIndex = 1;
            UnitPickerIndex = 0;
            _locator = CrossGeolocator.Current;
            _ranges = new double[7] { 0.25, 0.5, 0.75, 1, 2, 3, 5 };

            _locator.PositionChanged += GeolocatorOnPositionChanged;
            _locator.PositionError += GeolocatorOnPositionError;
            _busStopRepository.BusStopsDeletedEvent += (s, e) => BusStops = new List<BusStop>();
        }

        ~NearbyPageVM()
        {
            _locator.PositionChanged -= GeolocatorOnPositionChanged;
            _locator.PositionError -= GeolocatorOnPositionError;
        }

        public ICommand AppearingCommand => new DelegateCommand(async () =>
        {
            await _locator.StartListeningAsync(
                TimeSpan.FromSeconds(10),
                50.0,
                true,
                new ListenerSettings
                {
                    ActivityType = ActivityType.Fitness,
                    AllowBackgroundUpdates = true,
                    PauseLocationUpdatesAutomatically = false
                }
            );
            BusStopsAll = _busStopRepository.GetAll().AsReadOnly();
        });

        public ICommand DisappearingCommand => new DelegateCommand(async () =>
        {
            await _locator.StopListeningAsync();
        });

        public ICommand RefreshCommand => new DelegateCommand(async () =>
        {
            IsBusy = true;
            await SetCurrentLocation();
            BusStops = GetNearbyBusStops(_ranges[RangePickerIndex]);
            IsBusy = false;
        });

        public ICommand ItemTappedCommand => new DelegateCommand<BusStop>(async busStop =>
        {
            var parameters = new NavigationParameters();
            parameters.Add("busStopIp", busStop.Id);
            parameters.Add("distance", busStop.Distance);
            await NavigationService.NavigateAsync(nameof(BusStopPage), parameters);
        });


        private void GeolocatorOnPositionChanged(object sender, PositionEventArgs e)
        {
            Position = e.Position;
            BusStops = GetNearbyBusStops(_ranges[RangePickerIndex]);
        }

        private void GeolocatorOnPositionError(object sender, PositionErrorEventArgs e)
        {
            DialogHelper.DisplayToast("Błąd lokalizacji", ToastTime.Short);
        }

        private async Task SetCurrentLocation()
        {
            IsBusy = true;
            Position = await _locator.GetLastKnownLocationAsync();

            var currentPosition = await _locator.GetPositionAsync(timeout: TimeSpan.FromSeconds(10));
            if (currentPosition != null)
                Position = currentPosition;
            else
                DialogHelper.DisplayToast("Nie można określić lokalizacji", ToastTime.Long);

            IsBusy = false;
        }

        private List<BusStop> GetNearbyBusStops(double range)
        {
            IsBusy = true;
            foreach (var stop in BusStopsAll)
            {
                var busStopPosition = new Position(stop.Longitude, stop.Latitude);  //Todo: swap
                stop.Distance = Position.CalculateDistance(busStopPosition, GeolocatorUtils.DistanceUnits.Kilometers);
            }

            var result = BusStopsAll.Where(b => b.Distance < range)
            .OrderBy(b => b.Distance)
            .ToList();

            IsBusy = false;
            return result;
        }


    }
}
