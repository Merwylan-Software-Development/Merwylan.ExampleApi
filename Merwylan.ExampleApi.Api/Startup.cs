using System;
using System.Configuration;
using System.Text;
using Merwylan.ExampleApi.Api.Middleware;
using Merwylan.ExampleApi.Audit;
using Merwylan.ExampleApi.Mail;
using Merwylan.ExampleApi.Persistence;
using Merwylan.ExampleApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;

namespace Merwylan.ExampleApi.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://merwylan.nl", "https://www.merwylan.com");
                    });
            });

            services.Configure<IISServerOptions>(options => options.AutomaticAuthentication = false);
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddLogging(x =>
            {
                x.AddConsole();
                x.AddNLog();
            });

            services.ConfigureMailServices(new MailConfig
            {
                Port = int.Parse(ConfigurationManager.AppSettings["mailPort"]),
                Sender = ConfigurationManager.AppSettings["mailSender"],
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"],
                Recipients = ConfigurationManager.AppSettings["mailRecipients"],
                SenderPassword = ConfigurationManager.AppSettings["mailPassword"]
            });

            var securityKey = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["securityKey"]);
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService>(x => new UserService(x.GetService<IUserRepository>()!, securityKey));

            services.AddAuthentication(
                x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
                ).AddJwtBearer(x=> 
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(securityKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.ConfigurePersistenceServices(ConfigurationManager.ConnectionStrings["database"].ConnectionString);
            services.ConfigureAuditingServices();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Merwylan Example API V1");
            });

            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseMiddleware<AuditMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
