using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Services;
using PAWS.Api.Security;

namespace PAWS.Api.Controllers
{
    [ApiController]
    [Route("api/reports/export")]
    public class ReportExportController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly AuditService _audit;

        public ReportExportController(ReportService reportService, AuditService audit)
        {
            _reportService = reportService;
            _audit = audit;
        }

        [HttpGet("faculty")]
        [RequirePermission("Reports.Export")]
        public IActionResult ExportFacultyReport(string cycle)
        {
            var pdf = _reportService.GenerateFacultyReport(cycle);

            _audit.Log("EXPORT", "FacultyReport", null, new { cycle });

            return File(pdf, "application/pdf", $"PAWS_Report_{cycle}.pdf");
        }
    }
}
