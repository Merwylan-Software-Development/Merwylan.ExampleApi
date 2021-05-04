using System;
using System.Linq;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Api
{
    public class Program
    {
        public const string API_PREFIX = "example/api/";

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = services.GetRequiredService<ExampleContext>();
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await context.Database.MigrateAsync();

                    await using var transaction = await context.Database.BeginTransactionAsync();
                    var missingActions =
                        ExampleContext.ActionsSeeds.Where(x => !context.Actions.Any(y => y.Id == x.Id));
                    await context.Actions.AddRangeAsync(missingActions);

                    var missingRoles =
                        ExampleContext.RolesSeeds.Where(x => !context.Roles.Any(y => y.Id == x.Id));
                    await context.Roles.AddRangeAsync(missingRoles);
                    var missingUsers =
                        ExampleContext.UsersSeeds.Where(x => !context.Users.Any(y => y.Id == x.Id));
                    await context.Users.AddRangeAsync(missingUsers);

                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Actions] ON");
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Actions] OFF");

                    await transaction.CommitAsync();
                }
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(e, "An error occurred during migration");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
