using System;
using System.Collections.Generic;
using System.Reflection;

namespace ActivityInfo.Query
{

    public class QueryMapper<T> where T : new()
    {
        private QueryModel query;
        private List<IPropertyMapper> mappers = new List<IPropertyMapper>();

        public QueryMapper(string formId) {
            
            query = new QueryModel(formId);
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.CanWrite)
                {
                    IPropertyMapper mapper = mapperForProperty(property, formId);
                    mapper.AddToQuery(query);
                    mappers.Add(mapper);
                }
            }
        }

        public QueryModel QueryModel {
            get {
                return query;
            }
        }

        public List<T> Map(ColumnSet columnSet) {
            foreach(IPropertyMapper mapper in mappers) {
                mapper.Start(columnSet);
            }
            List<T> list = new List<T>();
            for (int i = 0; i < columnSet.RowCount;++i) {
                T row = new T();
                foreach(IPropertyMapper mapper in mappers) {
                    mapper.ReadProperty(row, i);    
                }
                list.Add(row);
            }
            return list;
        }

        private IPropertyMapper mapperForProperty(PropertyInfo property, string formId)
        {
            if (property.GetCustomAttribute(typeof(RecordId)) != null)
            {
                if (property.PropertyType.Equals(typeof(string)))
                {
                    return new SimpleMapper(property, "_id");
                } else if(property.PropertyType.Equals(typeof(RecordRef))) {
                    return new RecordIdParser(property, formId);
                }
            }

            if (property.Name.Equals("FormId") && property.DeclaringType == typeof(BaseRecord))
            {
                return new FormIdParser(property, formId);
            }

            var formulaAttribute = property.GetCustomAttribute<Formula>();
            string formula;
            if (formulaAttribute != null)
            {
                formula = formulaAttribute.Value;
            }
            else
            {
                formula = property.Name;
            }

            if (property.PropertyType == typeof(GeoPoint))
            {
                return new GeoPointMapper(property, formula);
            }
            else if(property.PropertyType == typeof(RecordRef)) {
                return new RefMapper(property, formula);
            }
            else
            {
                return new SimpleMapper(property, formula);
            }
        }

        private interface IPropertyMapper
        {
            void Start(ColumnSet columnSet);
            void ReadProperty(object instance, int rowIndex);
            void AddToQuery(QueryModel query);
        }

        private interface IConverter 
        {
            object convert(IColumnView column, int rowIndex);
        }

        private class StringConverter : IConverter
        {
            public object convert(IColumnView column, int rowIndex)
            {
                return column.GetString(rowIndex);
            }
        }

        private class DoubleConverter : IConverter
        {
            public object convert(IColumnView column, int rowIndex)
            {
                return column.GetDouble(rowIndex);
            }
        }

        private class IntConverter : IConverter
        {
            public object convert(IColumnView column, int rowIndex)
            {
                double value = column.GetDouble(rowIndex);
                if(Double.IsNaN(value)) {
                    return 0;
                } else {
                    return (int)value;
                }
            }
        }

        private class EnumConverter : IConverter {
            private Type type;

            public EnumConverter(Type type)
            {
                this.type = type;
            }

            public object convert(IColumnView column, int rowIndex) {
                return null;
            }
        }

        private static IConverter converterForType(PropertyInfo property) {
            Type type = property.PropertyType;
            if(type.Equals(typeof(int))) {
                return new IntConverter();
            } else if(type.Equals(typeof(string))) {
                return new StringConverter();
            } else if(type.Equals(typeof(double))) {
                return new DoubleConverter();
            } else if(type.IsEnum) {
                return new EnumConverter(type);
            } else {
                throw new ActivityInfoException(String.Format("Invalid type '{0}' for property '{1}' in {2}",
                                                              type.FullName,
                                                              property.Name,
                                                              property.DeclaringType.FullName));
            }
        }

        private class SimpleMapper : IPropertyMapper
        {
            private PropertyInfo property;
            private string formula;
            private IColumnView columnView;
            private IConverter converter;

            public SimpleMapper(PropertyInfo property, string formula)
            {
                this.property = property;
                this.formula = formula;
                this.converter = converterForType(property);
            }

            public void AddToQuery(QueryModel query)
            {
                query.AddFormula(property.Name, formula);
            }

            public void Start(ColumnSet columnSet)
            {
                columnView = columnSet[property.Name];
            }

            public void ReadProperty(object instance, int rowIndex)
            {
                property.SetValue(instance, converter.convert(columnView, rowIndex));
            }

        }


        private class RecordIdParser : IPropertyMapper
        {
            private PropertyInfo property;
            private IColumnView columnView;
            private string formId;

            public RecordIdParser(PropertyInfo property, string formId)
            {
                this.property = property;
                this.formId = formId;
            }

            public void AddToQuery(QueryModel query)
            {
                query.AddRecordId(property.Name);
            }

            public void Start(ColumnSet columnSet)
            {
                columnView = columnSet[property.Name];
            }

            public void ReadProperty(object instance, int rowIndex)
            {
                var recordId = columnView.GetString(rowIndex);
                var recordRef = new RecordRef(formId, recordId);
                property.SetValue(instance, recordRef);
            }
        }

        private class FormIdParser : IPropertyMapper
        {
            private PropertyInfo property;
            private string formId;

            public FormIdParser(PropertyInfo property, string formId)
            {
                this.property = property;
                this.formId = formId;
            }

            public void AddToQuery(QueryModel query)
            {
            }

            public void Start(ColumnSet columnSet)
            {
            }

            public void ReadProperty(object instance, int rowIndex)
            {
                property.SetValue(instance, formId);
            }

        }


        private class RefMapper : IPropertyMapper
        {
            private PropertyInfo property;
            private string formula;
            private string recordColumnId;
            private string formColumnId;
            private IColumnView recordColumn;
            private IColumnView formColumn;

            public RefMapper(PropertyInfo property, string formula)
            {
                this.property = property;
                this.formula = formula;
                this.recordColumnId = string.Format("{0}", property.Name);
                this.formColumnId = string.Format("{0}$form", property.Name);

            }

            public void AddToQuery(QueryModel query)
            {
                query.AddFormula(recordColumnId, string.Format("{0}", formula));
                query.AddFormula(formColumnId, string.Format("{0}._class", formula));
            }

            public void Start(ColumnSet columnSet)
            {
                recordColumn = columnSet[recordColumnId];
                formColumn = columnSet[formColumnId];
            }

            public void ReadProperty(object instance, int rowIndex)
            {
                string recordId = recordColumn.GetString(rowIndex);
                string formId = formColumn.GetString(rowIndex);

                if(recordId != null) {
                    property.SetValue(instance, new RecordRef(formId, recordId));
                }
            }
        }

        private class GeoPointMapper : IPropertyMapper
        {
            private PropertyInfo property;
            private string formula;
            private string latitudeColumnId;
            private string longitudeColumnId;
            private IColumnView latitudeColumn;
            private IColumnView longitudeColumn;

            public GeoPointMapper(PropertyInfo property, string formula)
            {
                this.property = property;
                this.formula = formula;
                this.latitudeColumnId = string.Format("{0}.latitude", property.Name);
                this.longitudeColumnId = string.Format("{0}.longitude", property.Name);

            }

            public void AddToQuery(QueryModel query)
            {
                query.AddFormula(latitudeColumnId, String.Format("{0}.latitude", formula));
                query.AddFormula(longitudeColumnId, String.Format("{0}.longitude", formula));
            }


            public void Start(ColumnSet columnSet)
            {
                this.latitudeColumn = columnSet[latitudeColumnId];
                this.longitudeColumn = columnSet[longitudeColumnId];
            }

            public void ReadProperty(object instance, int rowIndex)
            {
                double lat = latitudeColumn.GetDouble(rowIndex);
                double lng = longitudeColumn.GetDouble(rowIndex);
                if(!Double.IsNaN(lat) && !Double.IsNaN(lng)) {
                    property.SetValue(instance, new GeoPoint(lat, lng));
                }
            }
        }
    }
}
