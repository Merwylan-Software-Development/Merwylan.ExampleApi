using System.ComponentModel.DataAnnotations;

namespace Merwylan.ExampleApi.Shared.Auth
{
    public class AuthenticateRequest
    {
        [Required] 
        public string Username { get; set; } = string.Empty;

        [Required] 
        public string Password { get; set; } = string.Empty;
    }
}