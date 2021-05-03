using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence.Entities;
using Merwylan.ExampleApi.Services;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Merwylan.ExampleApi.Api.Attributes
{
    public class AuthorizedActionFilter : IAuthorizationFilter
    {
        private Actions[] _actions;
        public AuthorizedActionFilter(Actions[] actions)
        {
            _actions = actions ?? throw new ArgumentNullException(nameof(_actions), @"Please provide a non-empty action value.");
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if(!IsAuthorized(context))
                    context.Result = new UnauthorizedResult();
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }

        public virtual bool IsAuthorized(AuthorizationFilterContext context)
        {
            var user = GetUser(context.HttpContext);
            var authorizedActions = user.Roles.SelectMany(x => x.Actions).Select(x=> x.Id);
            return _actions.All(action => authorizedActions.Contains((int)action));
        }

        private UserDto GetUser(HttpContext context)
        {
            var claimValue = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var isValidInt = int.TryParse(claimValue, out var userId);

            if (!isValidInt) throw new AuthenticationException();

            var userService = context.RequestServices.GetRequiredService<IUserService>();
            var user = userService.GetUserById(userId);
            if (user == null) throw new AuthenticationException();

            return user;
        }
    }
}
