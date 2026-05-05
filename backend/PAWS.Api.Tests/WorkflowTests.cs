using Xunit;
using PAWS.Api.Data;
using PAWS.Api.Models;
using Microsoft.EntityFrameworkCore;
using PAWS.Api.Controllers.V1;
using PAWS.Api.Services;
using PAWS.Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace PAWS.Api.Tests
{
    public class WorkflowTests
    {
        [Fact]
        public void FullWorkflow_ShouldExecuteSuccessfully()
        {
            var options = new DbContextOptionsBuilder<PawsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new PawsDbContext(options);
            var currentUser = new CurrentUserService { User = new CurrentUser { AppUserId = 1, Email = "tester@missouri.edu" } };
            var audit = new AuditService(context, currentUser);

            var student = new Student { Id = 1, FirstName = "Joel", LastName = "Stancer", ProgramTrack = "PAWS", Classification = "Junior" };
            context.Students.Add(student);

            context.CourseRecords.Add(new CourseRecord
            {
                StudentId = 1,
                AcademicYear = "2025-2026",
                Term = "Fall",
                CourseSubject = "BIO",
                CourseNumber = "1500",
                CreditHours = 3,
                GradePointsEarned = 12,
                CountsTowardScienceMathGpa = true
            });

            context.Requirements.Add(new Requirement { Id = 1, Name = "Retreat", Category = "Program" });
            context.RequirementApplicabilities.Add(new RequirementApplicability { RequirementId = 1, ProgramTrack = "PAWS", Classification = "Junior", Active = true });

            context.SaveChanges();

            var gpaController = new GpaController(context, audit);
            var gpaResult = gpaController.Calculate(1) as OkObjectResult;
            Assert.NotNull(gpaResult);

            var reqController = new PAWS.Api.Controllers.RequirementsController(context, audit);
            var genResult = reqController.GenerateRequirements(1, "2025-2026") as OkObjectResult;
            Assert.NotNull(genResult);

            var exportService = new CsvExportService();
            var csv = exportService.Students(context);
            Assert.NotEmpty(csv);

            var xlsxService = new XlsxExportService();
            var xlsx = xlsxService.FullWorkbook(context, "2025-2026");
            Assert.NotEmpty(xlsx);
        }
    }
}
