using System;
using System.Runtime.Serialization;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    [Serializable]
    public class RoleAlreadyExistsException : Exception
    {
        public RoleAlreadyExistsException()
        {
        }

        public RoleAlreadyExistsException(string message) : base(message)
        {
        }

        public RoleAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RoleAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}