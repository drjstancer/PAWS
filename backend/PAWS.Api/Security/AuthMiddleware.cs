using PAWS.Api.Data;
using PAWS.Api.Models;

namespace PAWS.Api.Security
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, PawsDbContext db, ICurrentUserService currentUser)
        {
            var email = context.Request.Headers["x-forwarded-user"].FirstOrDefault();
            var name = context.Request.Headers["x-forwarded-name"].FirstOrDefault();

            if (!string.IsNullOrEmpty(email))
            {
                var user = db.AppUsers.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        Email = email,
                        FullName = name ?? email,
                        InstitutionalId = email
                    };
                    db.AppUsers.Add(user);
                    db.SaveChanges();
                }

                var roles = db.UserRoleAssignments
                    .Where(r => r.AppUserId == user.Id && r.Active)
                    .Select(r => r.AppRoleId)
                    .ToList();

                var permissions = db.RolePermissionAssignments
                    .Where(r => roles.Contains(r.AppRoleId) && r.Active)
                    .Select(r => r.AppPermissionId)
                    .ToList();

                var permissionNames = db.AppPermissions
                    .Where(p => permissions.Contains(p.Id))
                    .Select(p => p.Name)
                    .ToList();

                currentUser.User = new CurrentUser
                {
                    AppUserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Permissions = permissionNames
                };
            }

            await _next(context);
        }
    }
}
