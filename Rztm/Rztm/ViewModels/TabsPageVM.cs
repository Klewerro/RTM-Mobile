using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rztm.Helpers;
using Rztm.Repositories;
using Rztm.Services;
using Xamarin.Essentials;

namespace Rztm.ViewModels
{
    public class TabsPageVM : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IGithubService _githubService;
        private readonly IBusStopRepository _busStopRepository;
        private readonly IGeolocator _locator;

        public TabsPageVM(INavigationService navigationService, 
            IPageDialogService pageDialogService,
            IGithubService githubService,
            IBusStopRepository busStopRepository) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _githubService = githubService;
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
            Xamarin.Essentials.Preferences.Set("busStopsDownloaded", false);
            DialogHelper.DisplayToast("Usunięto wszystkie przystanki", ToastTime.Short);
        });

        public ICommand OpenWebsiteCommand => new DelegateCommand(async () =>
            await Browser.OpenAsync("http://rtm.erzeszow.pl/", new BrowserLaunchOptions
            {
                PreferredToolbarColor = Color.IndianRed
            }));

        public ICommand CheckForUpdatesCommand => new DelegateCommand(async () 
            => await UpdateAppAsync());

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

        private async Task UpdateAppAsync()
        {
            var latestRelease = await _githubService.GetLatestVersionCode();
            var appUpdater = new AppUpdater();

            if (latestRelease.IsCurrentAppVersionLatestRelease)
            {
                if (!appUpdater.CheckIsAppAfterUpdate())
                    return;

                var dialogAnswer = await _pageDialogService
                    .DisplayAlertAsync("Uwaga", "Czy chcesz usunąć stary plik instalacyjny aplikacji (.apk)?",
                    "Tak", "Nie");
                if (dialogAnswer)
                    appUpdater.RemoveApkFile();
                return;
            }

            var dialogResponse = await _pageDialogService.DisplayAlertAsync("Aktualizacja",
                $"Aktualizacja {latestRelease.TagName.Remove(0, 1)} jest możliwa do pobrania. Czy chcesz pobrać ją teraz?",
                "Tak", "Nie");
            if (!dialogResponse)
                return;

            appUpdater.UpdateApp(latestRelease);
        }
    }
}
