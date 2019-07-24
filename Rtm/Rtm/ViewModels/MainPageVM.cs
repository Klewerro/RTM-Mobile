using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rtm.Models;
using Rtm.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace Rtm.ViewModels
{
    public class MainPageVM : ViewModelBase
    {
        
        private BusStop busStop;
        private int _busStopEntryId;
        private RtmService _rtmService;

        public BusStop BusStop
        {
            get => busStop;
            set
            {
                busStop = value;
                OnPropertyChanged("BusStop");
            }
        }

        public int BusStopEntryId
        {
            get => _busStopEntryId;
            set
            {
                _busStopEntryId = value;
                OnPropertyChanged("BusStopEntryId");
            }
        }

        public MainPageVM()
        {
            _rtmService = new RtmService();
            RefreshCommand?.Execute(null);
        }

        public ICommand RefreshCommand => new Command(async () =>
        {
            IsBusy = true;
            BusStop = await _rtmService.GetBusStop();
            IsBusy = false;
        });

        public ICommand DownloadCommand => new Command(async () =>
        {
            IsBusy = true;
            BusStop = await _rtmService.GetBusStop(BusStopEntryId);
            IsBusy = false;
        });
    }
}
