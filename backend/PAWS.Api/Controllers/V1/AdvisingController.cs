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
        private readonly ICurrentUserService _currentUser;

        public AdvisingController(PawsDbContext context, AuditService audit, ICurrentUserService currentUser)
        {
            _context = context;
            _audit = audit;
            _currentUser = currentUser;
        }

        [HttpGet("{studentId}")]
        [RequirePermission("Advising.View")]
        public IActionResult Get(int studentId)
        {
            var canViewRestricted = _currentUser.User.Permissions.Contains("Advising.ViewRestricted");

            var meetings = _context.AdvisingMeetings
                .Where(a => a.StudentId == studentId)
                .ToList();

            if (!canViewRestricted)
            {
                meetings = meetings
                    .Where(a => !a.RestrictedNote)
                    .ToList();
            }
            else
            {
                _audit.Log("VIEW_RESTRICTED", "AdvisingMeeting", null, new { studentId }, studentId.ToString());
            }

            return Ok(meetings);
        }

        [HttpPost]
        [RequirePermission("Advising.Create")]
        public IActionResult Create(AdvisingMeeting meeting)
        {
            if (meeting.FollowUpNeeded && meeting.FollowUpDate == null)
                return BadRequest("FollowUpDate required when FollowUpNeeded = true");

            if (meeting.RestrictedNote && !_currentUser.User.Permissions.Contains("Advising.ViewRestricted"))
                return Forbid();

            _context.AdvisingMeetings.Add(meeting);
            _context.SaveChanges();

            _audit.Log("CREATE", "AdvisingMeeting", null, meeting, meeting.Id.ToString());

            return Ok(meeting);
        }
    }
}
