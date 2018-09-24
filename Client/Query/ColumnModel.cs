using System;
using Newtonsoft.Json;

namespace ActivityInfo.Query
{
    public class ColumnModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("formula")]
        public string Formula { get; set; }

        public ColumnModel()
        {
        }

        public ColumnModel(string id, string formula)
        {
            this.Id = id;
            this.Formula = formula;
        }
    }
}
