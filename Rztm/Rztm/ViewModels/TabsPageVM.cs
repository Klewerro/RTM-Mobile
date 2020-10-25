using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Rztm.Helpers;
using Rztm.Repositories;
using Rztm.Services;
using Xamarin.Essentials;

namespace Rztm.ViewModels
{
    public class TabsPageVM : ViewModelBase
    {
        private readonly IGithubService _githubService;
        private readonly IBusStopRepository _busStopRepository;
        private readonly IGeolocator _locator;
        private readonly IAppUpdater _appUpdater;

        public TabsPageVM(INavigationService navigationService,
            IDialogService dialogService,
            IGithubService githubService,
            IBusStopRepository busStopRepository,
            IAppUpdater appUpdater) : base(navigationService, dialogService)
        {
            _githubService = githubService;
            _busStopRepository = busStopRepository;
            _locator = CrossGeolocator.Current;
            _appUpdater = appUpdater;
        }

        public TabsPageVM(INavigationService navigationService,
            IDialogService dialogService,
            IGithubService githubService,
            IBusStopRepository busStopRepository,
            IAppUpdater appUpdater,
            IGeolocator geolocator) : base(navigationService, dialogService)
        {
            _githubService = githubService;
            _busStopRepository = busStopRepository;
            _locator = geolocator;
            _appUpdater = appUpdater;
        }

        public ICommand DeleteBusStopsCommand => new DelegateCommand(async () =>
        {
            var alertResult = await DialogService.DisplayAlertAsync("Ostrzeżenie", "Wszystkie zapisane przystanki zostaną usunięte. " + 
                "Tej operacji nie będzie można cofnąć. Kontynuować?", "Tak", "Nie");
            if (!alertResult)
                return;

            _busStopRepository.DeleteAll();
            Xamarin.Essentials.Preferences.Set("busStopsDownloaded", false);
            DialogService.DisplayToast("Usunięto wszystkie przystanki", ToastTime.Short);
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

            Xamarin.Forms.MessagingCenter.Subscribe<string, string>(string.Empty, Constants.OpenBusStopShortcut, 
                async (s, androidIntentData) =>
            {
                var idText = androidIntentData.Substring(13, androidIntentData.Length - 13);
                var busStopId = int.Parse(idText);

                var parametersBusStop = new NavigationParameters();
                parametersBusStop.Add(Constants.ParameterKeyBusStopId, busStopId);
                parametersBusStop.Add(Constants.ParameterKeyIsBusStopShortcut, true);
                await NavigationService.NavigateAsync(nameof(Views.BusStopPage), parametersBusStop);
            });

            await PrepareGeolocationAsync();
        }


        private async Task PrepareGeolocationAsync()
        {
            if (_locator.IsListening || !await IsGeolocationAvailableAsync())
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

        private async Task<bool> IsGeolocationAvailableAsync()
        {
            if (_locator.IsGeolocationAvailable && _locator.IsGeolocationEnabled)
                return true;

            DialogService.DisplayToast("Brak lokalizacji. Sprawdź GPS.", ToastTime.Long);
            await Task.Delay(9000);
            CheckInternetConnection();
            return false;
        }

        private async Task UpdateAppAsync()
        {
            var latestRelease = await _githubService.GetLatestVersionCodeAsync();
            var currentAppVersion = _appUpdater.GetCurrentVersion();

            if (latestRelease.CheckIsLatestRelease(currentAppVersion))
            {
                if (!_appUpdater.CheckIsAppAfterUpdate())
                {
                    DialogService.DisplayToast("Aplikacja jest aktualna", ToastTime.Short);
                    return;
                }   

                var dialogAnswer = await DialogService
                    .DisplayAlertAsync("Uwaga", "Czy chcesz usunąć stary plik instalacyjny aplikacji (.apk)?",
                    "Tak", "Nie");
                if (dialogAnswer)
                    _appUpdater.RemoveApkFile();
                return;
            }

            var description = $"Wersja obecna: {currentAppVersion}\nAktualizacja: {latestRelease.TagName}\n" +
                $"Opis:\n{latestRelease.Description}\n\n" +
                $"Czy chcesz pobrać aktualizację teraz?";

            var dialogResponse = await DialogService.DisplayAlertAsync("Aktualizacja",
                description, "Tak", "Nie");
            if (!dialogResponse)
                return;

            _appUpdater.UpdateApp(latestRelease);
        }
    }
}
