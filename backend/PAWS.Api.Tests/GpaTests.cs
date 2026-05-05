using Xunit;
using PAWS.Api.Controllers.V1;
using PAWS.Api.Data;
using PAWS.Api.Models;
using Microsoft.EntityFrameworkCore;
using PAWS.Api.Services;
using PAWS.Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace PAWS.Api.Tests
{
    public class GpaTests
    {
        [Fact]
        public void CalculatesGpaCorrectly()
        {
            var options = new DbContextOptionsBuilder<PawsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new PawsDbContext(options);
            var currentUser = new CurrentUserService { User = new CurrentUser { AppUserId = 1, Email = "tester@missouri.edu" } };
            var audit = new AuditService(context, currentUser);

            context.CourseRecords.AddRange(
                new CourseRecord { StudentId = 1, AcademicYear = "2025-2026", Term = "Fall", CourseSubject = "BIO", CourseNumber = "1500", CreditHours = 3, GradePointsEarned = 12, PerCreditGradeValue = 4, CountsTowardScienceMathGpa = true },
                new CourseRecord { StudentId = 1, AcademicYear = "2025-2026", Term = "Fall", CourseSubject = "CHEM", CourseNumber = "1100", CreditHours = 3, GradePointsEarned = 9, PerCreditGradeValue = 3, CountsTowardScienceMathGpa = true }
            );
            context.SaveChanges();

            var controller = new GpaController(context, audit);
            var result = controller.Calculate(1) as OkObjectResult;
            var data = Assert.IsType<GpaCalculationResultDto>(result!.Value);

            Assert.Equal(6, data.TotalAttemptedCredits);
            Assert.Equal(21, data.TotalGradePoints);
            Assert.Equal(3.500m, data.CumulativeGpa);
            Assert.Equal(3.500m, data.ScienceMathGpa);
            Assert.Equal(2, data.IncludedCourseCount);
            Assert.Empty(data.Warnings);
        }
    }
}
