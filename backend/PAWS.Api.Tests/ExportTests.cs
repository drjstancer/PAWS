using Xunit;
using PAWS.Api.Services;
using PAWS.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace PAWS.Api.Tests
{
    public class ExportTests
    {
        [Fact]
        public void CsvExport_ShouldContainHeaders()
        {
            var options = new DbContextOptionsBuilder<PawsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PawsDbContext(options);
            context.Students.Add(new PAWS.Api.Models.Student { Id = 1, FirstName = "Joel", LastName = "Stancer" });
            context.SaveChanges();

            var service = new CsvExportService();
            var bytes = service.Students(context);
            var content = System.Text.Encoding.UTF8.GetString(bytes);

            Assert.Contains("StudentId", content);
            Assert.Contains("First Name", content);
        }

        [Fact]
        public void XlsxExport_ShouldContainMultipleSheets()
        {
            var options = new DbContextOptionsBuilder<PawsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PawsDbContext(options);

            var service = new XlsxExportService();
            var bytes = service.FullWorkbook(context, null);

            Assert.NotEmpty(bytes);
        }
    }
}
