using System;
namespace ActivityInfo.Query
{
    public class BaseQueryResult
    {
        public BaseQueryResult()
        {
        }

        public string RecordId { get; set; }
        public string FormId { get; set; }
        public RecordRef RecordRef {
            get
            {
                return new RecordRef(FormId, RecordId);
            }            
        }
    }
}
