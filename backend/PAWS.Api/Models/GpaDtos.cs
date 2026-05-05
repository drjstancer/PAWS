namespace PAWS.Api.Models
{
    public class GpaCalculationResultDto
    {
        public int StudentId { get; set; }
        public string? AcademicYear { get; set; }
        public string? Term { get; set; }
        public decimal TotalAttemptedCredits { get; set; }
        public decimal TotalGradePoints { get; set; }
        public decimal? CumulativeGpa { get; set; }
        public decimal ScienceMathAttemptedCredits { get; set; }
        public decimal ScienceMathGradePoints { get; set; }
        public decimal? ScienceMathGpa { get; set; }
        public int IncludedCourseCount { get; set; }
        public int ScienceMathCourseCount { get; set; }
        public List<string> Warnings { get; set; } = new();
    }

    public class GpaSyncRequestDto
    {
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public bool SyncToStudentProfile { get; set; } = true;
        public bool CreateAcademicRecord { get; set; } = true;
    }
}
