namespace PAWS.Api.Models
{
    public class TrackStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }

    public class StudentStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }

    public class Advisor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Department { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AdvisorRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }

    public class AdvisorRoleAssignment
    {
        public int Id { get; set; }
        public int AdvisorId { get; set; }
        public int AdvisorRoleId { get; set; }
        public bool Active { get; set; } = true;
    }

    public class StudentAdvisorAssignment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AdvisorId { get; set; }
        public int AdvisorRoleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; } = true;
        public string? Notes { get; set; }
    }

    public class RucaCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }

    public class RucaCode
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public int RucaCategoryId { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; } = true;
    }
}
