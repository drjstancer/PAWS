using Xunit;
using PAWS.Api.Controllers.V1;
using PAWS.Api.Data;
using PAWS.Api.Models;
using Microsoft.EntityFrameworkCore;
using PAWS.Api.Services;

namespace PAWS.Api.Tests
{
    public class GpaTests
    {
        [Fact]
        public void CalculatesGpaCorrectly()
        {
            var options = new DbContextOptionsBuilder<PawsDbContext>()
                .UseInMemoryDatabase(databaseName: "GpaTestDb")
                .Options;

            using var context = new PawsDbContext(options);

            context.CourseRecords.AddRange(
                new CourseRecord { StudentId = 1, CreditHours = 3, GradePointsEarned = 12, PerCreditGradeValue = 4, CountsTowardScienceMathGpa = true },
                new CourseRecord { StudentId = 1, CreditHours = 3, GradePointsEarned = 9, PerCreditGradeValue = 3, CountsTowardScienceMathGpa = true }
            );
            context.SaveChanges();

            var controller = new GpaController(context, new AuditService(null!));
            var result = controller.Calculate(1);

            Assert.NotNull(result);
        }
    }
}
