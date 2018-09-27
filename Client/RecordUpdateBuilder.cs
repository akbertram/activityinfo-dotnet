using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActivityInfo
{
    [JsonConverter(typeof(RecordChangeJsonConverter))]
    public class RecordUpdateBuilder : IChange
    {
        private string formId;
        private string recordId;
        private string parentRecordId;

        private JObject fields = new JObject();

        public RecordUpdateBuilder()
        {
        }

        public void SetFormId(string formId)
        {
            this.formId = formId;
        }

        public void SetRecordId(string recordId) 
        {
            this.recordId = recordId;
        }

        public void GenerateNewId() 
        {
            var random = new Random();
            this.recordId = String.Format("s{0:D10}", random.Next());
        }

        public void SetIndicatorValue(int indicatorId, double value) 
        {
            SetFieldValue(String.Format("i{0:D10}", indicatorId), value);
        }

        public void SetFieldValue(string fieldId, string value) 
        {
            this.fields[fieldId] = value;
        }

        public void SetFieldValue(string fieldId, double value)
        {
            this.fields[fieldId] = value;
        }

        public void SetFieldValue(string fieldId, int value)
        {
            this.fields[fieldId] = value;
        }

        public void SetFieldValue(string fieldId, GeoPoint value)
        {
            this.fields[fieldId] = JObject.FromObject(value);
        }

        public void SetFieldValue(string fieldId, DateTime date)
        {
            this.fields[fieldId] = date.ToString("yyyy-MM-dd");
        }

        public void SetFieldValue(string fieldId, RecordRef recordRef)
        {
            this.fields[fieldId] = recordRef.ToString();
        }

        public JObject toJson() 
        {
            JObject jsonObject = new JObject();
            jsonObject["formId"] = formId;
            jsonObject["recordId"] = recordId;
            if(parentRecordId != null) {
                jsonObject["parentRecordId"] = parentRecordId;
            }
            jsonObject["fields"] = fields;
            return jsonObject;
        }
    }
}
