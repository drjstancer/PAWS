namespace PAWS.Api.Models
{
    public class RequirementApplicability
    {
        public int Id { get; set; }
        public int RequirementId { get; set; }
        public string ProgramTrack { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }
}
