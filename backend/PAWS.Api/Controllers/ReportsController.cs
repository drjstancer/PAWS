using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public ReportsController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpGet("grant-summary")]
        public IActionResult GetGrantSummary(string cycle)
        {
            var students = _context.Students.ToList();
            var requirements = _context.StudentRequirementStatuses
                .Where(r => r.RequirementCycle == cycle)
                .ToList();

            var summary = students.Select(s =>
            {
                var studentReqs = requirements.Where(r => r.StudentId == s.Id).ToList();
                var completed = studentReqs.Count(r => r.Status == "Completed" || r.Status == "Waived");

                return new
                {
                    s.MuId,
                    s.ProgramTrack,
                    s.Classification,
                    s.CohortYear,
                    s.RucaCategory,
                    totalRequirements = studentReqs.Count,
                    completed,
                    complianceRate = studentReqs.Count == 0 ? 0 : (int)((double)completed / studentReqs.Count * 100)
                };
            }).ToList();

            return Ok(summary);
        }
    }
}
