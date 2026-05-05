using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/academic-records")]
    public class AcademicController : ControllerBase
    {
        private readonly PawsDbContext _context;
        public AcademicController(PawsDbContext context) { _context = context; }

        [HttpGet("{studentId}")]
        public IActionResult Get(int studentId)
        {
            return Ok(_context.AcademicRecords.Where(a => a.StudentId == studentId));
        }

        [HttpPost]
        public IActionResult Create(AcademicRecord record)
        {
            if (string.IsNullOrEmpty(record.AcademicYear) || string.IsNullOrEmpty(record.Term))
                return BadRequest("AcademicYear and Term required");

            _context.AcademicRecords.Add(record);
            _context.SaveChanges();
            return Ok(record);
        }
    }
}
