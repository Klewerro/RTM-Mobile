using Rztm.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rztm.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabsPage : TabbedPage
    {
        public TabsPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<string, string>(string.Empty, Constants.DroidAppShortcutInvoked, DroidAppShortcutHandler);
        }

        private void DroidAppShortcutHandler(string sender, string intentData)
        {
            switch (intentData)
            {
                case "fovorites":
                    CurrentPage = Children[1];
                    break;
                case "nearby":
                    CurrentPage = Children[2];
                    break;
            }
        }
    }
}