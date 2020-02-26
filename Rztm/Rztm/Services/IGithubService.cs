using Rztm.Models;
using System.Threading.Tasks;

namespace Rztm.Services
{
    public interface IGithubService
    {
        Task<GithubRelease> GetLatestVersionCode();
    }
}