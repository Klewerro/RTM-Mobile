using System.IO;

namespace Rztm.Droid
{
    public class AndroidConstants
    {
        public static readonly string DownloadDirectory = Android.OS.Environment
            .GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        public static readonly string UpdateInstallationPath = Path.Combine(DownloadDirectory, "RzTM-mobile.apk");
    }
}