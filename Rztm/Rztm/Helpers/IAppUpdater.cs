using Rztm.Models;

namespace Rztm.Helpers
{
    public interface IAppUpdater
    {
        void UpdateApp(GithubRelease latestRelease);
        bool CheckIsAppAfterUpdate();
        void RemoveApkFile();
        void DownloadLatestVersion(string url);
    }
}
