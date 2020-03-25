using Moq;
using NUnit.Framework;
using Plugin.DownloadManager.Abstractions;
using Plugin.Geolocator.Abstractions;
using Prism.Navigation;
using Prism.Services;
using Rztm.DependencyInterfaces;
using Rztm.Helpers;
using Rztm.Models;
using Rztm.Repositories;
using Rztm.Services;
using Rztm.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Xamarin.Forms;

namespace Rztm.UnitTests
{
    [TestFixture]
    public class AppUpdaterTests
    {
        private TabsPageVM _tabsPageVM;
        private Mock<IGithubService> _githubServiceMock;
        private Mock<IAppUpdater> _appUpdaterMock;
        private Mock<IPageDialogService> _pageDialogServiceMock;
        private Mock<IAppPropertyService> _appPropertyServiceMock;

        private GithubRelease _githubRelease;
        private readonly string _newerTag = "v1.2";
        private readonly string _olderTag = "v0.9";
        private readonly string _presentTag = "v1.0";


        [SetUp]
        public void SetUp()
        {
            var navigationServiceMock = new Mock<INavigationService>();
            _pageDialogServiceMock = new Mock<IPageDialogService>();
            var busStopRepositoryMock = new Mock<IBusStopRepository>();
            _githubServiceMock = new Mock<IGithubService>();
            _appUpdaterMock = new Mock<IAppUpdater>();
            _appPropertyServiceMock = new Mock<IAppPropertyService>();
            var geolocatorMock = new Mock<IGeolocator>();

            _tabsPageVM = new TabsPageVM(navigationServiceMock.Object, _pageDialogServiceMock.Object,
                _githubServiceMock.Object, busStopRepositoryMock.Object, _appUpdaterMock.Object, 
                _appPropertyServiceMock.Object, geolocatorMock.Object);

            _githubRelease = new GithubRelease
            {
                Name = "Newer tRelease",
                TagName = null,
                Assets = new List<Asset>
                {
                    new Asset
                    {
                        Id = 1,
                        Name = "TestAsset",
                        Size = 2,
                        Url = "www.update-download-test.com"
                    }
                }
            };
        }

#region VM TESTS
        [Test]
        public void ViewModelUpdateApp_WhenNewerVersionAvailable_CallAppUpdaterUpdateApp()
        {
            _githubRelease.TagName = _newerTag;
            _githubServiceMock.Setup(gs => gs.GetLatestVersionCodeAsync()).Returns(Task.FromResult(_githubRelease));
            _pageDialogServiceMock.Setup(pds => pds.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            _appPropertyServiceMock.Setup(ap => ap.GetCurrentAppVersion()).Returns("1.2");
            _appUpdaterMock.Setup(au => au.UpdateApp(_githubRelease)).Verifiable();
            _tabsPageVM.CheckForUpdatesCommand.Execute(null);

            _appUpdaterMock.Verify(au => au.UpdateApp(_githubRelease), Times.Once);
        }

        [Test]
        public void ViewModelUpdateApp_WhenPresentGithubVersion_NoCallAppUpdaterUpdateApp()
        {
            _githubRelease.TagName = _presentTag;
            _githubServiceMock.Setup(gs => gs.GetLatestVersionCodeAsync()).Returns(Task.FromResult(_githubRelease));
            _pageDialogServiceMock.Setup(pds => pds.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            _appPropertyServiceMock.Setup(ap => ap.GetCurrentAppVersion()).Returns(_presentTag);
            _appUpdaterMock.Setup(au => au.UpdateApp(_githubRelease)).Verifiable();
            _tabsPageVM.CheckForUpdatesCommand.Execute(null);

            _appUpdaterMock.Verify(au => au.UpdateApp(_githubRelease), Times.Never);
        }

        [Test]
        public void ViewModelUpdateApp_WhenPresentGithubVersionAndAppAfterUpdate_AskUserForDeleteApk_IfTrueDeleteApk()
        {
            _githubRelease.TagName = _olderTag;
            _githubServiceMock.Setup(gs => gs.GetLatestVersionCodeAsync()).Returns(Task.FromResult(_githubRelease));
            _pageDialogServiceMock.Setup(pds => pds.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true)).Verifiable();
            _appPropertyServiceMock.Setup(ap => ap.GetCurrentAppVersion()).Returns(_presentTag);
            _appUpdaterMock.Setup(au => au.RemoveApkFile()).Verifiable();

            _appUpdaterMock.Setup(au => au.CheckIsAppAfterUpdate()).Returns(true);

            _tabsPageVM.CheckForUpdatesCommand.Execute(null);

            _pageDialogServiceMock.Verify(pds => pds.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _appUpdaterMock.Verify(au => au.RemoveApkFile(), Times.Once);
        }

        [Test]
        public void ViewModelUpdateApp_WhenPresentGithubVersionAndAppNotAfterUpdate_DontAskUserAboutRemoveApk()
        {
            _githubRelease.TagName = _olderTag;
            _githubServiceMock.Setup(gs => gs.GetLatestVersionCodeAsync()).Returns(Task.FromResult(_githubRelease));
            _pageDialogServiceMock.Setup(pds => pds.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true)).Verifiable();
            _appPropertyServiceMock.Setup(ap => ap.GetCurrentAppVersion()).Returns(_presentTag);
            _appUpdaterMock.Setup(au => au.RemoveApkFile()).Verifiable();

            _appUpdaterMock.Setup(au => au.CheckIsAppAfterUpdate()).Returns(false);

            _tabsPageVM.CheckForUpdatesCommand.Execute(null);

            _pageDialogServiceMock.Verify(pds => pds.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _appUpdaterMock.Verify(au => au.RemoveApkFile(), Times.Never);
        }
        #endregion

#region APP UPDATER TESTS
        [Test]
        public void DownloadLatestVersion_WhenApkIsNotDownloaded_CallDownloadUpdateApp()
        {
            _githubRelease.TagName = _newerTag;
            var updateSupportMock = new Mock<IUpdateSupport>();
            var downloadManagerMock = new Mock<IDownloadManager>();

            updateSupportMock.Setup(us => us.CheckIfApkIsDownloaded()).Returns(false);

            downloadManagerMock.Setup(dm => dm.Start(It.IsAny<IDownloadFile>(), true)).Verifiable();
            downloadManagerMock.SetupAdd(dm => dm.CollectionChanged += It.IsAny<NotifyCollectionChangedEventHandler>())
                .Verifiable();


            AppUpdater testAppUpdater = new AppUpdater(updateSupportMock.Object, downloadManagerMock.Object);
            testAppUpdater.UpdateApp(_githubRelease);


            downloadManagerMock.Verify(dm => dm.Start(It.IsAny<IDownloadFile>(), true));
            downloadManagerMock.VerifyAdd(dm => dm.CollectionChanged += It.IsAny<NotifyCollectionChangedEventHandler>());

            downloadManagerMock.SetupRemove(dm => dm.CollectionChanged -= It.IsAny<NotifyCollectionChangedEventHandler>());
        }

        [Test]
        public void DownloadLatestVersion_WhenApkIsDownloaded_CallApkInstall()
        {
            _githubRelease.TagName = _newerTag;

            var updateSupportMock = new Mock<IUpdateSupport>();
            var downloadManagerMock = new Mock<IDownloadManager>();

            updateSupportMock.Setup(us => us.CheckIfApkIsDownloaded()).Returns(true);
            updateSupportMock.Setup(us => us.ApkInstall()).Verifiable();

            downloadManagerMock.Setup(dm => dm.Start(It.IsAny<IDownloadFile>(), true)).Verifiable();


            AppUpdater testAppUpdater = new AppUpdater(updateSupportMock.Object, downloadManagerMock.Object);
            testAppUpdater.UpdateApp(_githubRelease);

            updateSupportMock.Verify(us => us.ApkInstall());
            downloadManagerMock.Verify(dm => dm.Start(It.IsAny<IDownloadFile>(), true), Times.Never);
        }



#endregion

    }
}
