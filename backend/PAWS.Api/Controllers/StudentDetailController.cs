using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/student-detail")]
    public class StudentDetailController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public StudentDetailController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentDetail(int id)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();

            var requirements = _context.StudentRequirementStatuses
                .Where(r => r.StudentId == id)
                .ToList();

            var completed = requirements.Count(r => r.Status == "Completed" || r.Status == "Waived");

            return Ok(new
            {
                student,
                requirementSummary = new
                {
                    total = requirements.Count,
                    completed,
                    complianceRate = requirements.Count == 0 ? 0 : (int)((double)completed / requirements.Count * 100)
                }
            });
        }
    }
}
