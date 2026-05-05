using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;
using PAWS.Api.Validation;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/course-records")]
    public class CourseController : ControllerBase
    {
        private readonly PawsDbContext _db;
        private readonly AuditService _audit;

        public CourseController(PawsDbContext db, AuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        [HttpGet("{studentId}")]
        [RequirePermission("Academic.View")]
        public IActionResult Get(int studentId)
        {
            return Ok(_db.CourseRecords.Where(c => c.StudentId == studentId));
        }

        [HttpPost]
        [RequirePermission("Academic.Edit")]
        public IActionResult Create(CourseRecord record)
        {
            if (record.CreditHours <= 0)
                return BadRequest(ErrorResponses.Validation("Invalid credit hours",
                    new ValidationError { Field = "CreditHours", Issue = "Must be greater than 0" }));

            _db.CourseRecords.Add(record);
            _db.SaveChanges();

            _audit.Log("CREATE", "CourseRecord", null, record, record.Id.ToString());

            return Ok(record);
        }
    }
}
