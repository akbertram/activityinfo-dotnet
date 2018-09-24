using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActivityInfo
{
    /// <summary>
    /// Base class for Records
    /// </summary>
    public abstract class BaseRecord : IRecord
    {
        private string formId;

        public BaseRecord(string formId)
        {
            this.formId = formId;
        }

        public string RecordId { get; set; }

        public string FormId
        {
            get
            {
                return formId;
            }
        }

        public RecordRef Ref
        {
            get
            {
                return new RecordRef(FormId, RecordId);
            }
        }

        public void ReadFields(JObject jsonObject) {
            var fields = jsonObject["fields"] as JObject;
            if (fields != null)
            {
                foreach (PropertyInfo property in this.GetType().GetProperties())
                {
                    if (property.DeclaringType != typeof(BaseRecord))
                    {
                        var fieldValue = fields.GetValue(property.Name);
                        if (fieldValue != null)
                        {
                            property.SetValue(this, fieldValue.ToObject(property.PropertyType));
                        }
                    }
                }
            }
        }

        public void WriteFields(JsonWriter writer, JsonSerializer serializer)
        {
            foreach(PropertyInfo property in this.GetType().GetProperties()) {
                if (property.DeclaringType != typeof(BaseRecord))
                {
                    writer.WritePropertyName(property.Name);

                    object value = property.GetValue(this);
                    if (value == null)
                    {
                        writer.WriteNull();
                    }
                    else if (value is Enum)
                    {
                        var enumItem = value as Enum;
                        var enumType = value.GetType();
                        var member = enumType.GetMember(enumItem.ToString());
                        var enumId = member[0].GetCustomAttribute<EnumId>();
                        if(enumId == null) {
                            throw new Exception(string.Format("Enum item '{0}.{1}' is missing [EnumItem(\"xyz\")] attribute", enumType.FullName, member));
                        }
                        writer.WriteValue(enumId.Value);
                    } 
                    else 
                    {
                        serializer.Serialize(writer, value);
                    }
                }
            }
        }
    }
}
