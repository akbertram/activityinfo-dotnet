using System;
namespace ActivityInfo
{
    public class ActivityInfoException : Exception
    {
        public ActivityInfoException()
        {
        }

        public ActivityInfoException(string message) : base(message)
        {
        }

        public ActivityInfoException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
