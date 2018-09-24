using System;
using Newtonsoft.Json;

namespace ActivityInfo
{
    public class RecordRefJsonConverter : JsonConverter
    {
        public RecordRefJsonConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(RecordRef));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if(reader.TokenType != JsonToken.String) {
                throw new JsonException(string.Format(string.Format("Expected Record ref as a string, found {0}", reader.TokenType)));
            }                
            var s = reader.ReadAsString();
            var delimiter = s.IndexOf(':');
            if(delimiter == -1) {
                throw new JsonException(string.Format("Expected Record ref in the format {{formId}}:{{recordId}}, found: '{0}'", s));
            }

            return new RecordRef(s.Substring(0, delimiter), s.Substring(delimiter + 1));
        }
       
    }
}
