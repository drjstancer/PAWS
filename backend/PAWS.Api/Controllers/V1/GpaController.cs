using Microsoft.AspNetCore.Mvc;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;
using PAWS.Api.Services;

namespace PAWS.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v1/gpa")]
    public class GpaController : ControllerBase
    {
        private readonly PawsDbContext _context;
        private readonly AuditService _audit;

        public GpaController(PawsDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [HttpGet("calculate/{studentId}")]
        [RequirePermission("Academic.View")]
        public IActionResult Calculate(int studentId)
        {
            var courses = _context.CourseRecords
                .Where(c => c.StudentId == studentId)
                .ToList();

            if (!courses.Any())
                return Ok(new GpaCalculationResultDto { StudentId = studentId, Warnings = new List<string> { "No course records found" } });

            decimal totalCredits = 0;
            decimal totalPoints = 0;
            decimal sciCredits = 0;
            decimal sciPoints = 0;
            int totalCount = 0;
            int sciCount = 0;

            var warnings = new List<string>();

            foreach (var c in courses)
            {
                var courseLabel = $"{c.CourseSubject} {c.CourseNumber}".Trim();

                if (c.CreditHours <= 0)
                {
                    warnings.Add($"Course {courseLabel} excluded due to invalid credit hours");
                    continue;
                }

                decimal? weightedGradePoints = c.GradePointsEarned;
                if (weightedGradePoints == null && c.PerCreditGradeValue != null)
                    weightedGradePoints = c.PerCreditGradeValue.Value * c.CreditHours;

                if (weightedGradePoints == null)
                {
                    warnings.Add($"Course {courseLabel} excluded due to missing grade points");
                    continue;
                }

                totalCredits += c.CreditHours;
                totalPoints += weightedGradePoints.Value;
                totalCount++;

                if (c.CountsTowardScienceMathGpa)
                {
                    sciCredits += c.CreditHours;
                    sciPoints += weightedGradePoints.Value;
                    sciCount++;
                }
            }

            var result = new GpaCalculationResultDto
            {
                StudentId = studentId,
                TotalAttemptedCredits = totalCredits,
                TotalGradePoints = totalPoints,
                CumulativeGpa = totalCredits == 0 ? null : Math.Round(totalPoints / totalCredits, 3),
                ScienceMathAttemptedCredits = sciCredits,
                ScienceMathGradePoints = sciPoints,
                ScienceMathGpa = sciCredits == 0 ? null : Math.Round(sciPoints / sciCredits, 3),
                IncludedCourseCount = totalCount,
                ScienceMathCourseCount = sciCount,
                Warnings = warnings
            };

            return Ok(result);
        }

        [HttpPost("calculate-and-sync")]
        [RequirePermission("Academic.Edit")]
        public IActionResult CalculateAndSync(GpaSyncRequestDto request)
        {
            var calcResult = Calculate(request.StudentId) as OkObjectResult;
            if (calcResult == null) return BadRequest();

            var result = calcResult.Value as GpaCalculationResultDto;
            if (result == null) return BadRequest();

            if (request.SyncToStudentProfile)
            {
                var student = _context.Students.FirstOrDefault(s => s.Id == request.StudentId);
                if (student != null)
                {
                    student.CumulativeGpa = result.CumulativeGpa;
                    student.ScienceGpa = result.ScienceMathGpa;
                }
            }

            if (request.CreateAcademicRecord)
            {
                _context.AcademicRecords.Add(new AcademicRecord
                {
                    StudentId = request.StudentId,
                    AcademicYear = request.AcademicYear,
                    Term = request.Term,
                    CumulativeGpa = result.CumulativeGpa,
                    ScienceMathGpa = result.ScienceMathGpa,
                    GpaDataSource = "CourseRecord Calculation"
                });
            }

            _context.SaveChanges();
            _audit.Log("CALCULATE_GPA", "CourseRecord", null, result, request.StudentId.ToString());

            return Ok(result);
        }
    }
}
