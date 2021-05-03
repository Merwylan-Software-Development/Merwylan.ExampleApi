using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Api.Extensions;
using Merwylan.ExampleApi.Audit;
using Merwylan.ExampleApi.Services;
using Merwylan.ExampleApi.Shared.Auth;
using Merwylan.ExampleApi.Shared.Extensions;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route(Program.API_PREFIX)]
    public class UsersController : ExampleControllerBase
    {

        public UsersController(ILogger<UsersController> logger, IUserService userService, IAuditService auditService) 
            : base(logger, userService, auditService)
        {
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            try
            {
                var response = await UserService.AuthenticateAsync(model, IpAddress());

                if (response == null)
                    return BadRequest();

                SetTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            // Keep this to override the middleware: in this particular case, it should be a bad request.
            catch (AuthenticationException)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return BadRequest();

            var response = await UserService.RefreshTokenAsync(refreshToken, IpAddress());

            if (response == null)
                return Unauthorized();

            SetTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            if (!AuthenticatedUser.HasClaim(Actions.RevokeTokens))
            {
                return Unauthorized();
            }

            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token)) return BadRequest();

            var response = await UserService.RevokeTokenAsync(token, IpAddress());
            if (!response) return NotFound();

            return Ok();
        }

        [HttpGet("[controller]")]
        public IActionResult GetAllUsers()
        {
            if (!AuthenticatedUser.HasClaim(Actions.ViewUsers))
            {
                return Unauthorized();
            }

            var users = UserService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("[controller]/{id}")]
        public IActionResult GetUserById(int id)
        {
            var authenticatedUser = AuthenticatedUser;
            if (authenticatedUser.Id ==id || !authenticatedUser.HasClaim(Actions.ViewUsers))
            {
                return Unauthorized();
            }

            var user = UserService.GetUserById(id);
            return Ok(user);
        }

        [HttpPost("[controller]")]
        public async Task<IActionResult> AddUserAsync(PostUser user)
        {
            if (!AuthenticatedUser.HasClaim(Actions.AddUsers))
            {
                return Unauthorized();
            }

            return Ok(await UserService.AddUserAsync(user));
        }

        [HttpPost("[controller]")]
        public async Task<IActionResult> EditUserAsync(PutUser user)
        {
            if (!AuthenticatedUser.HasClaim(Actions.EditUsers))
            {
                return Unauthorized();
            }

            await UserService.EditUserAsync(user);
            return Ok();
        }

        [HttpDelete("[controller]/{id}")]
        public async Task<IActionResult> DeleteUserByIdAsync(int id)
        {
            if (!AuthenticatedUser.HasClaim(Actions.DeleteUsers))
            {
                return Unauthorized();
            }

            await UserService.DeleteUserAsync(id);
            return Ok();
        }

        [HttpGet("actions")]
        public IActionResult GetAllActions()
        {
            if (!AuthenticatedUser.HasClaim(Actions.GetAllClaims))
            {
                return Unauthorized();
            }

            return Ok(UserService.GetActions());
        }

        [HttpGet]

        [HttpGet("[controller]/{id}/refresh-tokens")]
        public IActionResult GetRefreshTokensById(int id)
        {
            if (!AuthenticatedUser.HasClaim(Actions.ViewTokens))
            {
                return Unauthorized();
            }

            var user = UserService.GetUserById(id);
            return Ok(user.RefreshTokens);
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
