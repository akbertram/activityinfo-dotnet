using System;
namespace ActivityInfo
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class EnumId : System.Attribute
    {
        private string value;
        public EnumId(string value)
        {
            this.value = value;
        }

        public string Value
        {
            get
            {
                return value;
            }
        }
    }
}
