using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActivityInfo.Schema
{
    public class FormElementJsonConverter : JsonConverter
    {
        public FormElementJsonConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(FormElement));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var elementObject = JToken.ReadFrom(reader) as JObject;
            var elementType = elementObject["type"];

            if(elementType.Equals("section")) {
                return new FormSection();
            } else {

                Field field = new Field();
                serializer.Populate(elementObject.CreateReader(), field);

                field.Type = ReadType(elementObject, serializer);
                return field;
            }
        }

        public FieldType ReadType(JObject jObject, JsonSerializer serializer) {

            string type = jObject["type"].ToString().ToLower();

            JObject parameters = jObject["typeParameters"] as JObject;

            if (type.Equals("free_text"))
            {
                return serializer.Deserialize<TextType>(parameters.CreateReader());
            }
            else if (type.Equals("quantity"))
            {
                return serializer.Deserialize<QuantityType>(parameters.CreateReader());
            }
            else if (type.Equals("enumerated"))
            {
                return serializer.Deserialize<EnumeratedType>(parameters.CreateReader());
            }
            else if (type.Equals("calculated"))
            {
                return serializer.Deserialize<EnumeratedType>(parameters.CreateReader());
            }
            else if (type.Equals("subform"))
            {
                return serializer.Deserialize<SubFormFieldType>(parameters.CreateReader());
            }
            else if (type.Equals("reference"))
            {
                return serializer.Deserialize<ReferenceType>(parameters.CreateReader());
            }
            else if (type.Equals("date"))
            {
                return new SimpleType(FieldTypeClass.LocalDate);
            }
            else if (type.Equals("month"))
            {
                return new SimpleType(FieldTypeClass.Month);
            }
            else if (type.Equals("epiweek"))
            {
                return new SimpleType(FieldTypeClass.Week);
            }
            else if (type.Equals("geopoint"))
            {
                return new SimpleType(FieldTypeClass.GeoPoint);
            }
            else if (type.Equals("geoarea"))
            {
                return new SimpleType(FieldTypeClass.GeoArea);
            }
            else if (type.Equals("barcode"))
            {
                return new SimpleType(FieldTypeClass.Barcode);
            } 
            else if(type.Equals("narrative")) 
            {
                return new SimpleType(FieldTypeClass.Narrative);    
            } 
            else if(type.Equals("attachment")) 
            {
                return new SimpleType(FieldTypeClass.Attachment);
            } 
            else if(type.Equals("serial")) 
            {
                return new SimpleType(FieldTypeClass.SerialNumber);
            } else {
                throw new ActivityInfoException(string.Format("Unsupported field type '{0}'", type));
            }

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
