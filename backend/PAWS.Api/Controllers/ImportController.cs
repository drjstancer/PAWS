using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;
using PAWS.Api.Validation;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/v1/import")]
    public class ImportController : ControllerBase
    {
        private readonly PawsDbContext _context;
        private readonly AuditService _audit;

        public ImportController(PawsDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [HttpPost("students")]
        [RequirePermission("Students.Edit")]
        public IActionResult ImportStudents(List<StudentImportDto> records)
        {
            if (records == null || records.Count == 0)
                return BadRequest(ErrorResponses.Validation("Import payload is empty", new ValidationError { Field = "records", Issue = "At least one student record is required" }));

            if (records.Count > 1000)
                return BadRequest(ErrorResponses.Validation("Import batch is too large", new ValidationError { Field = "records", Issue = "Maximum batch size is 1000 records" }));

            var result = new ImportResultDto();

            foreach (var r in records)
            {
                var errors = ValidateRecord(r);
                if (errors.Any())
                {
                    result.Rejected++;
                    result.Errors.Add($"{r.MuId}: {string.Join("; ", errors.Select(e => e.Field + " - " + e.Issue))}");
                    continue;
                }

                try
                {
                    var existing = _context.Students.FirstOrDefault(s => s.MuId == r.MuId);
                    var rucaCategory = r.RucaCode.HasValue ? (r.RucaCode.Value <= 3 ? "Urban/Metropolitan" : "Rural/Nonmetropolitan") : null;

                    if (existing == null)
                    {
                        var student = new Student
                        {
                            MuId = r.MuId,
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            Email = r.Email,
                            ProgramTrack = r.ProgramTrack,
                            Classification = r.Classification,
                            CohortYear = r.CohortYear,
                            CumulativeGpa = r.CumulativeGpa,
                            ScienceGpa = r.ScienceGpa,
                            RucaCode = r.RucaCode,
                            RucaCategory = rucaCategory,
                            HtmAdvisor = r.HtmAdvisor,
                            Status = "Active"
                        };
                        _context.Students.Add(student);
                        result.Created++;
                    }
                    else
                    {
                        var before = new { existing.ProgramTrack, existing.Classification, existing.CumulativeGpa, existing.ScienceGpa, existing.RucaCode, existing.RucaCategory };
                        existing.ProgramTrack = r.ProgramTrack;
                        existing.Classification = r.Classification;
                        existing.CumulativeGpa = r.CumulativeGpa;
                        existing.ScienceGpa = r.ScienceGpa;
                        existing.RucaCode = r.RucaCode;
                        existing.RucaCategory = rucaCategory;
                        existing.HtmAdvisor = r.HtmAdvisor;
                        result.Updated++;
                    }
                }
                catch (Exception ex)
                {
                    result.Rejected++;
                    result.Errors.Add($"{r.MuId}: {ex.Message}");
                }
            }

            _context.SaveChanges();
            _audit.Log("IMPORT", "Students", null, new { result.Created, result.Updated, result.Rejected });
            return Ok(result);
        }

        private static List<ValidationError> ValidateRecord(StudentImportDto r)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(r.MuId)) errors.Add(new ValidationError { Field = "MuId", Issue = "MU ID is required" });
            if (string.IsNullOrWhiteSpace(r.FirstName)) errors.Add(new ValidationError { Field = "FirstName", Issue = "First name is required" });
            if (string.IsNullOrWhiteSpace(r.LastName)) errors.Add(new ValidationError { Field = "LastName", Issue = "Last name is required" });
            if (string.IsNullOrWhiteSpace(r.ProgramTrack)) errors.Add(new ValidationError { Field = "ProgramTrack", Issue = "Program track is required" });
            if (string.IsNullOrWhiteSpace(r.Classification)) errors.Add(new ValidationError { Field = "Classification", Issue = "Classification is required" });
            if (r.CohortYear < 2000 || r.CohortYear > DateTime.UtcNow.Year + 2) errors.Add(new ValidationError { Field = "CohortYear", Issue = "Cohort year is outside valid range" });
            if (r.RucaCode.HasValue && (r.RucaCode < 1 || r.RucaCode > 10)) errors.Add(new ValidationError { Field = "RucaCode", Issue = "RUCA code must be 1-10" });
            if (r.CumulativeGpa.HasValue && (r.CumulativeGpa < 0 || r.CumulativeGpa > 4)) errors.Add(new ValidationError { Field = "CumulativeGpa", Issue = "GPA must be between 0 and 4" });
            if (r.ScienceGpa.HasValue && (r.ScienceGpa < 0 || r.ScienceGpa > 4)) errors.Add(new ValidationError { Field = "ScienceGpa", Issue = "Science GPA must be between 0 and 4" });
            return errors;
        }
    }
}
