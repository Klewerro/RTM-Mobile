# RzTM-Mobile
Xamarin.Forms Android application for comfortable reading RTM bus tables in Rzeszow.


## Application description
The idea for the application came from the lack of an official application for displaying data from bus stopboards, which would allow a comfortable view of bus departure times on bus stopboards.
The application has four main views:
* List of bus stops - data downloading from RTM API
* Favorites - stops marked as favorites are marked using yellow bar on the lists
* Nearby - stops at a certain distance from the user
* Bus stop information - list of the next 10 departures, current distance from the user, information / message (which is in reality displayed at the very bottom of the board)
In addition, the user from the application level has the option of open the official RTM website or clear the stop database. From the bus stop information level, you can change its name to a more intuitive one, or start navigation to it in a standard application.

<p align="center">
  <img src="/Files/screenshots/busStopsList.png" width="275" />
  <img src="/Files/screenshots/favorites.png" width="275" /> 
  <img src="/Files/screenshots/nearby.png" width="275" />
  <img align="top" src="/Files/screenshots/details.png" width="275" />
  <img src="/Files/screenshots/busStopsList-ios.png" width="275" />
  <img src="/Files/screenshots/favorites-ios.png" width="275" /> 
  <img src="/Files/screenshots/nearby-ios.png" width="275" />
  <img src="/Files/screenshots/details-ios.png" width="275" />
</p>


## Technologies and libraries used:
* Xamarin.Forms
* Prism.Forms and Unity
* Xml.Linq
* Sqlite-net-pcl
* Xamarin.Essentials and Xam.Plugin.Geolocator
* Newtonsoft.Json
* Acr.UserDialogs

## Installation guide (Android only):
1) Download .APK file from releases tab and copy to your phone.
2) If you dont have allow installation of non-official apps in settings enabled - enable it.
3) Find .APK file using phone files explorer and run it.

<br><br>
*The application I made only downloads data from the RTM website and displays it in a different way. In case of any conditions regarding my use of this data, please submit an issue for further contact.*
