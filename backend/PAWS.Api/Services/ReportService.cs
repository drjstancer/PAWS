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
            var students = _context.Students.ToList();
            var requirements = _context.StudentRequirementStatuses.Where(r => r.RequirementCycle == cycle).ToList();

            var totalStudents = students.Count;
            var activeStudents = students.Count(s => s.Status == "Active");
            var avgGpa = students.Where(s => s.CumulativeGpa.HasValue).Any()
                ? students.Where(s => s.CumulativeGpa.HasValue).Average(s => s.CumulativeGpa!.Value)
                : 0;
            var avgScienceGpa = students.Where(s => s.ScienceGpa.HasValue).Any()
                ? students.Where(s => s.ScienceGpa.HasValue).Average(s => s.ScienceGpa!.Value)
                : 0;

            var completed = requirements.Count(r => r.Status == "Completed" || r.Status == "Waived");
            var incomplete = requirements.Count(r => r.Status == "Not Started" || r.Status == "In Progress");
            var complianceRate = requirements.Count == 0 ? 0 : (int)((double)completed / requirements.Count * 100);

            var programGroups = students
                .GroupBy(s => string.IsNullOrWhiteSpace(s.ProgramTrack) ? "Unknown" : s.ProgramTrack)
                .Select(g => new ReportGroup(g.Key, g.Count()))
                .OrderByDescending(g => g.Count)
                .ToList();

            var rucaGroups = students
                .GroupBy(s => string.IsNullOrWhiteSpace(s.RucaCategory) ? "Unknown" : s.RucaCategory!)
                .Select(g => new ReportGroup(g.Key, g.Count()))
                .OrderByDescending(g => g.Count)
                .ToList();

            var riskGroups = BuildRiskGroups(students, requirements);

            var ruralCount = students.Count(s => s.RucaCategory == "Rural/Nonmetropolitan");
            var ruralPct = totalStudents == 0 ? 0 : Math.Round((decimal)ruralCount / totalStudents * 100, 1);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.Letter);

                    page.Header().Column(header =>
                    {
                        header.Item().Text("JPAWS/PAWS Faculty & Grant Reporting Summary").FontSize(20).Bold().FontColor(Colors.Blue.Darken4);
                        header.Item().Text($"Reporting Cycle: {cycle}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    });

                    page.Content().Column(col =>
                    {
                        col.Spacing(14);

                        SectionTitle(col, "1. Executive Summary");
                        col.Item().Text($"The JPAWS/PAWS data system currently includes {totalStudents} student records, including {activeStudents} active participants. For the {cycle} reporting cycle, the program shows an overall compliance rate of {complianceRate}%. Participants with GPA data have an average cumulative GPA of {Math.Round(avgGpa, 2)} and an average science/math GPA of {Math.Round(avgScienceGpa, 2)}.");
                        col.Item().Text($"Rural/nonmetropolitan participants represent {ruralPct}% of records with current RUCA categorization, supporting the program's ability to report access and pathway reach across geographic background.");

                        SectionTitle(col, "2. Key Metrics");
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            MetricCell(table, "Total Students", totalStudents.ToString());
                            MetricCell(table, "Active Students", activeStudents.ToString());
                            MetricCell(table, "Compliance Rate", complianceRate + "%");
                            MetricCell(table, "Average GPA", Math.Round(avgGpa, 2).ToString());
                        });

                        SectionTitle(col, "3. Participation by Program Track");
                        BarVisual(col, programGroups, Colors.Yellow.Darken2);

                        SectionTitle(col, "4. Compliance Summary");
                        col.Item().Text($"Completed or waived requirements: {completed}. Incomplete requirements: {incomplete}. These indicators should be interpreted as program engagement and follow-up signals rather than deficit labels for students.");
                        ProgressBar(col, complianceRate, Colors.Green.Darken2);

                        SectionTitle(col, "5. Academic Outcomes");
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            LabelValue(table, "Average Cumulative GPA", Math.Round(avgGpa, 2).ToString());
                            LabelValue(table, "Average Science/Math GPA", Math.Round(avgScienceGpa, 2).ToString());
                        });

                        SectionTitle(col, "6. Equity and Rurality Summary");
                        BarVisual(col, rucaGroups, Colors.Blue.Darken3);
                        col.Item().Text("RUCA categories are used as geographic access indicators. RUCA codes 1-3 are categorized as Urban/Metropolitan; RUCA codes 4-10 are categorized as Rural/Nonmetropolitan.").FontSize(9).FontColor(Colors.Grey.Darken2);

                        SectionTitle(col, "7. Decision-Support Risk Signals");
                        BarVisual(col, riskGroups, Colors.Red.Darken2);
                        col.Item().Text("Risk indicators are rule-based decision-support signals using compliance completion and academic indicators. They should guide outreach and advising, not be interpreted as deterministic predictions.").FontSize(9).FontColor(Colors.Grey.Darken2);

                        SectionTitle(col, "8. Interpretation for Grant and Faculty Reporting");
                        col.Item().Text("The system supports continuous program monitoring, timely intervention, and reproducible reporting. Data can be used to describe program reach, student progression, compliance engagement, and academic outcomes while maintaining a transparent audit trail for exports and report generation.");
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated by PAWS Data Management System on ");
                        x.Span(DateTime.Now.ToString("MM/dd/yyyy")).SemiBold();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static List<ReportGroup> BuildRiskGroups(IEnumerable<Models.Student> students, List<Models.StudentRequirementStatus> requirements)
        {
            return students.Select(s =>
            {
                var studentReqs = requirements.Where(r => r.StudentId == s.Id).ToList();
                var missing = studentReqs.Count(r => r.Status != "Completed" && r.Status != "Waived");
                var score = 0;
                if (missing > 3) score += 2;
                if (s.CumulativeGpa.HasValue && s.CumulativeGpa < 3.0m) score += 2;
                if (s.ScienceGpa.HasValue && s.ScienceGpa < 2.8m) score += 2;
                return score >= 4 ? "High" : score >= 2 ? "Moderate" : "Low";
            })
            .GroupBy(x => x)
            .Select(g => new ReportGroup(g.Key, g.Count()))
            .OrderBy(g => g.Label)
            .ToList();
        }

        private static void SectionTitle(ColumnDescriptor col, string title)
        {
            col.Item().PaddingTop(8).Text(title).FontSize(13).Bold().FontColor(Colors.Blue.Darken4);
        }

        private static void MetricCell(TableDescriptor table, string label, string value)
        {
            table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Column(c =>
            {
                c.Item().Text(label).FontSize(8).FontColor(Colors.Grey.Darken1);
                c.Item().Text(value).FontSize(15).Bold();
            });
        }

        private static void LabelValue(TableDescriptor table, string label, string value)
        {
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(label).SemiBold();
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(value);
        }

        private static void ProgressBar(ColumnDescriptor col, int percentage, string color)
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Height(14).Background(Colors.Grey.Lighten2).Layers(layers =>
                {
                    layers.PrimaryLayer().Width(Math.Max(1, percentage) * 4).Background(color);
                    layers.Layer().AlignCenter().Text(percentage + "%").FontSize(8).FontColor(Colors.White);
                });
            });
        }

        private static void BarVisual(ColumnDescriptor col, List<ReportGroup> groups, string color)
        {
            var max = groups.Count == 0 ? 1 : groups.Max(g => g.Count);
            foreach (var group in groups)
            {
                var width = Math.Max(20, (int)((decimal)group.Count / max * 280));
                col.Item().PaddingBottom(4).Row(row =>
                {
                    row.ConstantItem(130).Text(group.Label).FontSize(9);
                    row.ConstantItem(width).Height(12).Background(color);
                    row.RelativeItem().PaddingLeft(6).Text(group.Count.ToString()).FontSize(9);
                });
            }
        }

        private record ReportGroup(string Label, int Count);
    }
}
