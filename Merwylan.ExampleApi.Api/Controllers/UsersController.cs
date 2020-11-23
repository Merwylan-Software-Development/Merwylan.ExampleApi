using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Services;
using Merwylan.ExampleApi.Shared.Auth;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ExampleControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            try
            {
                var response = await _userService.AuthenticateAsync(model, IpAddress());
                if (response == null)
                    return BadRequest(new {message = "Username or password is incorrect"});

                SetTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (UserDoesNotExistException)
            {
                return Unauthorized();
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
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
                var response = await _userService.RefreshTokenAsync(refreshToken, IpAddress());

                if (response == null)
                    return Unauthorized(new {message = "Invalid token"});

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
                // We also accept the token from the cookies (yuummiiie!!)
                var token = model.Token ?? Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(token))
                    return BadRequest(new {message = "Token is required"});

                var response = await _userService.RevokeTokenAsync(token, IpAddress());

                if (!response)
                    return NotFound(new {message = "Token not found"});

                return Ok(new {message = "Token revoked"});
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
                var users = _userService.GetAllUsers();
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
                var user = _userService.GetUserById(id);
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
        [AllowAnonymous]
        public async Task<IActionResult> AddUserAsync(PostUser user)
        {
            try
            {
                return Ok(await _userService.AddUserAsync(user));
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
                await _userService.EditUserAsync(user);
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
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
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
        public IActionResult GetRefreshTokens(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);
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
