using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Unity;
using Prism;
using Prism.Ioc;
using Rtm.Views;
using Rtm.ViewModels;
using Rtm.Services;
using Rtm.Repositories;

namespace Rtm
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
            containerRegistry.RegisterForNavigation<TabsPage>();
            containerRegistry.RegisterForNavigation<BusStopPage, BusStopPageVM>();
            containerRegistry.RegisterForNavigation<ListPage, ListPageVM>();
            containerRegistry.RegisterForNavigation<FavoritesPage, ListPageVM>();

            containerRegistry.Register<IRtmService, RtmService>();
            containerRegistry.Register<IBusStopRepository, BusStopRepository>();
        }

    }
}
