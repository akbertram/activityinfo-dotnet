using System;
using Newtonsoft.Json;

namespace ActivityInfo
{

    public enum ResourceType {
        Folder,
        Form,
        Database
    }

    public class Resource
    {
        [JsonProperty("id")]
        public string Id { get; set;  }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("type")]
        public ResourceType Type;

        [JsonProperty("label")]
        public string Label;


        public Resource()
        {
        }
    }
}
