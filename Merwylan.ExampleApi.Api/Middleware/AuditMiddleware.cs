using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Audit;
using Merwylan.ExampleApi.Shared.Extensions;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Api.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IAuditService auditService)
        {
            try
            {
                context.Request.EnableBuffering();
                await _next.Invoke(context);
                await AuditAsync(context, auditService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to audit request.");
            }
        }

        private async Task AuditAsync(HttpContext context,IAuditService auditService)
        {
            var statusCode = context.Response.StatusCode;
            var endpoint = context.GetEndpoint();

            var auditModel = new AuditPostModel
            {
                StatusCode = statusCode,
                IsSuccessful = statusCode >= 200 && statusCode < 300,
                Method = context.Request.Method,
                Endpoint = endpoint.Metadata?.GetMetadata<HttpMethodAttribute>()?.Template, 
                // Do not display body when it concerns the user endpoints (sensitive data such as passwords
                // cannot be stored as plaintext)
                Request = endpoint.DisplayName.Contains("users", StringComparison.CurrentCultureIgnoreCase) 
                    ? null : await GetRequestAsync(context),
            };

            _logger.LogInformation($"Attempting to audit model {auditModel.SerializeCamelCase()}.");
            await auditService.AddAuditTrailAsync(auditModel);
        }

        private async Task<string> GetRequestAsync(HttpContext context)
        {
            var inputStream = context.Request.GetContentFromFileOrBody();
            inputStream.Position = 0;

            using var streamReader = new StreamReader(inputStream, Encoding.UTF8);

            return await streamReader.ReadToEndAsync();
        }
    }
}
