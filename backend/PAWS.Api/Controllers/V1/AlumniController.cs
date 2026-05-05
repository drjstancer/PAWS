using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/alumni-outcomes")]
    public class AlumniController : ControllerBase
    {
        private readonly PawsDbContext _db;
        private readonly AuditService _audit;

        public AlumniController(PawsDbContext db, AuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        [HttpPost]
        [RequirePermission("Alumni.Edit")]
        public IActionResult Create(AlumniOutcome outcome)
        {
            _db.AlumniOutcomes.Add(outcome);
            _db.SaveChanges();

            _audit.Log("CREATE", "AlumniOutcome", null, outcome, outcome.Id.ToString());

            return Ok(outcome);
        }
    }
}
