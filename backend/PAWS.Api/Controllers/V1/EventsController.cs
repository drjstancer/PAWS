using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;
using PAWS.Api.Validation;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly PawsDbContext _context;
        private readonly AuditService _audit;

        public EventsController(PawsDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [HttpGet]
        [RequirePermission("Events.View")]
        public IActionResult Get(string? category, bool? required)
        {
            var query = _context.Events.AsQueryable();
            if (!string.IsNullOrWhiteSpace(category)) query = query.Where(e => e.EventCategory == category);
            if (required.HasValue) query = query.Where(e => e.Required == required.Value);
            return Ok(query.OrderByDescending(e => e.EventDate).ToList());
        }

        [HttpPost]
        [RequirePermission("Events.Edit")]
        public IActionResult Create(Event evt)
        {
            var errors = ValidateEvent(evt);
            if (errors.Any()) return BadRequest(ErrorResponses.Validation("Event validation failed", errors.ToArray()));

            _context.Events.Add(evt);
            _context.SaveChanges();
            _audit.Log("CREATE", "Event", null, evt, evt.Id.ToString());
            return Ok(evt);
        }

        [HttpPatch("{id}")]
        [RequirePermission("Events.Edit")]
        public IActionResult Update(int id, Event update)
        {
            var existing = _context.Events.FirstOrDefault(e => e.Id == id);
            if (existing == null) return NotFound();

            var errors = ValidateEvent(update);
            if (errors.Any()) return BadRequest(ErrorResponses.Validation("Event validation failed", errors.ToArray()));

            var before = new Event
            {
                Id = existing.Id,
                EventName = existing.EventName,
                EventCategory = existing.EventCategory,
                EventDate = existing.EventDate,
                StartTime = existing.StartTime,
                EndTime = existing.EndTime,
                Location = existing.Location,
                Required = existing.Required,
                RelatedRequirementId = existing.RelatedRequirementId,
                Notes = existing.Notes
            };

            existing.EventName = update.EventName;
            existing.EventCategory = update.EventCategory;
            existing.EventDate = update.EventDate;
            existing.StartTime = update.StartTime;
            existing.EndTime = update.EndTime;
            existing.Location = update.Location;
            existing.Required = update.Required;
            existing.RelatedRequirementId = update.RelatedRequirementId;
            existing.Notes = update.Notes;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
            _audit.Log("UPDATE", "Event", before, existing, existing.Id.ToString());
            return Ok(existing);
        }

        private static List<ValidationError> ValidateEvent(Event evt)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(evt.EventName)) errors.Add(new ValidationError { Field = "EventName", Issue = "Event name is required" });
            if (string.IsNullOrWhiteSpace(evt.EventCategory)) errors.Add(new ValidationError { Field = "EventCategory", Issue = "Event category is required" });
            if (evt.EventDate == default) errors.Add(new ValidationError { Field = "EventDate", Issue = "Event date is required" });
            if (evt.StartTime.HasValue && evt.EndTime.HasValue && evt.EndTime <= evt.StartTime) errors.Add(new ValidationError { Field = "EndTime", Issue = "End time must be after start time" });
            return errors;
        }
    }
}
