using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly PawsDbContext _context;
        public EventsController(PawsDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult Get() => Ok(_context.Events);

        [HttpPost]
        public IActionResult Create(Event evt)
        {
            _context.Events.Add(evt);
            _context.SaveChanges();
            return Ok(evt);
        }
    }
}
