using System;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace Supertext.Base.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NET8_0_OR_GREATER
        protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}