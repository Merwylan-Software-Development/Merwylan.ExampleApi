using System.Text.Json.Serialization;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        [JsonIgnore] 
        public string HashedPassword { get; set; } = string.Empty;

        [JsonIgnore] 
        public RefreshTokenDto[] RefreshTokens { get; set; } = new RefreshTokenDto[0];

        public RoleDto[] Roles { get; set; } = new RoleDto[0];

    }
}
