using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Security;

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
        [RequirePermission("Compliance.View")]
        public IActionResult GetDashboard(string cycle)
        {
            var data = _context.StudentRequirementStatuses
                .Where(r => r.RequirementCycle == cycle)
                .ToList();

            var total = data.Count;
            var completed = data.Count(x => x.Status == "Completed" || x.Status == "Waived");

            var byStudent = data
                .GroupBy(r => r.StudentId)
                .Select(g => new
                {
                    studentId = g.Key,
                    total = g.Count(),
                    completed = g.Count(x => x.Status == "Completed" || x.Status == "Waived"),
                    complianceRate = g.Count() == 0 ? 0 : (int)((double)g.Count(x => x.Status == "Completed" || x.Status == "Waived") / g.Count() * 100)
                });

            return Ok(new
            {
                total,
                completed,
                complianceRate = total == 0 ? 0 : (int)((double)completed / total * 100),
                byStudent
            });
        }
    }
}
