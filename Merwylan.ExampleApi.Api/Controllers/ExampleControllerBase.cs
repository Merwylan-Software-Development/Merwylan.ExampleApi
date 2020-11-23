using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Merwylan.ExampleApi.Api.Controllers
{
    public class ExampleControllerBase : ControllerBase
    {
        public StatusCodeResult InternalServerError()
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
        public ObjectResult InternalServerError(string message)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, message);
        }
    }
}
