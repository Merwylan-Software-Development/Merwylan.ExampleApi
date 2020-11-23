using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Merwylan.ExampleApi.Persistence.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [NotNull] public string Name { get; set; } = string.Empty;

        public virtual ICollection<Action> Actions { get; set; } = null!;
        public virtual ICollection<User> Users { get; set; } = null!;
    }
}
