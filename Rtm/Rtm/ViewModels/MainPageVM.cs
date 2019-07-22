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
        private string _test = "Initial Text";
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

        public string Test
        {
            get => _test;
            set
            {
                _test = value;
                OnPropertyChanged("Test");
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


    }
}
