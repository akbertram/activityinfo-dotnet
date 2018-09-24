using System;
using Newtonsoft.Json;

namespace ActivityInfo
{
    public class RecordUpdateJsonConverter : JsonConverter
    {
        public RecordUpdateJsonConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(RecordUpdate));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RecordUpdate update = (RecordUpdate)value;

            writer.WriteStartObject();
            writer.WritePropertyName("formId");
            writer.WriteValue(update.Record.FormId);
            writer.WritePropertyName("recordId");
            writer.WriteValue(update.Record.RecordId);

            writer.WritePropertyName("fields");
            writer.WriteStartObject();

            update.Record.WriteFields(writer, serializer);

            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}
