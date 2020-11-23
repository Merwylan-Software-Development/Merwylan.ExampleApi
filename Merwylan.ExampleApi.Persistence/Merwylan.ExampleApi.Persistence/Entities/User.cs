using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Merwylan.ExampleApi.Persistence.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [NotNull] 
        public string Username { get; set; } = null!;

        [NotNull] 
        public string HashedPassword { get; set; } = null!;

        public virtual ICollection<Role> Roles { get; set; } = null!;

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = null!;
    }
}
