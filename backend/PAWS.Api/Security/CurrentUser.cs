namespace PAWS.Api.Security
{
    public class CurrentUser
    {
        public int? AppUserId { get; set; }
        public string InstitutionalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Email);
    }

    public interface ICurrentUserService
    {
        CurrentUser User { get; set; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUser User { get; set; } = new();
    }
}
