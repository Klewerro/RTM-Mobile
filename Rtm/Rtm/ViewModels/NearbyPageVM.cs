﻿using System;
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
    public class NearbyPageVM : ListPageViewModelBase
    {
        private readonly IBusStopRepository _busStopRepository;
        private readonly IGeolocator _locator;
        private Position _position;
        private int _rangePickerIndex;
        private bool _isSearchInLineEnabled;
        private readonly double[] _ranges;

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

        public bool IsSearchInLineEnabled
        {
            get => _isSearchInLineEnabled;
            set => _isSearchInLineEnabled = value;
        }


        public NearbyPageVM(INavigationService navigationService, IBusStopRepository busStopRepository) : base(navigationService)
        {
            _busStopRepository = busStopRepository;
            Position = new Position();
            RangePickerIndex = 1;
            _locator = CrossGeolocator.Current;
            _ranges = new double[7] { 0.25, 0.5, 0.75, 1, 2, 3, 5 };

            _busStopRepository.BusStopsDeletedEvent += (s, e) => BusStops = new List<BusStop>();
            _locator.PositionChanged += GeolocatorOnPositionChanged;
            _locator.PositionError += GeolocatorOnPositionError;
        }

        ~NearbyPageVM()
        {
            _locator.PositionChanged -= GeolocatorOnPositionChanged;
            _locator.PositionError -= GeolocatorOnPositionError;
        }

        public ICommand AppearingCommand => new DelegateCommand(async () =>
        {
            await Task.Delay(200);
            Position = await GetCurrentPosition();
            BusStopsAll = _busStopRepository.GetAll().AsReadOnly();
            BusStops = GetNearbyBusStops(_ranges[RangePickerIndex]);
        });

        public ICommand DisappearingCommand => new DelegateCommand(() =>
        {
        });

        public ICommand RefreshCommand => new DelegateCommand(async () =>
        {
            IsBusy = true;
            Position = await GetCurrentPosition();
            BusStops = GetNearbyBusStops(_ranges[RangePickerIndex]);
            IsBusy = false;
        });


        private void GeolocatorOnPositionChanged(object sender, PositionEventArgs e)
        {
            if (BusStopsAll == null)
                return;
            Position = e.Position;
            BusStops = GetNearbyBusStops(_ranges[RangePickerIndex]);
        }

        private void GeolocatorOnPositionError(object sender, PositionErrorEventArgs e)
        {
            DialogHelper.DisplayToast("Błąd lokalizacji", ToastTime.Short);
        }

        private async Task<Position> GetCurrentPosition()
        {
            IsBusy = true;
            var result = await _locator.GetLastKnownLocationAsync();

            var currentPosition = await _locator.GetPositionAsync(timeout: TimeSpan.FromSeconds(10));
            if (currentPosition != null)
                result = currentPosition;
            else
                DialogHelper.DisplayToast("Nie można określić lokalizacji", ToastTime.Long);

            IsBusy = false;
            return result;
        }

        private List<BusStop> GetNearbyBusStops(double range)
        {
            IsBusy = true;
            foreach (var stop in BusStopsAll)
            {
                stop.Distance = Position.CalculateDistance(stop.ConvertBusStopToPositon(), GeolocatorUtils.DistanceUnits.Kilometers);
            }

            var result = BusStopsAll.Where(b => b.Distance < range)
            .OrderBy(b => b.Distance)
            .ToList();

            IsBusy = false;
            return result;
        }

        //private List<BusStop> GetNearbyBusStopsInLine(double errorMargin)
        //{
        //    var result = new List<BusStop>();
        //    var positionDiff = Position.Latitude / Position.Longitude;

        //    foreach (var stop in BusStopsAll)
        //    {
        //        var stopDiff = stop.Latitude / stop.Longitude;
        //        if (stopDiff < )
        //    }
        //}


    }
}
