using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityInfo.Query
{
    public class ColumnSetDeserializer : JsonConverter
    {
        public ColumnSetDeserializer()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ColumnSet);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, IColumnView> columns = new Dictionary<string, IColumnView>();
       
            if(reader.TokenType != JsonToken.StartObject) {
                throw new Exception("Expected a JSON Object");
            }

            readCheck(reader, JsonToken.PropertyName, "rows");
            int rowCount = (int)reader.ReadAsInt32();
            readCheck(reader, JsonToken.PropertyName, "columns");
            readCheck(reader, JsonToken.StartObject);

            while(reader.Read()) {

                if(reader.TokenType == JsonToken.EndObject) {
                    break;
                } else if(reader.TokenType != JsonToken.PropertyName) {
                    throw new Exception("Expected column name");
                }

                string columnId = reader.Value as string;

                readCheck(reader, JsonToken.StartObject);
                readCheck(reader, JsonToken.PropertyName, "type");
                ColumnType type = parseColumnType(reader.ReadAsString());

                readCheck(reader, JsonToken.PropertyName, "storage");
                string storage = reader.ReadAsString();

                if(storage.Equals("empty")) {
                    columns.Add(columnId, new EmptyColumnView(type, rowCount));

                } else if(storage.Equals("array")) {
                    columns.Add(columnId, readArrayColumn(reader, type, rowCount));
                
                } else if(storage.Equals("constant")) {
                    columns.Add(columnId, readConstantColumn(reader, type, rowCount));

                } else {
                    throw new Exception(string.Format("Unsupported storage type '{0}'", storage));
                }

                readCheck(reader, JsonToken.EndObject);
            }
            readCheck(reader, JsonToken.EndObject);

            return new ColumnSet(rowCount, columns);
        }

        private ColumnType parseColumnType(string typeName)
        {
            if(typeName.ToLower().Equals("string")) {
                return ColumnType.String;
            } else if(typeName.ToLower().Equals("number")) {
                return ColumnType.Number;
            } else if(typeName.ToLower().Equals("boolean")) {
                return ColumnType.Boolean;
            } else {
                throw new Exception(string.Format("Unrecognized column type '{0}'", typeName));
            }
        }

        private IColumnView readArrayColumn(JsonReader reader, ColumnType type, int rowCount)
        {
            readCheck(reader, JsonToken.PropertyName, "values");
            readCheck(reader, JsonToken.StartArray);
            switch (type)
            {
                case ColumnType.String:
                    {
                        string[] values = new string[rowCount];
                        int i = 0;
                        while (i < rowCount)
                        {
                            values[i++] = reader.ReadAsString();
                        }
                        readCheck(reader, JsonToken.EndArray);

                        return new StringArrayColumnView(values);
                    }
                case ColumnType.Number:
                    {
                        double[] values = new double[rowCount];
                        int i = 0;
                        while (i < rowCount)
                        {
                            reader.Read();
                            if(reader.TokenType == JsonToken.Null) {
                                values[i++] = double.NaN;
                            } else {
                                values[i++] = (double)reader.Value;
                            }
                        }
                        readCheck(reader, JsonToken.EndArray);

                        return new DoubleArrayColumnView(values);
                    }
                case ColumnType.Boolean:
                    {
                        byte[] values = new byte[rowCount];
                        int i = 0;
                        while (i < rowCount)
                        {
                            reader.Read();
                            if(reader.TokenType == JsonToken.Null) {
                                values[i++] = BooleanArrayColumnView.MISSING;
                            } else {
                                values[i++] = (byte)(((bool)reader.Value) ? 1 : 0);
                            }
                        }
                        readCheck(reader, JsonToken.EndArray);

                        return new BooleanArrayColumnView(values);
                    }
            }
            throw new Exception(string.Format("Unsupported type '{0}'", type));
        }

        private IColumnView readConstantColumn(JsonReader reader, ColumnType type, int rowCount) {
            object value;

            readCheck(reader, JsonToken.PropertyName, "value");
            reader.Read();

            if(reader.TokenType == JsonToken.Null) {
                value = null;
            } else {
                value = reader.Value;
            }

            return new ConstantColumnView(type, value, rowCount);
        }


        private static void readCheck(JsonReader reader, JsonToken expectedType) {
            reader.Read();
            if(reader.TokenType != expectedType) {
                throw new Exception(string.Format("Expected token {0}, found: {1}", expectedType, reader.TokenType));
            }
        }
        private static void readCheck(JsonReader reader, JsonToken expectedType, string expectedValue) {
            reader.Read();
            if (reader.TokenType != expectedType)
            {
                throw new Exception(string.Format("Expected token {0}, found: {1}", expectedType, reader.TokenType));
            }
            var s = reader.Value as string;         
            if(!expectedValue.Equals(s)) {
                throw new Exception(string.Format("Expected token {0} [{1}], found '{2}'", expectedValue, expectedType, s));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

