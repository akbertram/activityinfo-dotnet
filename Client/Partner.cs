using System;
using ActivityInfo.Query;
using Newtonsoft.Json;

namespace ActivityInfo
{
    public class Partner
    {

        /// <summary>
        /// The Partner's Id
        /// </summary>
        /// <value>The identifier.</value>
        [RecordId]
        public RecordRef RecordRef { get; set; }

        /// <summary>
        /// The Partner's Human-readable Label (name)
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }

        public Partner()
        {
        }

		public override string ToString()
		{
            return String.Format("[Partner id={0}, label={1}]", RecordRef, Label);
		}
	}
}
