using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Unity;
using Prism;
using Prism.Ioc;
using Rztm.Views;
using Rztm.ViewModels;
using Rztm.Services;
using Rztm.Repositories;
using Rztm.Database;

namespace Rztm
{
    public partial class App
    {

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
            containerRegistry.RegisterForNavigation<TabsPage, TabsPageVM>();
            containerRegistry.RegisterForNavigation<BusStopPage, BusStopPageVM>();
            containerRegistry.RegisterForNavigation<ListPage, ListPageVM>();
            containerRegistry.RegisterForNavigation<FavoritesPage, FavoritesPageVM>();
            containerRegistry.RegisterForNavigation<NearbyPage, NearbyPageVM>();

            containerRegistry.Register<IRtmService, RtmService>();
            containerRegistry.RegisterSingleton<IBusStopRepository, BusStopRepository>();
            containerRegistry.Register<ILocalDatabase, LocalDatabase>();
        }

    }
}
