using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/shadowing")]
    public class ShadowingController : ControllerBase
    {
        private readonly PawsDbContext _context;
        public ShadowingController(PawsDbContext context) { _context = context; }

        [HttpGet("{studentId}")]
        public IActionResult Get(int studentId)
        {
            return Ok(_context.ShadowingWorkflows.Where(s => s.StudentId == studentId));
        }

        [HttpPost]
        public IActionResult Create(ShadowingWorkflow workflow)
        {
            _context.ShadowingWorkflows.Add(workflow);
            _context.SaveChanges();
            return Ok(workflow);
        }
    }
}
