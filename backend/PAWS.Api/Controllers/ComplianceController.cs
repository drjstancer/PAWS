using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/compliance")]
    public class ComplianceController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public ComplianceController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var total = _context.StudentRequirementStatuses.Count();
            var completed = _context.StudentRequirementStatuses.Count(x => x.Status == "Completed" || x.Status == "Waived");
            var rate = total == 0 ? 0 : (int)((double)completed / total * 100);

            return Ok(new
            {
                totalRequirements = total,
                completed,
                complianceRate = rate
            });
        }
    }
}
