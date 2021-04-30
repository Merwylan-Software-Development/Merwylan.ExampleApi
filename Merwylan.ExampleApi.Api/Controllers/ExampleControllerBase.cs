using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using Merwylan.ExampleApi.Services;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Mvc;

namespace Merwylan.ExampleApi.Api.Controllers
{
    public class ExampleControllerBase : ControllerBase
    {
        protected readonly IUserService UserService;
        protected UserDto AuthenticatedUser => GetUser();

        public ExampleControllerBase(IUserService userService)
        {
            UserService = userService;
        }

        internal StatusCodeResult InternalServerError()
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
        internal ObjectResult InternalServerError(string message)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, message);
        }

        private UserDto GetUser()
        {
            var claimValue = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var isValidInt = int.TryParse(claimValue, out var userId);

            if (!isValidInt) throw new AuthenticationException();

            var user = UserService.GetUserById(userId);
            if (user == null) throw new AuthenticationException();

            return user;
        }
    }
}
