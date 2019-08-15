using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Rtm.Exceptions;
using Rtm.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Rtm.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        private bool _isBusy;
        private string _title;

        protected INavigationService NavigationService { get; private set; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public IDisposable ConnectionDialog { get; private set; }


        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
            Connectivity.ConnectivityChanged += Connectivity_ConnectionChanged;
        }

        public async Task<T> ApiCall<T>(Task<T> method) 
        {
            var currentConnection = Connectivity.NetworkAccess;
            if (currentConnection != NetworkAccess.Internet)
            {
                //Todo: Toast
                throw new ConnectionException("No internet connection");    
            }

            try
            {
                return await method;
            }
            catch (Exception otherEx)
            {
                throw new ApiException(otherEx.Message);
            }
        }

        public bool CheckConnection(Action action)
        {
            var currentConnection = Connectivity.NetworkAccess;
            if (currentConnection != NetworkAccess.Internet)
            {
                ConnectionDialog = DialogHelper.DisplayToast("Brak połączenia z internetem", ToastTime.OneHour, "Odśwież", async () =>
                {
                    ConnectionDialog.Dispose();
                    action.Invoke();
                });

                return false;
            }

            return true;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        private async void Connectivity_ConnectionChanged(object sender, ConnectivityChangedEventArgs e)
        {
            //await PageDialogService.DisplayAlertAsync("Warning", "Internet connection missing", "Ok");
            //Todo: Toast
        }

    }
}
