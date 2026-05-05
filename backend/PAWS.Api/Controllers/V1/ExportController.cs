using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Security;
using PAWS.Api.Services;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/export")]
    public class ExportController : ControllerBase
    {
        private readonly PawsDbContext _db;
        private readonly CsvExportService _csv;
        private readonly XlsxExportService _xlsx;
        private readonly AuditService _audit;

        public ExportController(PawsDbContext db, CsvExportService csv, XlsxExportService xlsx, AuditService audit)
        {
            _db = db;
            _csv = csv;
            _xlsx = xlsx;
            _audit = audit;
        }

        [HttpGet("students")]
        [RequirePermission("Reports.Export")]
        public IActionResult Students()
        {
            var file = _csv.Students(_db);
            _audit.Log("EXPORT", "Students", null, new { count = file.Length });
            return File(file, "text/csv", "students.csv");
        }

        [HttpGet("compliance")]
        [RequirePermission("Reports.Export")]
        public IActionResult Compliance(string cycle)
        {
            var file = _csv.Compliance(_db, cycle);
            _audit.Log("EXPORT", "Compliance", null, new { cycle });
            return File(file, "text/csv", $"compliance_{cycle}.csv");
        }

        [HttpGet("courses")]
        [RequirePermission("Reports.Export")]
        public IActionResult Courses()
        {
            var file = _csv.Courses(_db);
            _audit.Log("EXPORT", "Courses", null, null);
            return File(file, "text/csv", "courses.csv");
        }

        [HttpGet("shadowing")]
        [RequirePermission("Reports.Export")]
        public IActionResult Shadowing(string cycle)
        {
            var file = _csv.Shadowing(_db, cycle);
            _audit.Log("EXPORT", "Shadowing", null, new { cycle });
            return File(file, "text/csv", $"shadowing_{cycle}.csv");
        }

        [HttpGet("alumni")]
        [RequirePermission("Reports.Export")]
        public IActionResult Alumni()
        {
            var file = _csv.Alumni(_db);
            _audit.Log("EXPORT", "Alumni", null, null);
            return File(file, "text/csv", "alumni.csv");
        }

        [HttpGet("workbook")]
        [RequirePermission("Reports.Export")]
        public IActionResult Workbook(string? cycle)
        {
            var file = _xlsx.FullWorkbook(_db, cycle);
            _audit.Log("EXPORT", "Workbook", null, new { cycle });
            var fileName = $"PAWS_Workbook_{cycle ?? "All"}_{DateTime.UtcNow:yyyyMMdd}.xlsx";
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
