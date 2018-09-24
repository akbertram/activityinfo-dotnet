using System;
using Newtonsoft.Json;

namespace ActivityInfo.Query
{
    public class RowSource
    {
        [JsonProperty("rootFormId")]
        public string RootFormId { get; set; }

        public RowSource(string rootFormId)
        {
            this.RootFormId = rootFormId;
        }
    }
}
