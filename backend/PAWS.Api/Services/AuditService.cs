using System.Text.Json;
using PAWS.Api.Data;
using PAWS.Api.Models;
using PAWS.Api.Security;

namespace PAWS.Api.Services
{
    public class AuditService
    {
        private readonly PawsDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public AuditService(PawsDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public void Log(string action, string entity, object? before, object? after, string? entityId = null)
        {
            var log = new AuditLog
            {
                AppUserId = _currentUser.User.AppUserId,
                Action = action,
                EntityName = entity,
                EntityId = entityId,
                BeforeValue = before != null ? JsonSerializer.Serialize(before) : null,
                AfterValue = after != null ? JsonSerializer.Serialize(after) : null
            };

            _db.AuditLogs.Add(log);
            _db.SaveChanges();
        }
    }
}
