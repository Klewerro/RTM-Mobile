using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rztm.Database;
using Rztm.Helpers;
using Rztm.Repositories;
using Xamarin.Essentials;

namespace Rztm.ViewModels
{
    public class TabsPageVM : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IBusStopRepository _busStopRepository;
        private readonly IGeolocator _locator;

        public TabsPageVM(INavigationService navigationService, 
            IPageDialogService pageDialogService,
            IBusStopRepository busStopRepository) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _busStopRepository = busStopRepository;
            _locator = CrossGeolocator.Current;
        }

        public ICommand DeleteBusStopsCommand => new DelegateCommand(async () =>
        {
            var alertResult = await _pageDialogService.DisplayAlertAsync("Ostrzeżenie", "Wszystkie zapisane przystanki zostaną usunięte. " + 
                "Tej operacji nie będzie można cofnąć. Kontynuować?", "Tak", "Nie");
            if (!alertResult)
                return;

            _busStopRepository.DeleteAll();
            DialogHelper.DisplayToast("Usunięto wszystkie przystanki", ToastTime.Short);
        });

        public ICommand OpenWebsiteCommand => new DelegateCommand(async () =>
            await Browser.OpenAsync("http://rtm.erzeszow.pl/", new BrowserLaunchOptions
            {
                PreferredToolbarColor = Color.IndianRed
            }));

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            await PrepareGeolocation();
        }


        private async Task PrepareGeolocation()
        {
            if (_locator.IsListening || !await IsGeolocationAvailable())
                return;

            await _locator.StartListeningAsync(
                TimeSpan.FromSeconds(10),
                50.0,
                true,
                new ListenerSettings
                {
                    ActivityType = ActivityType.Fitness,
                    AllowBackgroundUpdates = false,
                    PauseLocationUpdatesAutomatically = false
                });
        }

        private async Task<bool> IsGeolocationAvailable()
        {
            if (_locator.IsGeolocationAvailable && _locator.IsGeolocationEnabled)
                return true;

            DialogHelper.DisplayToast("Brak lokalizacji. Sprawdź GPS.", ToastTime.Long);
            await Task.Delay(9000);
            CheckInternetConnection();
            return false;
        }
    }
}
