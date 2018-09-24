using System;
namespace ActivityInfo.Query
{
    [System.AttributeUsage(System.AttributeTargets.Property)]     
    public class Formula : System.Attribute  
    {
        private string value;
        public Formula(string value)
        {
            this.value = value;
        }

        public string Value {
            get {
                return value;
            }
        }
    }
}
