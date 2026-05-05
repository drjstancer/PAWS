using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public StudentsController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
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
        public IActionResult CreateStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
            return Ok(student);
        }
    }
}
