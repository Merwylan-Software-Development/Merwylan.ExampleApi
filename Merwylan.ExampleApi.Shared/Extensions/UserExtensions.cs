using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Merwylan.ExampleApi.Shared.UserManagement;

namespace Merwylan.ExampleApi.Shared.Extensions
{
    public static class UserExtensions
    {
        public static bool HasClaim(this UserDto user, Actions action)
        {
            return user.AuthorizedActions.Any(x => x.Name == action.ToString());
        }
    }
}
