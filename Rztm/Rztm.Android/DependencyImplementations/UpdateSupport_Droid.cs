using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Rztm.DependencyInterfaces;
using Rztm.Droid.DependencyImplementations;
using Xamarin.Forms;

[assembly: Dependency(typeof(UpdateSupport_Droid))]
namespace Rztm.Droid.DependencyImplementations
{
    public class UpdateSupport_Droid : FileProvider, IUpdateSupport
    {
        private static readonly string providerPath = ".apkprovider";

        public bool CheckIfApkIsDownloaded()
            => File.Exists(AndroidConstants.UpdateInstallationPath);

        public void ApkInstall()
        {
            var apkUri = FileProvider.GetUriForFile(
               MainActivity.Instance,
               MainActivity.Instance.ApplicationContext.PackageName + providerPath,
               new Java.IO.File(AndroidConstants.UpdateInstallationPath));

            Intent install = new Intent(Intent.ActionView);
            install.AddFlags(ActivityFlags.GrantReadUriPermission);
            install.AddFlags(ActivityFlags.ClearTop);

            install.SetData(apkUri);
            MainActivity.Instance.StartActivity(install);

            //Todo: Add support for Android < N!
        }
    }
}