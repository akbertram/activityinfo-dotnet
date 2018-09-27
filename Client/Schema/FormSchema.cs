using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityInfo.Schema
{
    public class FormSchema
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("schemaVersion")]
        public long SchemaVersion { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("elements")]
        public List<FormElement> Elements { get; set; }

        public FormSchema()
        {
        }
    }
}
