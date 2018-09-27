using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActivityInfo
{
    public class RecordChangeJsonConverter : JsonConverter
    {
        public RecordChangeJsonConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(RecordUpdateBuilder));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RecordUpdateBuilder change = value as RecordUpdateBuilder;
            if(change == null) {
                writer.WriteNull();
            } else {
                change.toJson().WriteTo(writer);
            }
        }
    }
}
