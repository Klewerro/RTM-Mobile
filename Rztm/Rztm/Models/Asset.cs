using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rztm.Models
{
    public class Asset
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        [JsonProperty("browser_download_url")]
        public string Url { get; set; }
    }
}
