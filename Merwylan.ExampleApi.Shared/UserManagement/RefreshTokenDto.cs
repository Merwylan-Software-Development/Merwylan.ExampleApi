using System;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string? IpAddress { get; set; }
    }
}