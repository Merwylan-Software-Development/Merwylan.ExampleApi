using System.ComponentModel.DataAnnotations;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    public class PostUser
    {
        [Required]
        [StringLength(16, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required] 
        public int[] Roles { get; set; } = null!;
    }
}
