using System;
using Newtonsoft.Json;

namespace ActivityInfo
{
    public class Record : IRecord
    {
        public Record()
        {
        }


        public string FormId => throw new NotImplementedException();

        public string RecordId => throw new NotImplementedException();

        public void WriteFields(JsonWriter writer, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
