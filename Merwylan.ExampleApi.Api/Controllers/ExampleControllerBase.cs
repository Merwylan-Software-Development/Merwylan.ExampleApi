using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using Merwylan.ExampleApi.Audit;
using Merwylan.ExampleApi.Services;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Api.Controllers
{
    public class ExampleControllerBase : ControllerBase
    {
        protected readonly IUserService UserService;
        protected readonly IAuditService AuditService;
        protected readonly ILogger<ControllerBase> Logger;

        public ExampleControllerBase(ILogger<ControllerBase> logger, IUserService userService, IAuditService auditService)
        {
            Logger = logger;
            UserService = userService;
            AuditService = auditService;
        }

        internal StatusCodeResult InternalServerError()
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
        internal ObjectResult InternalServerError(string message)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, message);
        }

    }   
}
