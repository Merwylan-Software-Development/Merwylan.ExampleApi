using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Merwylan.ExampleApi.Shared.UserManagement;

namespace Merwylan.ExampleApi.Api.Extensions
{
    public static class UserExtensions
    {
        public static bool HasClaim(this UserDto user, Actions action)
        {
            return user.Roles.Any(x => x.Actions.Any(y => y.Id == (int)action));
        }
    }
}
