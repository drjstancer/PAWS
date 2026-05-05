using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;
using PAWS.Api.Validation;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/v1/requirements")]
    public class RequirementsController : ControllerBase
    {
        private readonly PawsDbContext _context;
        private readonly AuditService _audit;

        public RequirementsController(PawsDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [HttpGet]
        [RequirePermission("Requirements.View")]
        public IActionResult List(bool activeOnly = true)
        {
            var query = _context.Requirements.AsQueryable();
            // Requirement currently has no Active property in the model. Keep activeOnly parameter for future parity.
            return Ok(query.OrderBy(r => r.Category).ThenBy(r => r.Name).ToList());
        }

        [HttpPost]
        [RequirePermission("Requirements.Edit")]
        public IActionResult CreateRequirement(Requirement requirement)
        {
            var errors = ValidateRequirement(requirement);
            if (errors.Any()) return BadRequest(ErrorResponses.Validation("Requirement validation failed", errors.ToArray()));

            if (_context.Requirements.Any(r => r.Name == requirement.Name))
                return BadRequest(ErrorResponses.Validation("Requirement already exists", new ValidationError { Field = "Name", Issue = "Requirement names must be unique" }));

            _context.Requirements.Add(requirement);
            _context.SaveChanges();
            _audit.Log("CREATE", "Requirement", null, requirement, requirement.Id.ToString());
            return Ok(requirement);
        }

        [HttpPatch("{id}")]
        [RequirePermission("Requirements.Edit")]
        public IActionResult UpdateRequirement(int id, Requirement update)
        {
            var existing = _context.Requirements.FirstOrDefault(r => r.Id == id);
            if (existing == null) return NotFound();

            var errors = ValidateRequirement(update);
            if (errors.Any()) return BadRequest(ErrorResponses.Validation("Requirement validation failed", errors.ToArray()));

            if (_context.Requirements.Any(r => r.Id != id && r.Name == update.Name))
                return BadRequest(ErrorResponses.Validation("Requirement already exists", new ValidationError { Field = "Name", Issue = "Requirement names must be unique" }));

            var before = new Requirement { Id = existing.Id, Name = existing.Name, Category = existing.Category, Required = existing.Required };
            existing.Name = update.Name;
            existing.Category = update.Category;
            existing.Required = update.Required;

            _context.SaveChanges();
            _audit.Log("UPDATE", "Requirement", before, existing, existing.Id.ToString());
            return Ok(existing);
        }

        [HttpGet("applicability")]
        [RequirePermission("Requirements.View")]
        public IActionResult ListApplicability(int? requirementId)
        {
            var query = _context.RequirementApplicabilities.AsQueryable();
            if (requirementId.HasValue) query = query.Where(a => a.RequirementId == requirementId.Value);
            return Ok(query.OrderBy(a => a.ProgramTrack).ThenBy(a => a.Classification).ToList());
        }

        [HttpPost("applicability")]
        [RequirePermission("Requirements.Edit")]
        public IActionResult CreateApplicability(RequirementApplicability applicability)
        {
            var errors = ValidateApplicability(applicability);
            if (errors.Any()) return BadRequest(ErrorResponses.Validation("Requirement applicability validation failed", errors.ToArray()));

            if (!_context.Requirements.Any(r => r.Id == applicability.RequirementId))
                return BadRequest(ErrorResponses.Validation("Requirement not found", new ValidationError { Field = "RequirementId", Issue = "RequirementId must reference an existing requirement" }));

            var exists = _context.RequirementApplicabilities.Any(a =>
                a.RequirementId == applicability.RequirementId &&
                a.ProgramTrack == applicability.ProgramTrack &&
                a.Classification == applicability.Classification &&
                a.Active);

            if (exists)
                return BadRequest(ErrorResponses.Validation("Applicability rule already exists", new ValidationError { Field = "Applicability", Issue = "Duplicate active applicability rule" }));

            _context.RequirementApplicabilities.Add(applicability);
            _context.SaveChanges();
            _audit.Log("CREATE", "RequirementApplicability", null, applicability, applicability.Id.ToString());
            return Ok(applicability);
        }

        [HttpPatch("student-status/{id}")]
        [RequirePermission("Requirements.Edit")]
        public IActionResult UpdateStudentRequirementStatus(int id, StudentRequirementStatus update)
        {
            var existing = _context.StudentRequirementStatuses.FirstOrDefault(s => s.Id == id);
            if (existing == null) return NotFound();

            var errors = ValidateStudentRequirementStatus(update);
            if (errors.Any()) return BadRequest(ErrorResponses.Validation("Student requirement status validation failed", errors.ToArray()));

            var before = new StudentRequirementStatus
            {
                Id = existing.Id,
                StudentId = existing.StudentId,
                RequirementId = existing.RequirementId,
                RequirementCycle = existing.RequirementCycle,
                Status = existing.Status,
                CompletionDate = existing.CompletionDate,
                Notes = existing.Notes
            };

            existing.Status = update.Status;
            existing.CompletionDate = update.CompletionDate;
            existing.Notes = update.Notes;

            _context.SaveChanges();
            _audit.Log("UPDATE", "StudentRequirementStatus", before, existing, existing.Id.ToString());
            return Ok(existing);
        }

        [HttpPost("generate/{studentId}")]
        [RequirePermission("Requirements.Generate")]
        public IActionResult GenerateRequirements(int studentId, string cycle)
        {
            if (string.IsNullOrWhiteSpace(cycle))
                return BadRequest(ErrorResponses.Validation("Cycle is required", new ValidationError { Field = "cycle", Issue = "Cycle must be provided" }));

            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
            if (student == null) return NotFound();

            var applicable = _context.RequirementApplicabilities
                .Where(r => r.ProgramTrack == student.ProgramTrack && r.Classification == student.Classification && r.Active)
                .ToList();

            var created = 0;
            foreach (var app in applicable)
            {
                bool exists = _context.StudentRequirementStatuses.Any(s =>
                    s.StudentId == studentId &&
                    s.RequirementId == app.RequirementId &&
                    s.RequirementCycle == cycle);

                if (!exists)
                {
                    _context.StudentRequirementStatuses.Add(new StudentRequirementStatus
                    {
                        StudentId = studentId,
                        RequirementId = app.RequirementId,
                        RequirementCycle = cycle,
                        Status = "Not Started"
                    });
                    created++;
                }
            }

            _context.SaveChanges();
            _audit.Log("GENERATE", "StudentRequirementStatus", null, new { studentId, cycle, created }, studentId.ToString());
            return Ok(new { studentId, cycle, created });
        }

        [HttpPost("bulk-generate")]
        [RequirePermission("Requirements.Generate")]
        public IActionResult BulkGenerate(string program, string classification, string cycle)
        {
            if (string.IsNullOrWhiteSpace(program) || string.IsNullOrWhiteSpace(classification) || string.IsNullOrWhiteSpace(cycle))
                return BadRequest(ErrorResponses.Validation("Program, classification, and cycle are required"));

            var students = _context.Students
                .Where(s => s.ProgramTrack == program && s.Classification == classification)
                .ToList();

            var applicable = _context.RequirementApplicabilities
                .Where(r => r.ProgramTrack == program && r.Classification == classification && r.Active)
                .ToList();

            var created = 0;
            foreach (var student in students)
            {
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
                        created++;
                    }
                }
            }

            _context.SaveChanges();
            _audit.Log("BULK_GENERATE", "StudentRequirementStatus", null, new { program, classification, cycle, students = students.Count, created });
            return Ok(new { program, classification, cycle, students = students.Count, created });
        }

        private static List<ValidationError> ValidateRequirement(Requirement requirement)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(requirement.Name)) errors.Add(new ValidationError { Field = "Name", Issue = "Name is required" });
            if (string.IsNullOrWhiteSpace(requirement.Category)) errors.Add(new ValidationError { Field = "Category", Issue = "Category is required" });
            return errors;
        }

        private static List<ValidationError> ValidateApplicability(RequirementApplicability applicability)
        {
            var errors = new List<ValidationError>();
            if (applicability.RequirementId <= 0) errors.Add(new ValidationError { Field = "RequirementId", Issue = "RequirementId is required" });
            if (string.IsNullOrWhiteSpace(applicability.ProgramTrack)) errors.Add(new ValidationError { Field = "ProgramTrack", Issue = "ProgramTrack is required" });
            if (string.IsNullOrWhiteSpace(applicability.Classification)) errors.Add(new ValidationError { Field = "Classification", Issue = "Classification is required" });
            return errors;
        }

        private static List<ValidationError> ValidateStudentRequirementStatus(StudentRequirementStatus status)
        {
            var errors = new List<ValidationError>();
            var allowed = new[] { "Not Started", "In Progress", "Completed", "Waived", "Not Applicable" };
            if (!allowed.Contains(status.Status)) errors.Add(new ValidationError { Field = "Status", Issue = "Invalid requirement status" });
            if (status.Status == "Completed" && status.CompletionDate == null) errors.Add(new ValidationError { Field = "CompletionDate", Issue = "CompletionDate is required when status is Completed" });
            if (status.Status == "Waived" && string.IsNullOrWhiteSpace(status.Notes)) errors.Add(new ValidationError { Field = "Notes", Issue = "Notes are required when status is Waived" });
            return errors;
        }
    }
}
