namespace PAWS.Api.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string MuId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProgramTrack { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public decimal? CumulativeGpa { get; set; }
        public decimal? ScienceGpa { get; set; }
    }
}
