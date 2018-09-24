using System;
using Newtonsoft.Json;

namespace ActivityInfo
{
    [JsonConverter(typeof(RecordRefJsonConverter))]
    public class RecordRef
    {
        private readonly string formId;
        private readonly string recordId;

        public RecordRef(string formId, string recordId)
        {
            this.formId = formId;
            this.recordId = recordId;
        }

        public string FormId
        {
            get
            {
                return formId;
            }
        }

        public string RecordId
        {
            get
            {
                return recordId;
            }
        }

        public override bool Equals(object obj)
        {
            var that = obj as RecordRef;
            if(that == null) {
                return false;
            }
            return this.formId.Equals(that.formId) &&
                       this.recordId.Equals(that.recordId);
        }

        public override int GetHashCode()
        {
            return formId.GetHashCode();
        }    

        public override string ToString()
        {
            return string.Format("{0}:{1}", formId, recordId);
        }

    }
}
