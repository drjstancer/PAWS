using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/advising")]
    public class AdvisingController : ControllerBase
    {
        private readonly PawsDbContext _context;
        public AdvisingController(PawsDbContext context) { _context = context; }

        [HttpGet("{studentId}")]
        public IActionResult Get(int studentId)
        {
            return Ok(_context.AdvisingMeetings.Where(a => a.StudentId == studentId));
        }

        [HttpPost]
        public IActionResult Create(AdvisingMeeting meeting)
        {
            if (meeting.FollowUpNeeded && meeting.FollowUpDate == null)
                return BadRequest("FollowUpDate required when FollowUpNeeded = true");

            _context.AdvisingMeetings.Add(meeting);
            _context.SaveChanges();
            return Ok(meeting);
        }
    }
}
