using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly PawsDbContext _context;
        private readonly AuditService _audit;

        public StudentsController(PawsDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [HttpGet]
        [RequirePermission("Students.View")]
        public IActionResult GetStudents(string? program, string? classification)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(program))
                query = query.Where(s => s.ProgramTrack == program);

            if (!string.IsNullOrEmpty(classification))
                query = query.Where(s => s.Classification == classification);

            return Ok(query.ToList());
        }

        [HttpPost]
        [RequirePermission("Students.Edit")]
        public IActionResult CreateStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();

            _audit.Log("CREATE", "Student", null, student, student.Id.ToString());

            return Ok(student);
        }
    }
}
