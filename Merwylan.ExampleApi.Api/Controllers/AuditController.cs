using System;
using Merwylan.ExampleApi.Api.Attributes;
using Merwylan.ExampleApi.Api.Extensions;
using Merwylan.ExampleApi.Audit;
using Merwylan.ExampleApi.Services;
using Merwylan.ExampleApi.Shared.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Merwylan.ExampleApi.Api.Controllers
{
    [Route(Program.API_PREFIX + "[controller]")]
    [ApiController]
    [Authorize]
    public class AuditController : ExampleControllerBase
    {
        public AuditController(ILogger<AuditController> logger, IUserService userService, IAuditService auditService)
            : base(logger, userService, auditService)
        {
        }

        [HttpPost]
        [AuthorizedAction(Actions.AuditSearch)]
        public IActionResult Search(AuditSearchModel search)
        {
            return Ok(AuditService.GetAuditModels(search));
        }
    }
}
