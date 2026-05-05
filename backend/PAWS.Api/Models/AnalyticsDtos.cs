namespace PAWS.Api.Models
{
    public class RiskSignalDto
    {
        public int StudentId { get; set; }
        public string MuId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string ProgramTrack { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; } = "Low";
        public List<string> RiskFactors { get; set; } = new();
        public string RecommendedAction { get; set; } = string.Empty;
    }

    public class PublicationTableDto
    {
        public string TableName { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new();
        public List<Dictionary<string, object?>> Rows { get; set; } = new();
    }

    public class FacultyReportDto
    {
        public string Title { get; set; } = "JPAWS/PAWS Faculty Reporting Summary";
        public string ReportingCycle { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public object? ExecutiveSummary { get; set; }
        public object? ParticipationSummary { get; set; }
        public object? ComplianceSummary { get; set; }
        public object? AcademicSummary { get; set; }
        public object? EquityRuralitySummary { get; set; }
        public object? RiskSummary { get; set; }
    }
}
