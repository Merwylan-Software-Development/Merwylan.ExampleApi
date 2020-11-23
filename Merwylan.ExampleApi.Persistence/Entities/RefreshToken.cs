using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Merwylan.ExampleApi.Persistence.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        [NotNull]
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string? IpAddress { get; set; } 
        public bool IsActive { get; set; }
        public DateTime Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public User User { get; set; } = null!;
    }
}
