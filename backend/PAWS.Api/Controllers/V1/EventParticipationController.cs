using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/event-participation")]
    public class EventParticipationController : ControllerBase
    {
        private readonly PawsDbContext _db;
        private readonly AuditService _audit;

        public EventParticipationController(PawsDbContext db, AuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        [HttpPost]
        [RequirePermission("Events.Edit")]
        public IActionResult Record(StudentEventParticipation participation)
        {
            _db.StudentEventParticipations.Add(participation);
            _db.SaveChanges();

            _audit.Log("CREATE", "EventParticipation", null, participation, participation.Id.ToString());

            return Ok(participation);
        }
    }
}
