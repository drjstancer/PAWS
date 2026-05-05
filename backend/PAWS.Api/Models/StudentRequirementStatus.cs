namespace PAWS.Api.Models
{
    public class StudentRequirementStatus
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int RequirementId { get; set; }
        public string Status { get; set; } = "Not Started";
        public DateTime? CompletionDate { get; set; }
    }
}
