using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using Rztm.DependencyInterfaces;
using Rztm.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Rztm.Helpers
{
    public class AppUpdater : IAppUpdater
    {
        //private static readonly string prefApkVersionBefore = "APP_VERSION_BEFORE_APK_INSTALL";
        private readonly IUpdateSupport _updateSupport;
        private readonly IDownloadManager _downloadManager;

        public AppUpdater(IUpdateSupport updateSupport, IDownloadManager downloadManager)
        {
            _updateSupport = updateSupport;
            _downloadManager = downloadManager;
        }


        public string GetCurrentVersion() => VersionTracking.CurrentVersion;

        public void UpdateApp(GithubRelease latestRelease)
        {
            if (_updateSupport.CheckIfApkIsDownloaded())
                _updateSupport.ApkInstall();
            else
                DownloadLatestVersion(latestRelease.Assets[0].Url);
        }

        public bool CheckIsAppAfterUpdate()
        {
            var prevVersion = VersionTracking.PreviousVersion;

            if (string.IsNullOrEmpty(prevVersion)
                || parseAppVersion(prevVersion) >= parseAppVersion(GetCurrentVersion()))
                return false;

            return true;
        }

        public void RemoveApkFile()
            => _updateSupport.RemoveApkFile();

        public void DownloadLatestVersion(string url)
        {
            var file = _downloadManager.CreateDownloadFile(url);
            _downloadManager.Start(file);
            _downloadManager.CollectionChanged += async (s, e) =>
            {
                if (file.Status == DownloadFileStatus.COMPLETED)
                {
                    _updateSupport.ApkInstall();
                    //Xamarin.Essentials.Preferences.Set(prefApkVersionBefore, GetAppVersion());
                }
            };
        }

        private double parseAppVersion(string versionTag)
        {
            var numberString = versionTag.Remove(0, 1);
            var number = double.Parse(numberString);

            return number;
        }
    }
}
