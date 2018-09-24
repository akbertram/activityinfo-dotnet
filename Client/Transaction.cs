using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityInfo
{
    /// <summary>
    /// A set of changes to a Form's Records.
    /// </summary>
    public class Transaction
    {
        private List<IChange> _changes;

        public Transaction()
        {
            _changes = new List<IChange>();
        }

        [JsonProperty("changes")]
        public List<IChange> Changes {
            get {
                return _changes;
            }
        }

        public void AddChange(IChange change) {
            _changes.Add(change);
        }
    }
}
