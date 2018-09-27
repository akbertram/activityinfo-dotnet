using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActivityInfo.Query
{
    public class Row
    {
        private ColumnSet columnSet;
        private int rowIndex;

        public Row()
        {
        }

        public Row(ColumnSet columnSet, int rowIndex)
        {
            this.columnSet = columnSet;
            this.rowIndex = rowIndex;
        }

        public object this[string columnId]
        {
            get { return columnSet[columnId][rowIndex]; }
        }

        public string GetString(string columnId) {
            return columnSet[columnId].GetString(rowIndex);
        }

        public double GetQuantity(string columnId) {
            return columnSet[columnId].GetDouble(rowIndex);
        }


    }
}
