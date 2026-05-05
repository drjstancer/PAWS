using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PAWS.Api.Data;

namespace PAWS.Api.Services
{
    public class ReportService
    {
        private readonly PawsDbContext _context;

        public ReportService(PawsDbContext context)
        {
            _context = context;
        }

        public byte[] GenerateFacultyReport(string cycle)
        {
            var totalStudents = _context.Students.Count();
            var avgGpa = _context.Students.Where(s => s.CumulativeGpa.HasValue).Average(s => s.CumulativeGpa);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    page.Header().Text("PAWS Program Report").FontSize(20).Bold();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Cycle: {cycle}");
                        col.Item().Text($"Total Students: {totalStudents}");
                        col.Item().Text($"Average GPA: {Math.Round(avgGpa ?? 0,2)}");
                        col.Item().Text("This report summarizes program outcomes, compliance, and academic trends.");
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("MM/dd/yyyy")).SemiBold();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
