using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityInfo.Query
{
    public class QueryModel
    {
        private List<RowSource> _rowSources;
        private List<ColumnModel> _columns;

        public QueryModel(string rootFormId)
        {
            _rowSources = new List<RowSource>();
            _rowSources.Add(new RowSource(rootFormId));
            _columns = new List<ColumnModel>();
        }

        [JsonProperty("rowSources")]
        public List<RowSource> RowSources {
            get {
                return _rowSources;
            }
        }

        [JsonProperty("columns")]
        public List<ColumnModel> Columns {
            get {
                return _columns;
            }
        }

        public void AddRecordId(String columnId) {
            _columns.Add(new ColumnModel(columnId, "_id"));
        }

        public void AddField(String columnId, String fieldCodeOrId) {
            _columns.Add(new ColumnModel(columnId, fieldCodeOrId));
        }

        public void AddFormula(String columnId, String formula) {
            _columns.Add(new ColumnModel(columnId, formula));
        }
    }
}
