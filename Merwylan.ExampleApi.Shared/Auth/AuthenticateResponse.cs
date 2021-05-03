using System.Text.Json.Serialization;
using Merwylan.ExampleApi.Shared.UserManagement;

namespace Merwylan.ExampleApi.Shared.Auth
{
    public class AuthenticateResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(UserDto user, string jwtToken, string refreshToken)
        {
            UserId = user.Id;
            Username = user.Username;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}