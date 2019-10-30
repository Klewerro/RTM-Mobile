using Foundation;
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

    }
}


