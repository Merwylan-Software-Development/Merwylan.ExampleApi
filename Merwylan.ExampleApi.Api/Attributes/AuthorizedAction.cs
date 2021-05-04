using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Mvc;

namespace Merwylan.ExampleApi.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizedAction : TypeFilterAttribute
    {
        public AuthorizedAction(Actions action) : base(typeof(AuthorizedActionFilter))
        {
            Arguments = new []{ (object)action};
        }
    }
}
