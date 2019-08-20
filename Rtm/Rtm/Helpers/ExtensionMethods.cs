using Plugin.Geolocator.Abstractions;
using Rtm.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rtm.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Adding downloaded using RTM API departures and description to busStop downloaded from database.
        /// </summary>
        /// <param name="busStop">BusStop from database, which dont have departures and description</param>
        /// <param name="downloadedBusStop">BusStop downloaded from API, which have all busStop data</param>
        public static void AddDownloadedData(this BusStop busStop, BusStop downloadedBusStop)
        {
            busStop.Description = downloadedBusStop.Description;
            busStop.Departures = downloadedBusStop.Departures;
        }

        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                observableCollection.Add(item);
            }
        }

        public static Position ConvertBusStopToPositon(this BusStop busStop)
            => new Position(busStop.Latitude, busStop.Longitude);
    }
}
