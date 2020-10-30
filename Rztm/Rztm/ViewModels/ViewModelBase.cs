using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Rztm.Exceptions;
using Rztm.Helpers;
using Rztm.Helpers.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Rztm.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        private bool _isBusy;
        private string _title;
        private bool _isInternetAccess;

        protected INavigationService NavigationService { get; private set; }
        protected IDialogService DialogService { get; private set; }

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

        public bool IsInternetAccess
        {
            get => _isInternetAccess;
            set => SetProperty(ref _isInternetAccess, value);
        }

        public IDisposable ConnectionDialog { get; private set; }


        public ViewModelBase(INavigationService navigationService, IDialogService dialogService)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
#if DEBUG
            if (DeviceInfo.Platform == DevicePlatform.Unknown)  //for unitTesting purposes
                return;
#endif
            CheckInternetConnection();
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

        public bool ConnectionErrorRetry(Action action)
        {
            var currentConnection = Connectivity.NetworkAccess;
            if (currentConnection != NetworkAccess.Internet)
            {
                ConnectionDialog = DialogService.DisplayToast(StringResources.ConnectionError, 
                    ToastTime.OneHour, StringResources.TryAgain,  () =>
                {
                    ConnectionDialog.Dispose();
                    action.Invoke();
                });

                return false;
            }

            return true;
        }

        public virtual void CheckInternetConnection()
        {
            IsInternetAccess = Connectivity.NetworkAccess == NetworkAccess.Internet ? true : false;
            if (!IsInternetAccess)
                DialogService.DisplayToast(StringResources.MissingInternetConnection, ToastTime.OneHour);
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            CheckInternetConnection();
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        private void Connectivity_ConnectionChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsInternetAccess = Connectivity.NetworkAccess == NetworkAccess.Internet ? true : false;
            if (IsInternetAccess)
                DialogService.DisplayToast(StringResources.ConnectedToInternet, ToastTime.Short);
            else
                DialogService.DisplayToast(StringResources.MissingInternetConnection, ToastTime.OneHour);
        }

    }
}
