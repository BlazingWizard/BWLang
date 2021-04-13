using System;

namespace BW.Common.Exceptions
{
    [Serializable]
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {

        }

        public InvalidTokenException(string message) : base(message)
        {

        }

        public InvalidTokenException(string message, System.Exception inner) : base(message, inner)
        {

        }

        protected InvalidTokenException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
