namespace PAWS.Api.Models
{
    public class Requirement
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool Required { get; set; }
    }
}
