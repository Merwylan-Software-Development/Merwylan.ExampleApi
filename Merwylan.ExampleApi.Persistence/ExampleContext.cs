using Merwylan.ExampleApi.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Merwylan.ExampleApi.Persistence 
{ 
    public class ExampleContext : DbContext
    {
        private static readonly Action[] _actionsSeeds =
        {
            new Action { Id = 1, Value = "users-view" },
            new Action { Id = 2, Value = "users-add" },
            new Action { Id = 3, Value = "users-update" },
            new Action { Id = 4, Value = "users-delete" },
        };

        private static readonly Role[] _rolesSeeds =
        {
            new Role
            {
                // For seeding many-to-many, how do you reference both actions and users?
                Id = 1, Name = "Administrator", Actions = _actionsSeeds
            }
        };

        private static readonly User[] _usersSeeds =
        {
            new User
            {
                // Do we reference roles here? See comment above.
                // Default user root with password root123, delete after api is configured!
                Id = 1, Username = "root",
                HashedPassword = "$2a$11$CvBqa.owIsNm1bVVdg4sSO1p0SmEkRmpmljo3Ys9jj/Wg2eN5wcxK", Roles = _rolesSeeds
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
            modelBuilder.Entity<Action>().HasData(_actionsSeeds);
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Actions)
                .WithMany(a => a.Roles);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users);
            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User);

            //modelBuilder.Entity<User>().HasData(_usersSeeds);
        }
    }
}
