namespace PAWS.Api.Models
{
    public class ProgramTrack
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Classification
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Cohort
    {
        public int Id { get; set; }
        public int Year { get; set; }
    }
}
