using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityInfo
{
    public class DatabaseMetadata
    {
        [JsonProperty("databaseId")]
        public string DatabaseId;

        [JsonProperty("userId")]
        public long UserId;

        [JsonProperty("label")]
        public string Label;

        [JsonProperty("owner")]
        public Boolean owner;

        [JsonProperty("version")]
        public string Version;

        [JsonProperty("resources")]
        public List<Resource> Resources;

        public DatabaseMetadata()
        {
        }
    }
}
