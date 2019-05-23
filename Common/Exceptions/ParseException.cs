using System;

namespace BW.Common.Exceptions
{
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException()
        {

        }

        public ParseException(string message) : base(message)
        {

        }

        public ParseException(string message, System.Exception inner) : base(message, inner)
        {

        }

        protected ParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
