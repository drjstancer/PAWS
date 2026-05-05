using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public AnalyticsController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpGet("overview")]
        public IActionResult GetOverview()
        {
            var students = _context.Students.ToList();

            var byProgram = students
                .GroupBy(s => s.ProgramTrack)
                .Select(g => new
                {
                    programTrack = g.Key,
                    count = g.Count(),
                    averageCumulativeGpa = g.Where(s => s.CumulativeGpa.HasValue).Any()
                        ? Math.Round(g.Where(s => s.CumulativeGpa.HasValue).Average(s => s.CumulativeGpa!.Value), 2)
                        : (decimal?)null,
                    averageScienceGpa = g.Where(s => s.ScienceGpa.HasValue).Any()
                        ? Math.Round(g.Where(s => s.ScienceGpa.HasValue).Average(s => s.ScienceGpa!.Value), 2)
                        : (decimal?)null
                })
                .OrderBy(x => x.programTrack)
                .ToList();

            var byClassification = students
                .GroupBy(s => s.Classification)
                .Select(g => new { classification = g.Key, count = g.Count() })
                .OrderBy(x => x.classification)
                .ToList();

            var byRuca = students
                .GroupBy(s => s.RucaCategory ?? "Unknown")
                .Select(g => new { rucaCategory = g.Key, count = g.Count() })
                .OrderBy(x => x.rucaCategory)
                .ToList();

            var byCohort = students
                .GroupBy(s => s.CohortYear)
                .Select(g => new
                {
                    cohortYear = g.Key,
                    count = g.Count(),
                    averageCumulativeGpa = g.Where(s => s.CumulativeGpa.HasValue).Any()
                        ? Math.Round(g.Where(s => s.CumulativeGpa.HasValue).Average(s => s.CumulativeGpa!.Value), 2)
                        : (decimal?)null
                })
                .OrderBy(x => x.cohortYear)
                .ToList();

            return Ok(new
            {
                totalStudents = students.Count,
                activeStudents = students.Count(s => s.Status == "Active"),
                byProgram,
                byClassification,
                byRuca,
                byCohort
            });
        }

        [HttpGet("equity-rurality")]
        public IActionResult GetEquityRurality()
        {
            var students = _context.Students.ToList();

            var ruralityByProgram = students
                .GroupBy(s => new { s.ProgramTrack, RucaCategory = s.RucaCategory ?? "Unknown" })
                .Select(g => new
                {
                    programTrack = g.Key.ProgramTrack,
                    rucaCategory = g.Key.RucaCategory,
                    count = g.Count(),
                    averageCumulativeGpa = g.Where(s => s.CumulativeGpa.HasValue).Any()
                        ? Math.Round(g.Where(s => s.CumulativeGpa.HasValue).Average(s => s.CumulativeGpa!.Value), 2)
                        : (decimal?)null,
                    averageScienceGpa = g.Where(s => s.ScienceGpa.HasValue).Any()
                        ? Math.Round(g.Where(s => s.ScienceGpa.HasValue).Average(s => s.ScienceGpa!.Value), 2)
                        : (decimal?)null
                })
                .OrderBy(x => x.programTrack)
                .ThenBy(x => x.rucaCategory)
                .ToList();

            return Ok(new
            {
                ruralityByProgram
            });
        }
    }
}
