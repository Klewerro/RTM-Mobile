using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Rtm.Database;
using Rtm.Helpers;
using Rtm.Repositories;
using Xamarin.Essentials;

namespace Rtm.ViewModels
{
    public class TabsPageVM : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly IBusStopRepository _busStopRepository;

        public TabsPageVM(INavigationService navigationService, 
            IPageDialogService pageDialogService,
            IBusStopRepository busStopRepository) : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _busStopRepository = busStopRepository;
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
    }
}
