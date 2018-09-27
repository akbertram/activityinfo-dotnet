using System;
using Newtonsoft.Json;

namespace ActivityInfo
{
    [JsonConverter(typeof(RecordUpdateJsonConverter))]
    public class RecordUpdate : IChange
    {
        private IRecord record;
            
        public RecordUpdate(IRecord record)
        {
            this.record = record;
        }

        public IRecord Record
        {
            get { return record; }
        }
    }
}
