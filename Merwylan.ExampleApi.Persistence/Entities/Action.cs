using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Merwylan.ExampleApi.Persistence.Entities
{
    public class Action
    {
        public Action(){
            Roles = new HashSet<Role>();
        }

        [Key]
        public int Id { get; set; }

        [NotNull] 
        public string Value { get; set; } = string.Empty;

        public ICollection<Role> Roles { get; set; } = null!;
    }
}
