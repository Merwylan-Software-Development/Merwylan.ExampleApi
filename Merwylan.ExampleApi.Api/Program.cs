using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Merwylan.ExampleApi.Api
{
    public class Program
    {
        public const string API_PREFIX = "example/api/";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
