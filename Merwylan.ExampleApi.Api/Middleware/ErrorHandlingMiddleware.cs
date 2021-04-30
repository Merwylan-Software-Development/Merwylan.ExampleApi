using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Merwylan.ExampleApi.Shared.Extensions;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to handle the request.");
            switch (ex)
            {
                case AuthenticationException e:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case UserDoesNotExistException e:
                case UserAlreadyExistsException e2:
                case RefreshTokenNotFoundException e3:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case { } e:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
        }
    }
}
