using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Prism.Ioc;
using Prism;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using Android.Content;
using Rztm.DependencyInterfaces;
using Rztm.Droid.DependencyImplementations;

namespace Rztm.Droid
{
    [Activity(Label = "RzTM", Icon = "@mipmap/icon", Theme = "@style/splashscreen", MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop, 
        Name = "com.cleversoft.MainActivity")] //Name set for shortcuts
    [MetaData("android.app.shortcuts", Resource = "@xml/shortcuts")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Xamarin.Forms.Platform.Android.FormsAppCompatActivity Instance { get; private set; }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.Window.RequestFeature(WindowFeatures.ActionBar);
            // Name of the MainActivity theme you had there before.
            // Or you can use global::Android.Resource.Style.ThemeHoloLight
            base.SetTheme(Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            CreateFilePathsForDownloadManager();

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Essentials.ExperimentalFeatures.Enable("OpenFileRequest_Experimental");
            Xamarin.Essentials.ExperimentalFeatures.Enable("ShareFileRequest_Experimental");
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Acr.UserDialogs.UserDialogs.Init(this);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState); //for geolocator plugin

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new AndroidInitializer(), Intent?.Data?.LastPathSegment));

            Instance = this;
            Xamarin.Forms.MessagingCenter.Subscribe<string, (int, string)>(string.Empty,
                Rztm.Helpers.Constants.CreateBusStopShortcut, CreateBusStopAppShortcut);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }


        private void CreateFilePathsForDownloadManager()
        {
            (CrossDownloadManager.Current as DownloadManagerImplementation).NotificationVisibility = DownloadVisibility.VisibleNotifyCompleted;

            CrossDownloadManager.Current.PathNameForDownloadedFile = new System.Func<IDownloadFile, string>(file => 
                ConstantsAndroid.UpdateInstallationPath);
        }

        private void CreateBusStopAppShortcut(object sender, (int id, string name) busStopTuple)
        {
            var shortcutBuilder = new ShortcutInfo.Builder(this, $"busStopShort_{busStopTuple.id}");
            shortcutBuilder.SetIcon(Android.Graphics.Drawables.Icon.CreateWithResource(this, Resource.Drawable.ic_shortcut_bus_stop));
            shortcutBuilder.SetShortLabel(busStopTuple.name);
            shortcutBuilder.SetLongLabel(busStopTuple.name);
            shortcutBuilder.SetIntent(new Intent(Intent.ActionView,
                Android.Net.Uri.Parse($"content:////actions//busStopShort_{busStopTuple.id}"),
                this,
                typeof(MainActivity)));

            ShortcutInfo busStopShortcut = shortcutBuilder.Build();
            ShortcutManager shortcutManager = (ShortcutManager)this.GetSystemService(Java.Lang.Class.FromType(typeof(ShortcutManager)));
            shortcutManager.SetDynamicShortcuts(new System.Collections.Generic.List<ShortcutInfo>() { busStopShortcut });
        }
    }


    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.Register<IUpdateSupport, UpdateSupport_Droid>();
            containerRegistry.Register<ISQLiteDatabase, SQLiteDatabase_Droid>();
        }
    }
}