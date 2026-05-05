using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/trend-analytics")]
    public class TrendAnalyticsController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public TrendAnalyticsController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpGet("compliance-trend")]
        public IActionResult GetComplianceTrend(string cycle)
        {
            var data = _context.StudentRequirementStatuses
                .Where(r => r.RequirementCycle == cycle)
                .ToList()
                .GroupBy(r => r.CompletionDate?.ToString("yyyy-MM") ?? "Incomplete")
                .Select(g => new
                {
                    period = g.Key,
                    total = g.Count(),
                    completed = g.Count(r => r.Status == "Completed" || r.Status == "Waived"),
                    complianceRate = g.Count() == 0 ? 0 : (int)((double)g.Count(r => r.Status == "Completed" || r.Status == "Waived") / g.Count() * 100)
                })
                .OrderBy(x => x.period)
                .ToList();

            return Ok(data);
        }

        [HttpGet("gpa-trend")]
        public IActionResult GetGpaTrend()
        {
            var data = _context.AcademicRecords
                .ToList()
                .GroupBy(a => new { a.AcademicYear, a.Term })
                .Select(g => new
                {
                    period = $"{g.Key.AcademicYear} {g.Key.Term}",
                    averageCumulativeGpa = g.Where(a => a.CumulativeGpa.HasValue).Any()
                        ? Math.Round(g.Where(a => a.CumulativeGpa.HasValue).Average(a => a.CumulativeGpa!.Value), 2)
                        : (decimal?)null,
                    averageScienceMathGpa = g.Where(a => a.ScienceMathGpa.HasValue).Any()
                        ? Math.Round(g.Where(a => a.ScienceMathGpa.HasValue).Average(a => a.ScienceMathGpa!.Value), 2)
                        : (decimal?)null
                })
                .OrderBy(x => x.period)
                .ToList();

            return Ok(data);
        }

        [HttpGet("risk-distribution")]
        public IActionResult GetRiskDistribution(string cycle)
        {
            var students = _context.Students.ToList();
            var reqs = _context.StudentRequirementStatuses.Where(r => r.RequirementCycle == cycle).ToList();

            var risks = students.Select(s =>
            {
                var studentReqs = reqs.Where(r => r.StudentId == s.Id).ToList();
                var missing = studentReqs.Count(r => r.Status != "Completed" && r.Status != "Waived");
                var score = 0;
                if (missing > 3) score += 2;
                if (s.CumulativeGpa.HasValue && s.CumulativeGpa < 3.0m) score += 2;
                if (s.ScienceGpa.HasValue && s.ScienceGpa < 2.8m) score += 2;
                return score >= 4 ? "High" : score >= 2 ? "Moderate" : "Low";
            })
            .GroupBy(level => level)
            .Select(g => new { riskLevel = g.Key, count = g.Count() })
            .OrderBy(x => x.riskLevel)
            .ToList();

            return Ok(risks);
        }
    }
}
