using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Mail
{
    public static class MailDependencies
    {
        public static void ConfigureMailServices(this IServiceCollection collection, MailConfig config)
        {
            collection.AddSingleton(x=> new MailService(x.GetService<ILogger<MailService>>(), config));
            collection.AddSingleton<IMailService>(x => x.GetService<MailService>());
        }
    }
}
