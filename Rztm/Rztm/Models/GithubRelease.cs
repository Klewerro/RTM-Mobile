using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rztm.Models
{
    public class GithubRelease
    {
        public int Id { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        public string Name { get; set; }

        [JsonProperty("published_at")]
        public DateTime ReleaseDate { get; set; }

        public List<Asset> Assets { get; set; }


        public bool IsLatestRelease()
        {
            var currentVersion = (Xamarin.Forms.Application.Current as App).ApplicationVersion;
            var currentVersionNumber = parseVersion(currentVersion);
            var latestVersionNumber = parseVersion("v1.1");

            if (latestVersionNumber > currentVersionNumber)
                return false;

            return true;
        }


        private double parseVersion(string versionTag)
        {
            var numberString = versionTag.Remove(0, 1);
            var number = double.Parse(numberString);

            return number;
        }
    }
}
