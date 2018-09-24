using System;
namespace ActivityInfo.Query
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class FormId : System.Attribute
    {
        private string value;

        public FormId(string value)
        {
            this.value = value;
        }

        public string Value {
            get {
                return Value;
            }
        }
    }
}
