using System.Text.Json.Serialization;
using Merwylan.ExampleApi.Shared.UserManagement;

namespace Merwylan.ExampleApi.Shared.Auth
{
    public class AuthenticateResponse
    {
        public UserDto User { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(UserDto user, string jwtToken, string refreshToken)
        {
            User = user;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}