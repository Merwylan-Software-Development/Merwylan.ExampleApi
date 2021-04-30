using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger) :base(userService)
        {
            _logger = logger;
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
            catch (UserDoesNotExistException)
            {
                return BadRequest();
            }
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

            try
            {
                var response = await UserService.RefreshTokenAsync(refreshToken, IpAddress());

                if (response == null)
                    return Unauthorized();

                SetTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (RefreshTokenNotFoundException)
            {
                return BadRequest();
            }
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            try
            {
                var token = model.Token ?? Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(token))
                    return BadRequest();

                if (!AuthenticatedUser.HasClaim(Actions.RevokeTokens))
                {
                    return Unauthorized();
                }

                var response = await UserService.RevokeTokenAsync(token, IpAddress());

                if (!response)
                    return NotFound();

                return Ok();
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while attempting to revoke token.");
                return InternalServerError();
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                if (!AuthenticatedUser.HasClaim(Actions.ViewUsers))
                {
                    return Unauthorized();
                }

                var users = UserService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while attempting to retrieve all users.");
                return InternalServerError();
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                if (!AuthenticatedUser.HasClaim(Actions.ViewUsers))
                {
                    return Unauthorized();
                }

                var user = UserService.GetUserById(id);
                return Ok(user);
            }
            catch (UserDoesNotExistException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while attempting to retrieve a user by id {id}.");
                return InternalServerError();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync(PostUser user)
        {
            try
            {
                if (!AuthenticatedUser.HasClaim(Actions.AddUsers))
                {
                    return Unauthorized();
                }

                return Ok(await UserService.AddUserAsync(user));
            }
            catch (UserAlreadyExistsException)
            {
                return BadRequest($"User with name {user.Username} already exists.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to add a user.");
                return InternalServerError();
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditUserAsync(PutUser user)
        {
            try
            {
                if (!AuthenticatedUser.HasClaim(Actions.EditUsers))
                {
                    return Unauthorized();
                }

                await UserService.EditUserAsync(user);
                return Ok();
            }
            catch (UserDoesNotExistException)
            {
                return BadRequest();
            }
            catch (UserAlreadyExistsException)
            {
                return BadRequest($"User with name {user.Username} already exists.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to edit a user.");
                return InternalServerError();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserByIdAsync(int id)
        {
            try
            {
                if (!AuthenticatedUser.HasClaim(Actions.DeleteUsers))
                {
                    return Unauthorized();
                }

                await UserService.DeleteUserAsync(id);
                return Ok();
            }
            catch (UserDoesNotExistException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"An error occurred while attempting to delete user with ID {id}.");
                return InternalServerError();
            }
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokensById(int id)
        {
            try
            {
                if (!AuthenticatedUser.HasClaim(Actions.ViewRefreshTokens))
                {
                    return Unauthorized();
                }

                var user = UserService.GetUserById(id);
                return Ok(user.RefreshTokens);
            }
            catch (UserDoesNotExistException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred while attempting to get the refresh tokens for user {id}.");
                return InternalServerError();
            }
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
