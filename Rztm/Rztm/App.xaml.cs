using Xamarin.Forms;
using Prism;
using Prism.Ioc;
using Rztm.Views;
using Rztm.ViewModels;
using Rztm.Services;
using Rztm.Repositories;
using Rztm.Database;
using Prism.Plugin.Popups;
using Rztm.Helpers;
using Plugin.DownloadManager.Abstractions;
using Plugin.DownloadManager;

namespace Rztm
{
    public partial class App
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer, string androidIntentData = null) : base(initializer)
        {
#if DEBUG
            HotReloader.Current.Run(this);
#endif

            if (!string.IsNullOrEmpty(androidIntentData))
            {
                if (androidIntentData.StartsWith("busStopShort_"))
                {
                    MessagingCenter.Send(string.Empty, Constants.OpenBusStopShortcut, androidIntentData);
                }
                else
                {
                    MessagingCenter.Send(string.Empty, Constants.DroidAppShortcutInvoked, androidIntentData);
                }

            }

        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync($"NavigationPage/{nameof(TabsPage)}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.RegisterForNavigation<TabsPage, TabsPageVM>();
            containerRegistry.RegisterForNavigation<BusStopPage, BusStopPageVM>();
            containerRegistry.RegisterForNavigation<ListPage, ListPageVM>();
            containerRegistry.RegisterForNavigation<FavoritesPage, FavoritesPageVM>();
            containerRegistry.RegisterForNavigation<NearbyPage, NearbyPageVM>();
            containerRegistry.RegisterForNavigation<BusStopPopupPage, BusStopPageVM>();

            containerRegistry.Register<IRtmService, RtmService>();
            containerRegistry.Register<IGithubService, GithubService>();
            containerRegistry.RegisterSingleton<IBusStopRepository, BusStopRepository>();
            containerRegistry.Register<ILocalDatabase, LocalDatabase>();
            containerRegistry.Register<IAppUpdater, AppUpdater>();
            containerRegistry.RegisterInstance<IDownloadManager>(CrossDownloadManager.Current);
            containerRegistry.Register<IDialogService, DialogService>();
        }

    }
}
