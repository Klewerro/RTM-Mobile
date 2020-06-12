using Foundation;
using Plugin.DownloadManager;
using Prism;
using Prism.Ioc;
using Rztm.DependencyInterfaces;
using Rztm.iOS.DependencyImplementations;
using System;
using UIKit;

namespace Rztm.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            //// create a new window instance based on the screen size
            //Window = new UIWindow(UIScreen.MainScreen.Bounds);
            //Window.RootViewController = new UIViewController();

            //// make the window visible
            //Window.MakeKeyAndVisible();
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new IosInitializer()));
            return base.FinishedLaunching(application, launchOptions);
        }

        /**
        * Save the completion-handler we get when the app opens from the background.
        * This method informs iOS that the app has finished all internal processing and can sleep again.
        */
        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
        {
            CrossDownloadManager.BackgroundSessionCompletionHandler = completionHandler;
        }

    }

    public class IosInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.Register<IUpdateSupport, UpdateSupport_iOS>();
            containerRegistry.Register<ISQLiteDatabase, SQLiteDatabase_iOS>();
        }
    }
}


