using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.Helpers
{
    public interface IAppPropertyService
    {
        string GetCurrentAppVersion();
    }

    public class AppPropertyService : IAppPropertyService
    {
        public string GetCurrentAppVersion()
            => (Xamarin.Forms.Application.Current as App).ApplicationVersion;
    }
}
