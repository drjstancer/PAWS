namespace PAWS.Api.Models
{
    public class StudentImportDto
    {
        public string MuId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProgramTrack { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public int CohortYear { get; set; }
        public decimal? CumulativeGpa { get; set; }
        public decimal? ScienceGpa { get; set; }
        public int? RucaCode { get; set; }
        public string? HtmAdvisor { get; set; }
    }

    public class ImportResultDto
    {
        public int Created { get; set; }
        public int Updated { get; set; }
        public int Rejected { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
