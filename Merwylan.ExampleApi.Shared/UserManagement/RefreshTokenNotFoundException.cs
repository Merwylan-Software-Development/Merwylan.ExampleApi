using System;
using System.Runtime.Serialization;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    [Serializable]
    public class RefreshTokenNotFoundException : Exception
    {
        public RefreshTokenNotFoundException()
        {
        }

        public RefreshTokenNotFoundException(string message) : base(message)
        {
        }

        public RefreshTokenNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RefreshTokenNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}