using Microsoft.Extensions.DependencyInjection;

namespace Merwylan.ExampleApi.Audit
{
    public static class AuditDependencies
    {
        public static void ConfigureAuditingServices(this IServiceCollection collection)
        {
            collection.AddScoped<IAuditService, AuditService>();
        }
    }
}
