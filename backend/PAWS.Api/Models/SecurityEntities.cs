namespace PAWS.Api.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string InstitutionalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AppRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Active { get; set; } = true;
    }

    public class AppPermission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Active { get; set; } = true;
    }

    public class UserRoleAssignment
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int AppRoleId { get; set; }
        public bool Active { get; set; } = true;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public int? AssignedByUserId { get; set; }
    }

    public class RolePermissionAssignment
    {
        public int Id { get; set; }
        public int AppRoleId { get; set; }
        public int AppPermissionId { get; set; }
        public bool Active { get; set; } = true;
    }

    public class AuditLog
    {
        public int Id { get; set; }
        public int? AppUserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? BeforeValue { get; set; }
        public string? AfterValue { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
