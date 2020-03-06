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
    public class AppUpdater
    {
        private static readonly string prefApkVersionBefore = "APP_VERSION_BEFORE_APK_INSTALL";
        private readonly IUpdateSupport _updateSupport = Xamarin.Forms.DependencyService.Get<IUpdateSupport>();

        //public void CheckLatestVersion(GithubRelease latestRelease)
        //{
        //    if (!latestRelease.IsCurrentAppVersionLatestRelease)
        //    {
        //        if (_updateSupport.CheckIfApkIsDownloaded())
        //            _updateSupport.ApkInstall();
        //        else
        //            DownloadLatestVersion(latestRelease.Assets[0].Url);
        //    }
        //}

        public void UpdateApp(GithubRelease latestRelease)
        {
            if (_updateSupport.CheckIfApkIsDownloaded())
                _updateSupport.ApkInstall();
            else
                DownloadLatestVersion(latestRelease.Assets[0].Url);
        }

        public bool CheckIsAppAfterUpdate()
        {
            var versionBeforeApkInstall = Xamarin.Essentials.Preferences.Get(prefApkVersionBefore, null);
            if (string.IsNullOrEmpty(versionBeforeApkInstall)
                || parseAppVersion(versionBeforeApkInstall) >= parseAppVersion(GetAppVersion()))
                return false;

            return true;
        }

        public void RemoveApkFile()
            => _updateSupport.RemoveApkFile();

        private void DownloadLatestVersion(string url)
        {
            var downloadManager = CrossDownloadManager.Current;
            var file = downloadManager.CreateDownloadFile(url);
            downloadManager.Start(file);
            downloadManager.CollectionChanged += async (s, e) =>
            {
                if (file.Status == DownloadFileStatus.COMPLETED)
                {
                    var fileSupport = Xamarin.Forms.DependencyService.Get<IUpdateSupport>();
                    fileSupport.ApkInstall();
                    Xamarin.Essentials.Preferences.Set(prefApkVersionBefore, GetAppVersion());
                }
            };
        }

        private string GetAppVersion()
            => (Xamarin.Forms.Application.Current as App).ApplicationVersion;

        private double parseAppVersion(string versionTag)
        {
            var numberString = versionTag.Remove(0, 1);
            var number = double.Parse(numberString);

            return number;
        }
    }
}
