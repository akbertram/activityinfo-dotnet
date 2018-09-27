using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActivityInfo.Query
{
    public class RowJsonConverter : JsonConverter
    {
        public RowJsonConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Row));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Row(JToken.ReadFrom(reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
