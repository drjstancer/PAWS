using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/requirements")]
    public class RequirementsController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public RequirementsController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpPost("generate/{studentId}")]
        public IActionResult GenerateRequirements(int studentId)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null) return NotFound();

            var applicable = _context.RequirementApplicabilities
                .Where(r => r.ProgramTrack == student.ProgramTrack && r.Classification == student.Classification)
                .ToList();

            foreach (var app in applicable)
            {
                bool exists = _context.StudentRequirementStatuses
                    .Any(s => s.StudentId == studentId && s.RequirementId == app.RequirementId);

                if (!exists)
                {
                    _context.StudentRequirementStatuses.Add(new StudentRequirementStatus
                    {
                        StudentId = studentId,
                        RequirementId = app.RequirementId,
                        Status = "Not Started"
                    });
                }
            }

            _context.SaveChanges();
            return Ok("Requirements generated");
        }
    }
}
