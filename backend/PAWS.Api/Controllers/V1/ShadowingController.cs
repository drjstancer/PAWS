using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;
using PAWS.Api.Validation;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/shadowing")]
    public class ShadowingController : ControllerBase
    {
        private readonly PawsDbContext _context;
        private readonly AuditService _audit;

        public ShadowingController(PawsDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [HttpGet]
        [RequirePermission("Shadowing.View")]
        public IActionResult List(string? cycle, string? vettingStatus, string? matchStatus, bool? readyForMatching)
        {
            var query = _context.ShadowingWorkflows.AsQueryable();

            if (!string.IsNullOrWhiteSpace(cycle)) query = query.Where(s => s.ShadowingCycle == cycle);
            if (!string.IsNullOrWhiteSpace(vettingStatus)) query = query.Where(s => s.VettingStatus == vettingStatus);
            if (!string.IsNullOrWhiteSpace(matchStatus)) query = query.Where(s => s.MatchStatus == matchStatus);
            if (readyForMatching.HasValue) query = query.Where(s => s.ReadyForMatching == readyForMatching.Value);

            return Ok(query.ToList());
        }

        [HttpGet("student/{studentId}")]
        [RequirePermission("Shadowing.View")]
        public IActionResult GetByStudent(int studentId)
        {
            return Ok(_context.ShadowingWorkflows.Where(s => s.StudentId == studentId).ToList());
        }

        [HttpPost]
        [RequirePermission("Shadowing.Edit")]
        public IActionResult Create(ShadowingWorkflow workflow)
        {
            var errors = ValidateWorkflow(workflow);
            if (errors.Any()) return BadRequest(ErrorResponses.Validation("Shadowing workflow validation failed", errors.ToArray()));

            workflow.ReadyForMatching = workflow.EligibilityStatus == "Eligible" && workflow.VettingStatus == "Cleared";

            _context.ShadowingWorkflows.Add(workflow);
            _context.SaveChanges();
            _audit.Log("CREATE", "ShadowingWorkflow", null, workflow, workflow.Id.ToString());

            return Ok(workflow);
        }

        [HttpPatch("{id}")]
        [RequirePermission("Shadowing.Edit")]
        public IActionResult Update(int id, ShadowingWorkflow update)
        {
            var existing = _context.ShadowingWorkflows.FirstOrDefault(s => s.Id == id);
            if (existing == null) return NotFound();

            var before = new ShadowingWorkflow
            {
                Id = existing.Id,
                StudentId = existing.StudentId,
                ShadowingCycle = existing.ShadowingCycle,
                EligibilityStatus = existing.EligibilityStatus,
                EligibilityDate = existing.EligibilityDate,
                VettingRequestSubmittedDate = existing.VettingRequestSubmittedDate,
                VettingStatus = existing.VettingStatus,
                HrClearanceReceivedDate = existing.HrClearanceReceivedDate,
                ReadyForMatching = existing.ReadyForMatching,
                MatchStatus = existing.MatchStatus,
                MatchedSpecialty = existing.MatchedSpecialty,
                MatchedProvider = existing.MatchedProvider,
                MatchDate = existing.MatchDate,
                ShadowingCompletedDate = existing.ShadowingCompletedDate,
                Notes = existing.Notes
            };

            existing.EligibilityStatus = update.EligibilityStatus;
            existing.EligibilityDate = update.EligibilityDate;
            existing.VettingRequestSubmittedDate = update.VettingRequestSubmittedDate;
            existing.VettingStatus = update.VettingStatus;
            existing.HrClearanceReceivedDate = update.HrClearanceReceivedDate;
            existing.MatchStatus = update.MatchStatus;
            existing.MatchedSpecialty = update.MatchedSpecialty;
            existing.MatchedProvider = update.MatchedProvider;
            existing.MatchDate = update.MatchDate;
            existing.ShadowingCompletedDate = update.ShadowingCompletedDate;
            existing.Notes = update.Notes;
            existing.ReadyForMatching = existing.EligibilityStatus == "Eligible" && existing.VettingStatus == "Cleared";
            existing.UpdatedAt = DateTime.UtcNow;

            var errors = ValidateWorkflow(existing);
            if (errors.Any()) return BadRequest(ErrorResponses.Validation("Shadowing workflow validation failed", errors.ToArray()));

            _context.SaveChanges();
            _audit.Log("UPDATE", "ShadowingWorkflow", before, existing, existing.Id.ToString());

            return Ok(existing);
        }

        [HttpGet("dashboard")]
        [RequirePermission("Shadowing.View")]
        public IActionResult Dashboard(string cycle)
        {
            var rows = _context.ShadowingWorkflows.Where(s => s.ShadowingCycle == cycle).ToList();
            return Ok(new
            {
                cycle,
                eligible = rows.Count(r => r.EligibilityStatus == "Eligible"),
                submitted = rows.Count(r => r.VettingStatus == "Submitted"),
                inProgress = rows.Count(r => r.VettingStatus == "In Progress"),
                cleared = rows.Count(r => r.VettingStatus == "Cleared"),
                readyForMatching = rows.Count(r => r.ReadyForMatching),
                matched = rows.Count(r => r.MatchStatus == "Matched"),
                completed = rows.Count(r => r.MatchStatus == "Completed" || r.ShadowingCompletedDate != null),
                studentsReadyForMatching = rows.Where(r => r.ReadyForMatching && r.MatchStatus != "Matched" && r.MatchStatus != "Completed")
            });
        }

        private static List<ValidationError> ValidateWorkflow(ShadowingWorkflow workflow)
        {
            var errors = new List<ValidationError>();
            if (workflow.StudentId <= 0) errors.Add(new ValidationError { Field = "StudentId", Issue = "StudentId is required" });
            if (string.IsNullOrWhiteSpace(workflow.ShadowingCycle)) errors.Add(new ValidationError { Field = "ShadowingCycle", Issue = "ShadowingCycle is required" });
            if (workflow.VettingStatus == "Cleared" && workflow.HrClearanceReceivedDate == null) errors.Add(new ValidationError { Field = "HrClearanceReceivedDate", Issue = "Required when VettingStatus is Cleared" });
            if (workflow.MatchStatus == "Matched" && workflow.MatchDate == null) errors.Add(new ValidationError { Field = "MatchDate", Issue = "Required when MatchStatus is Matched" });
            if (workflow.MatchStatus == "Matched" && string.IsNullOrWhiteSpace(workflow.MatchedProvider)) errors.Add(new ValidationError { Field = "MatchedProvider", Issue = "Required when MatchStatus is Matched" });
            return errors;
        }
    }
}
