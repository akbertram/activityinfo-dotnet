using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityInfo.Schema
{

    public enum FieldTypeClass
    {
        Text,
        Quantity,
        Enumerated,
        Reference,
        Subform,
        Narrative,
        LocalDate,
        Month,
        Week,
        Fortnight,
        GeoPoint,
        GeoArea,
        Boolean,
        Barcode,
        Attachment,
        SerialNumber,
        Calculated
    }

    public interface FieldType
    {
        FieldTypeClass TypeClass { get; }
    }

    public class SimpleType : FieldType {
        private FieldTypeClass typeClass;

        public SimpleType(FieldTypeClass typeClass)
        {
            this.typeClass = typeClass;
        }

        public FieldTypeClass TypeClass => typeClass;
    }

    public class TextType : FieldType
    {
        [JsonProperty("inputMask")]
        public string InputMask { get; set; }

        public FieldTypeClass TypeClass => FieldTypeClass.Text;
    }

    public class QuantityType : FieldType 
    {
        [JsonProperty("units")]
        public string Units { get; set; }

        public FieldTypeClass TypeClass => FieldTypeClass.Quantity;
    }

    public enum Cardinality {
        Single,
        Multiple
    }

    public class EnumItem {

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("label")]
        public string Label;
    }

    public class EnumeratedType : FieldType
    {
        public FieldTypeClass TypeClass => FieldTypeClass.Enumerated;

        [JsonProperty("values")]
        public List<EnumItem> Items { get; set; }
    }

    public class ReferencedForm 
    {
        [JsonProperty("formId")]
        public string FormId { get; set; }
    }

    public class ReferenceType : FieldType
    {
        public FieldTypeClass TypeClass => FieldTypeClass.Reference;

        [JsonProperty("range")]
        public List<ReferencedForm> Range { get; set; }
    }

    public class CalculatedFieldType : FieldType 
    {
        public FieldTypeClass TypeClass => FieldTypeClass.Calculated;

        [JsonProperty("formula")]
        public string Formula { get; set; }
    }

    public class SubFormFieldType : FieldType 
    {
        public FieldTypeClass TypeClass => FieldTypeClass.Subform;
    
        [JsonProperty("formId")]
        public String FormId { get; set; }
    
    }



}
