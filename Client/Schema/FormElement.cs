using System;
using Newtonsoft.Json;

namespace ActivityInfo.Schema
{
    [JsonConverter(typeof(FormElementJsonConverter))]
    public class FormElement
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public FormElement()
        {
        }
    }
}
