using System.ComponentModel.DataAnnotations;

namespace Merwylan.ExampleApi.Shared.UserManagement
{
    public class PutUser
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The userId must be provided to edit a user.")]
        public int Id { get; set; }

        [StringLength(16, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 3)]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "The roles of the user must be provided to edit a user.")]
        public int[] Roles { get; set; } = new int[0];
    }
}
