using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/requirements/bulk")]
    public class BulkRequirementsController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public BulkRequirementsController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpPost("generate")]
        public IActionResult GenerateBulk(string program, string classification, string cycle)
        {
            var students = _context.Students
                .Where(s => s.ProgramTrack == program && s.Classification == classification)
                .ToList();

            foreach (var student in students)
            {
                var applicable = _context.RequirementApplicabilities
                    .Where(r => r.ProgramTrack == program && r.Classification == classification)
                    .ToList();

                foreach (var app in applicable)
                {
                    bool exists = _context.StudentRequirementStatuses.Any(s =>
                        s.StudentId == student.Id &&
                        s.RequirementId == app.RequirementId &&
                        s.RequirementCycle == cycle);

                    if (!exists)
                    {
                        _context.StudentRequirementStatuses.Add(new StudentRequirementStatus
                        {
                            StudentId = student.Id,
                            RequirementId = app.RequirementId,
                            RequirementCycle = cycle,
                            Status = "Not Started"
                        });
                    }
                }
            }

            _context.SaveChanges();
            return Ok("Bulk requirements generated");
        }
    }
}
