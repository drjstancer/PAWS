using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/import")]
    public class ImportController : ControllerBase
    {
        private readonly PawsDbContext _context;

        public ImportController(PawsDbContext context)
        {
            _context = context;
        }

        [HttpPost("students")]
        public IActionResult ImportStudents(List<StudentImportDto> records)
        {
            var result = new ImportResultDto();

            foreach (var r in records)
            {
                try
                {
                    var existing = _context.Students.FirstOrDefault(s => s.MuId == r.MuId);

                    if (existing == null)
                    {
                        _context.Students.Add(new Student
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
                            HtmAdvisor = r.HtmAdvisor
                        });
                        result.Created++;
                    }
                    else
                    {
                        existing.ProgramTrack = r.ProgramTrack;
                        existing.Classification = r.Classification;
                        existing.CumulativeGpa = r.CumulativeGpa;
                        existing.ScienceGpa = r.ScienceGpa;
                        result.Updated++;
                    }
                }
                catch (Exception ex)
                {
                    result.Rejected++;
                    result.Errors.Add(ex.Message);
                }
            }

            _context.SaveChanges();
            return Ok(result);
        }
    }
}
