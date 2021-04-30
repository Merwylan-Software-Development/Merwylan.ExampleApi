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
    [Route(Program.API_PREFIX + "[controller]")]
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
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token)) return BadRequest();

            if (!AuthenticatedUser.HasClaim(Actions.RevokeTokens)) 
            {
                return Unauthorized();
            }

            var response = await UserService.RevokeTokenAsync(token, IpAddress());
            if (!response) return NotFound();

            return Ok();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (!AuthenticatedUser.HasClaim(Actions.ViewUsers))
            {
                return Unauthorized();
            }

            var users = UserService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (!AuthenticatedUser.HasClaim(Actions.ViewUsers))
            {
                return Unauthorized();
            }

            var user = UserService.GetUserById(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync(PostUser user)
        {
            if (!AuthenticatedUser.HasClaim(Actions.AddUsers))
            {
                return Unauthorized();
            }

            return Ok(await UserService.AddUserAsync(user));
        }

        [HttpPut]
        public async Task<IActionResult> EditUserAsync(PutUser user)
        {
            if (!AuthenticatedUser.HasClaim(Actions.EditUsers))
            {
                return Unauthorized();
            }

            await UserService.EditUserAsync(user);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserByIdAsync(int id)
        {
            if (!AuthenticatedUser.HasClaim(Actions.DeleteUsers))
            {
                return Unauthorized();
            }

            await UserService.DeleteUserAsync(id);
            return Ok();
        }

        [HttpGet("{id}/refresh-tokens")]
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
