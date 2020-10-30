using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Rztm.Helpers;
using Rztm.Helpers.Resources;
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
            var alertResult = await DialogService.DisplayAlertAsync(StringResources.Warning, $"{StringResources.AllSavedBusStopsWillBeDeleted}. " + 
                StringResources.ThisOperationCannotBeUndone_Continue, StringResources.Yes, StringResources.No);
            if (!alertResult)
                return;

            await _busStopRepository.DeleteAllAsync();
            Xamarin.Essentials.Preferences.Set("busStopsDownloaded", false);
            DialogService.DisplayToast(StringResources.AllBusStopsHaveBeenRemoved, ToastTime.Short);
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

            DialogService.DisplayToast(StringResources.NoLocationService_CheckGps, ToastTime.Long);
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
                    DialogService.DisplayToast(StringResources.ApplicationIsUpToDate, ToastTime.Short);
                    return;
                }   

                var dialogAnswer = await DialogService
                    .DisplayAlertAsync(StringResources.Warning, StringResources.DoYouWantToRemoveOldApplicationInstallationFile,
                    StringResources.Yes, StringResources.No);
                if (dialogAnswer)
                    _appUpdater.RemoveApkFile();
                return;
            }

            var description = $"{StringResources.CurrentVersion}: {currentAppVersion}\n{StringResources.Update}: {latestRelease.TagName}\n" +
                $"{StringResources.Description}:\n{latestRelease.Description}\n\n" +
                StringResources.DoYouWantToDownloadUpdateNow;

            var dialogResponse = await DialogService.DisplayAlertAsync(StringResources.Update,
                description, StringResources.Yes, StringResources.No);
            if (!dialogResponse)
                return;

            _appUpdater.UpdateApp(latestRelease);
        }
    }
}
