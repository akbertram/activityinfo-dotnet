using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityInfo.Query
{
    [JsonConverter(typeof(ColumnSetDeserializer))]
    public class ColumnSet
    {
        private int rowCount;
        private Dictionary<string, IColumnView> columns;

        public ColumnSet() {
        }

        public ColumnSet(int rowCount, Dictionary<string, IColumnView> columns)
        {
            this.rowCount = rowCount;
            this.columns = columns;
        }

        public int RowCount {
            get {
                return rowCount;
            }
        }

        public IColumnView this[string columnId]
        {
            get { return columns[columnId]; }
        }
    }
}
