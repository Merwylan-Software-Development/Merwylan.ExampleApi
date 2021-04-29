﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Merwylan.ExampleApi.Persistence.Entities
{
    public class Role
    {

        public Role()
        {
            this.Actions= new HashSet<Action>();
            this.Users= new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }

        [NotNull] public string Name { get; set; } = string.Empty;

        public virtual ICollection<Action> Actions { get; set; } = null!;
        public virtual ICollection<User> Users { get; set; } = null!;
    }
}
