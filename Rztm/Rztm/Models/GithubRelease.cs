using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

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

        [JsonProperty("body")]
        public string Description { get; set; }

        public List<Asset> Assets { get; set; }

        public bool IsCurrentAppVersionLatestRelease { get; private set; }


        public bool CheckIsLatestRelease(string currentAppVertion)
        {
            var currentVersionNumber = parseVersion(currentAppVertion);
            var latestVersionNumber = parseVersion(TagName);

            if (latestVersionNumber > currentVersionNumber)
            {
                IsCurrentAppVersionLatestRelease = false;
                return false;
            }


            IsCurrentAppVersionLatestRelease = true;
            return true;
        }

        private double parseVersion(string versionTag)
        {
            var numberString = versionTag.Remove(0, 1);
            var number = double.Parse(numberString, CultureInfo.InvariantCulture);

            return number;
        }
    }
}
