using System;
using System.Runtime.Serialization;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    [Serializable]
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException()
        {
        }

        public UserDoesNotExistException(string message) : base(message)
        {
        }

        public UserDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}