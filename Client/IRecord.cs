using System;
using Newtonsoft.Json;

namespace ActivityInfo
{
    public interface IRecord
    {
        string FormId { get; }
        string RecordId { get; }

        void WriteFields(JsonWriter writer, JsonSerializer serializer);
    }
}
