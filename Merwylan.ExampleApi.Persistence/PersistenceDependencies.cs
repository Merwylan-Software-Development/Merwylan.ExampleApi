using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Merwylan.ExampleApi.Persistence
{
    public static class PersistenceDependencies
    {
        public static void ConfigurePersistenceServices(this IServiceCollection collection, string connectionString)
        {
            collection.AddDbContext<UserManagementContext>(options => options.UseSqlServer(connectionString));
            collection.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
