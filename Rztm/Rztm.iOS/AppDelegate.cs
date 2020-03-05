using Foundation;
using Plugin.DownloadManager;
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
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
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
}


