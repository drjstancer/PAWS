using Microsoft.AspNetCore.Mvc;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/requirements/bulk")]
    public class BulkRequirementsController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateBulk()
        {
            return StatusCode(410, new
            {
                message = "This legacy endpoint has been retired. Use POST /api/v1/requirements/bulk-generate instead."
            });
        }
    }
}
