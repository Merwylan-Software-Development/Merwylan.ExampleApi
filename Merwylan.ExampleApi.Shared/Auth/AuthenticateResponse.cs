using System.Text.Json.Serialization;
using Merwylan.ExampleApi.Shared.UserManagement;

namespace Merwylan.ExampleApi.Shared.Auth
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(UserDto user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Username = user.Username;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}