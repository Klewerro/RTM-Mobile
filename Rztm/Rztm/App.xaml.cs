using Xamarin.Forms;
using Prism;
using Prism.Ioc;
using Rztm.Views;
using Rztm.ViewModels;
using Rztm.Services;
using Rztm.Repositories;
using Rztm.Database;
using Prism.Plugin.Popups;
using Rg.Plugins.Popup.Pages;
using Rztm.Helpers;
using Plugin.DownloadManager.Abstractions;
using Plugin.DownloadManager;

namespace Rztm
{
    public partial class App
    {
        public string ApplicationVersion { get; set; } = "v1.0";

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer)
        {
#if DEBUG
            HotReloader.Current.Run(this);
#endif
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
            containerRegistry.Register<IAppPropertyService, AppPropertyService>();
            containerRegistry.Register<IAppUpdater, AppUpdater>();
            containerRegistry.RegisterInstance<IDownloadManager>(CrossDownloadManager.Current);
            containerRegistry.Register<IDialogService, DialogService>();
        }

    }
}
