using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Rztm.Helpers;
using Rztm.Helpers.Resources;
using Rztm.Models;
using Rztm.Repositories;
using Rztm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rztm.ViewModels
{
    public class BusStopPageVM : ViewModelBase
    {
        private static readonly string parameterKeyIsInPopupView = "isInPopupView";

        private readonly IBusStopRepository _busStopRepository;
        private readonly IRtmService _rtmService;
        private IGeolocator _locator;
        private CancellationTokenSource _ctsCurrentPosition;
        private bool _isInPopupView;

        private BusStop _busStop;
        private string _changeNameText;
        private bool _isRenameVisible;

        public BusStop BusStop
        {
            get => _busStop;
            set
            {
                SetProperty(ref _busStop, value);
                ChangeNameText = BusStop.CustomName;
            }
        }

        public string ChangeNameText
        {
            get => _changeNameText;
            set => SetProperty(ref _changeNameText, value);
        }

        public bool IsRenameVisible
        {
            get => _isRenameVisible;
            set => SetProperty(ref _isRenameVisible, value);
        }


        public BusStopPageVM(INavigationService navigationService,
            IDialogService dialogService,
            IBusStopRepository busStopRepository,
            IRtmService rtmService) : base(navigationService, dialogService)
        {
            _busStopRepository = busStopRepository;
            _rtmService = rtmService;
            _locator = CrossGeolocator.Current;
            BusStop = new BusStop();
            _ctsCurrentPosition = new CancellationTokenSource();
        }

        public ICommand AppearingCommand => new DelegateCommand(() =>
        {
        });

        public ICommand DisappearingCommand => new DelegateCommand(() =>
        {
            _locator.PositionChanged -= GeolocatorOnPositionChanged;
            _locator.PositionError -= GeolocatorOnPositionError;
            _ctsCurrentPosition.Cancel();
        });

        public ICommand RefreshCommand => new DelegateCommand(async () =>
        {
            IsBusy = true;
            BusStop.AddDownloadedData(await DownloadBusStopAsync());
            BusStop.Distance = await GetDistanceToBusStopAsync();
            IsBusy = false;
        });

        public ICommand FavoritesToggleCommand => new DelegateCommand(() =>
        {
            if (BusStop.IsFavorite)
            {
                _busStopRepository.RemoveFromFavorites(BusStop);
                DialogService.DisplayToast(StringResources.RemovedFromFavorites, ToastTime.Short,
                    StringResources.Undo, () => _busStopRepository.AddToFavorites(BusStop));
            }
            else
            {
                _busStopRepository.AddToFavorites(BusStop);
                DialogService.DisplayToast(StringResources.AddedToFavorites, ToastTime.Short,
                    StringResources.Undo, () => _busStopRepository.RemoveFromFavorites(BusStop));
            }
        });

        public ICommand OpenInMapsCommand => new DelegateCommand(async () =>
        {
            var mapOptions = new Xamarin.Essentials.MapLaunchOptions
            {
                NavigationMode = Xamarin.Essentials.NavigationMode.Walking,
                Name = BusStop.Name
            };

            await Xamarin.Essentials.Map.OpenAsync(BusStop.Latitude, BusStop.Longitude, mapOptions);
        });

        public ICommand RenameClickedCommand => new DelegateCommand(() => IsRenameVisible = true);

        public ICommand DisplayAsApplicationShortutCommand => new DelegateCommand(() =>
        {
            Xamarin.Forms.MessagingCenter.Send(string.Empty, Constants.CreateBusStopShortcut, (BusStop.Id, BusStop.NameToDisplay));
        });

        public ICommand SaveNameCommand => new DelegateCommand(() =>
        {
            BusStop.CustomName = ChangeNameText;
            _busStopRepository.Rename(BusStop, ChangeNameText);
        });

        public ICommand DiscardCustomNameCommand => new DelegateCommand(() =>
            ChangeNameText = BusStop.CustomName);

        public ICommand RemoveCustomNameCommand => new DelegateCommand(() =>
        {
            ChangeNameText = null;
            BusStop.CustomName = ChangeNameText;
            _busStopRepository.Rename(BusStop, ChangeNameText);
        });

        public ICommand ItemTappedCommand => new DelegateCommand<Departure>(async departure =>
        {
            departure.IsExpanded = !departure.IsExpanded;
            if (departure.NextBusStopsNames.Count == 0)
            {
                departure.IsFetching = true;
                var nextStops = await _rtmService.GetNextBusStopsAsync(BusStop.Id, departure.RouteId.GetValueOrDefault());
                departure.NextBusStopsNames = nextStops.Select(x => x.busStopName).ToList();
                departure.IsFetching = false;
            }
        });

        public ICommand CollectionItemPicked => new DelegateCommand<Departure>(async departure =>
        {
            if (_isInPopupView)
                return;

            //Setting null to SelectedNextBusStopsName causing second call, so retire on null
            if (departure.SelectedNextBusStopsName == null)
                return;

            NavigationParameters parameters = new NavigationParameters();
            var selectedNextBusStop = _busStopRepository.Get(departure.SelectedNextBusStopsName);

            parameters.Add(Constants.ParameterKeyBusStopId, selectedNextBusStop.Id);
            parameters.Add(parameterKeyIsInPopupView, true);

            departure.SelectedNextBusStopsName = null;
            await NavigationService.NavigateAsync(nameof(Views.BusStopPopupPage), parameters);
        });


        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey(parameterKeyIsInPopupView))
            {
                _isInPopupView = (bool)parameters[parameterKeyIsInPopupView];
                if (!_isInPopupView)
                    return;
            }


            IsBusy = true;
            base.OnNavigatedTo(parameters);
            if (!IsInternetAccess)
                return;

            var areContains = parameters.ContainsKey(Constants.ParameterKeyBusStopId);

            BusStop.Id = (int)parameters[Constants.ParameterKeyBusStopId];
            if (BusStop.Id == 0)
                return;

            if (parameters.ContainsKey(Constants.ParameterKeyIsBusStopShortcut))
            {
                IsBusy = true;
                await Task.Delay(50);
                IsBusy = false;
            }
                

            BusStop = _busStopRepository.Get(BusStop.Id);

            if (parameters.ContainsKey("distance"))
                BusStop.Distance = (double)parameters["distance"];

            await PrepareBusStopUsingApiDataAsync();
            _locator.PositionChanged += GeolocatorOnPositionChanged;
            _locator.PositionError += GeolocatorOnPositionError;
            BusStop.Distance = await GetDistanceToBusStopAsync();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            if (_isInPopupView)
                parameters.Add(parameterKeyIsInPopupView, false);

        }


        private void GeolocatorOnPositionError(object sender, PositionErrorEventArgs e)
            => DialogService.DisplayToast(StringResources.LocationError, ToastTime.Short);

        private void GeolocatorOnPositionChanged(object sender, PositionEventArgs e)
        {
            var position = e.Position;
            BusStop.Distance = position.CalculateDistance(BusStop.ConvertBusStopToPositon(), GeolocatorUtils.DistanceUnits.Kilometers);
        }

        private async Task<double> GetDistanceToBusStopAsync()
        {
            try
            {
                if (!_locator.IsGeolocationAvailableAndEnabled())
                    return default;

                var currentPosition = await _locator.GetPositionAsync(TimeSpan.FromSeconds(10), _ctsCurrentPosition.Token);
                var distance = currentPosition.CalculateDistance(BusStop.ConvertBusStopToPositon(), GeolocatorUtils.DistanceUnits.Kilometers);

                if (currentPosition.IsZeroPosition())
                    throw new OperationCanceledException(); //For some phones, where cancellation token not throwing

                return distance;
            }
            catch (OperationCanceledException)
            {
                //Nothing needed, just BusStop.Distance is unset
            }
            finally
            {
                IsBusy = false;
            }

            return default;
        }

        private async Task PrepareBusStopUsingApiDataAsync()
        {
            var busStopFromApi = await DownloadBusStopAsync();
            BusStop.AddDownloadedData(busStopFromApi);
        }

        private async Task<BusStop> DownloadBusStopAsync()
        {
            if (!IsInternetAccess)
                return BusStop;

            IsBusy = true;
            try
            {
                var responseBusStop = await ApiCall(_rtmService.GetBusStopAsync(BusStop.Id));
                var stopRouteList = await ApiCall(_rtmService.GetBusStopRouteListAsync(BusStop.Id));
                responseBusStop.CoursingLines = stopRouteList.Select(x => x.number).ToList();
                responseBusStop.Departures.AddRouteIdsToDepartures(stopRouteList);
                return responseBusStop;
            }
            catch (Exceptions.ConnectionException)
            {
                ConnectionErrorRetry(async () => await PrepareBusStopUsingApiDataAsync());
            }
            finally
            {
                IsBusy = false;
            }

            return new BusStop();
        }
    }
}
