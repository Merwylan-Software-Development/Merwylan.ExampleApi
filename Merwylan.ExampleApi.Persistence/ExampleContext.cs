using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Merwylan.ExampleApi.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Merwylan.ExampleApi.Persistence 
{ 
    public class ExampleContext : DbContext
    {
        public static readonly Action[] ActionsSeeds =
        {
            new Action { Id = 1, Value = "tokens-revoke"},
            new Action { Id = 2, Value = "tokens-view" },
            new Action { Id = 3, Value = "users-view" },
            new Action { Id = 4, Value = "users-add" },
            new Action { Id = 5, Value = "users-edit" },
            new Action { Id = 6, Value = "users-delete" },
            new Action{Id = 7, Value = "actions-view"},
            new Action{Id = 8, Value = "audit-search"} 
        };

        public static readonly Role[] RolesSeeds =
        {
            new Role
            {
                Name = "Administrator", Actions = ActionsSeeds
            }
        };

        public static readonly User[] UsersSeeds =
        {
            new User
            {
                // Do we reference roles here? See comment above.
                // Default user root with password root123, delete after api is configured!
                Username = "root",
                HashedPassword = "$2a$11$CvBqa.owIsNm1bVVdg4sSO1p0SmEkRmpmljo3Ys9jj/Wg2eN5wcxK",
                Roles = RolesSeeds
            }
        };

        public ExampleContext(DbContextOptions<ExampleContext> options) : base(options) { }

        public DbSet<Action> Actions { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AuditTrail> AuditTrails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>();
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Actions)
                .WithMany(a => a.Roles);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users);
            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User);
        }
    }
}
