using System;
using Merwylan.ExampleApi.Api.Extensions;
using Merwylan.ExampleApi.Audit;
using Merwylan.ExampleApi.Services;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Api.Controllers
{
    [Microsoft.AspNetCore.Components.Route(Program.API_PREFIX + "[controller]")]
    [ApiController]
    [Authorize]
    public class AuditController : ExampleControllerBase
    {
        public AuditController(ILogger<AuditController> logger, IUserService userService, IAuditService auditService)
            : base(logger, userService, auditService)
        {
        }

        [HttpPost]
        public IActionResult Search(AuditSearchModel search)
        {
            if (!AuthenticatedUser.HasClaim(Actions.AuditSearch))
            {
                return Unauthorized();
            }

            return Ok(AuditService.GetAuditModels(search));
        }
    }
}
