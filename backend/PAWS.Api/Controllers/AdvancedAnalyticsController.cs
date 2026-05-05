using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/advanced-analytics")]
    public class AdvancedAnalyticsController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public AdvancedAnalyticsController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpGet("risk-signals")]
        public IActionResult GetRiskSignals(string cycle)
        {
            var students = _context.Students.ToList();
            var reqs = _context.StudentRequirementStatuses.Where(r => r.RequirementCycle == cycle).ToList();

            var results = students.Select(s =>
            {
                var studentReqs = reqs.Where(r => r.StudentId == s.Id).ToList();
                int missing = studentReqs.Count(r => r.Status != "Completed" && r.Status != "Waived");

                int riskScore = 0;
                var factors = new List<string>();

                if (missing > 3)
                {
                    riskScore += 2;
                    factors.Add("Multiple incomplete requirements");
                }

                if (s.CumulativeGpa.HasValue && s.CumulativeGpa < 3.0m)
                {
                    riskScore += 2;
                    factors.Add("Low cumulative GPA");
                }

                if (s.ScienceGpa.HasValue && s.ScienceGpa < 2.8m)
                {
                    riskScore += 2;
                    factors.Add("Low science GPA");
                }

                var level = riskScore >= 4 ? "High" : riskScore >= 2 ? "Moderate" : "Low";

                return new RiskSignalDto
                {
                    StudentId = s.Id,
                    MuId = s.MuId,
                    StudentName = $"{s.FirstName} {s.LastName}",
                    ProgramTrack = s.ProgramTrack,
                    Classification = s.Classification,
                    RiskScore = riskScore,
                    RiskLevel = level,
                    RiskFactors = factors,
                    RecommendedAction = level == "High" ? "Immediate advising intervention" : "Monitor"
                };
            }).ToList();

            return Ok(results);
        }

        [HttpGet("publication-table")]
        public IActionResult GetPublicationTable()
        {
            var data = _context.Students
                .GroupBy(s => s.ProgramTrack)
                .Select(g => new {
                    Program = g.Key,
                    Count = g.Count(),
                    AvgGpa = g.Where(x => x.CumulativeGpa.HasValue).Average(x => x.CumulativeGpa)
                }).ToList();

            var table = new PublicationTableDto
            {
                TableName = "Program Participation and GPA",
                Columns = new List<string> { "Program", "Count", "Average GPA" },
                Rows = data.Select(d => new Dictionary<string, object?>
                {
                    { "Program", d.Program },
                    { "Count", d.Count },
                    { "Average GPA", d.AvgGpa }
                }).ToList()
            };

            return Ok(table);
        }

        [HttpGet("faculty-report")]
        public IActionResult GetFacultyReport(string cycle)
        {
            var report = new FacultyReportDto
            {
                ReportingCycle = cycle,
                ExecutiveSummary = new { message = "PAWS program is demonstrating measurable student engagement and academic outcomes." },
                ParticipationSummary = _context.Students.Count(),
                ComplianceSummary = _context.StudentRequirementStatuses.Where(r => r.RequirementCycle == cycle).Count(),
                AcademicSummary = _context.Students.Average(s => s.CumulativeGpa),
                EquityRuralitySummary = _context.Students.GroupBy(s => s.RucaCategory).Select(g => new { g.Key, Count = g.Count() }),
                RiskSummary = "See risk-signals endpoint"
            };

            return Ok(report);
        }
    }
}
