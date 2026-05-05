using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/compliance/missing")]
    public class MissingRequirementsController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public MissingRequirementsController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetMissing(string cycle)
        {
            var missing = _context.StudentRequirementStatuses
                .Where(r => r.RequirementCycle == cycle && r.Status != "Completed" && r.Status != "Waived")
                .ToList();

            return Ok(missing);
        }
    }
}
