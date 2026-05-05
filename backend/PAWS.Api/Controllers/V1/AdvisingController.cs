using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/advising")]
    public class AdvisingController : ControllerBase
    {
        private readonly PawsDbContext _context;
        private readonly AuditService _audit;

        public AdvisingController(PawsDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [HttpGet("{studentId}")]
        [RequirePermission("Advising.View")]
        public IActionResult Get(int studentId)
        {
            return Ok(_context.AdvisingMeetings.Where(a => a.StudentId == studentId));
        }

        [HttpPost]
        [RequirePermission("Advising.Create")]
        public IActionResult Create(AdvisingMeeting meeting)
        {
            if (meeting.FollowUpNeeded && meeting.FollowUpDate == null)
                return BadRequest("FollowUpDate required when FollowUpNeeded = true");

            _context.AdvisingMeetings.Add(meeting);
            _context.SaveChanges();

            _audit.Log("CREATE", "AdvisingMeeting", null, meeting, meeting.Id.ToString());

            return Ok(meeting);
        }
    }
}
