using PAWS.Api.Models;

namespace PAWS.Api.Data
{
    public static class DbInitializer
    {
        public static void Seed(PawsDbContext db)
        {
            SeedProgramReferences(db);
            SeedSecurity(db);
            db.SaveChanges();
        }

        private static void SeedProgramReferences(PawsDbContext db)
        {
            SeedIfMissing(db.TrackStatuses, new[] { "Current Track", "Legacy Track" }, name => new TrackStatus { Name = name });
            SeedIfMissing(db.StudentStatuses, new[] { "Applicant", "Accepted", "Active", "On Hold", "Withdrawn", "Dismissed", "Graduated", "Alum" }, name => new StudentStatus { Name = name });
            SeedIfMissing(db.AdvisorRoles, new[] { "HTM", "Well-Being Advisor", "CASE Liaison", "MedOpp Advisor", "Staff", "Faculty", "Leadership", "Other" }, name => new AdvisorRole { Name = name });
            SeedIfMissing(db.RucaCategories, new[] { "Urban/Metropolitan", "Rural/Nonmetropolitan" }, name => new RucaCategory { Name = name });

            if (!db.RucaCodes.Any())
            {
                var urban = db.RucaCategories.First(c => c.Name == "Urban/Metropolitan");
                var rural = db.RucaCategories.First(c => c.Name == "Rural/Nonmetropolitan");

                for (var code = 1; code <= 10; code++)
                {
                    db.RucaCodes.Add(new RucaCode
                    {
                        Code = code,
                        RucaCategoryId = code <= 3 ? urban.Id : rural.Id,
                        Description = code <= 3 ? "Urban/Metropolitan" : "Rural/Nonmetropolitan"
                    });
                }
            }

            SeedIfMissing(db.Requirements, new[]
            {
                "Annual Contract Submission",
                "Retreat Attendance",
                "Pre-Med Day Participation",
                "MedPrep 1 Participation",
                "MedPrep 2 Participation",
                "HTM Meeting",
                "CASE Meeting",
                "Well-Being Meeting",
                "MedOpp Meeting"
            }, name => new Requirement { Name = name, Category = name.Contains("Meeting") ? "Meeting" : name.Contains("Attendance") || name.Contains("Participation") ? "Event" : "Form", Required = true });
        }

        private static void SeedSecurity(PawsDbContext db)
        {
            var roles = new[]
            {
                "System Admin",
                "Program Admin",
                "Program Staff",
                "Advisor",
                "Leadership Viewer",
                "Reporting Viewer",
                "Restricted Notes Viewer"
            };
            SeedIfMissing(db.AppRoles, roles, name => new AppRole { Name = name });

            var permissions = new[]
            {
                "Students.View", "Students.Edit",
                "Academic.View", "Academic.Edit",
                "Requirements.View", "Requirements.Edit", "Requirements.Generate",
                "Compliance.View",
                "Shadowing.View", "Shadowing.Edit",
                "Advising.View", "Advising.Create", "Advising.ViewRestricted",
                "Events.View", "Events.Edit",
                "Alumni.View", "Alumni.Edit",
                "Reports.View", "Reports.Export",
                "Analytics.View",
                "Users.Manage", "Audit.View"
            };
            SeedIfMissing(db.AppPermissions, permissions, name => new AppPermission { Name = name });

            db.SaveChanges();

            var adminRole = db.AppRoles.First(r => r.Name == "System Admin");
            foreach (var permission in db.AppPermissions.ToList())
            {
                if (!db.RolePermissionAssignments.Any(x => x.AppRoleId == adminRole.Id && x.AppPermissionId == permission.Id))
                {
                    db.RolePermissionAssignments.Add(new RolePermissionAssignment
                    {
                        AppRoleId = adminRole.Id,
                        AppPermissionId = permission.Id
                    });
                }
            }

            var programAdmin = db.AppRoles.First(r => r.Name == "Program Admin");
            AssignPermissions(db, programAdmin, permissions.Where(p => p != "Users.Manage" && p != "Audit.View"));

            var programStaff = db.AppRoles.First(r => r.Name == "Program Staff");
            AssignPermissions(db, programStaff, new[]
            {
                "Students.View", "Students.Edit", "Academic.View", "Academic.Edit", "Requirements.View", "Requirements.Generate",
                "Compliance.View", "Shadowing.View", "Shadowing.Edit", "Advising.View", "Advising.Create", "Events.View", "Events.Edit",
                "Alumni.View", "Reports.View", "Analytics.View"
            });

            var advisor = db.AppRoles.First(r => r.Name == "Advisor");
            AssignPermissions(db, advisor, new[] { "Students.View", "Academic.View", "Compliance.View", "Advising.View", "Advising.Create", "Reports.View" });

            var leadership = db.AppRoles.First(r => r.Name == "Leadership Viewer");
            AssignPermissions(db, leadership, new[] { "Students.View", "Compliance.View", "Reports.View", "Analytics.View" });

            var reporting = db.AppRoles.First(r => r.Name == "Reporting Viewer");
            AssignPermissions(db, reporting, new[] { "Reports.View", "Reports.Export", "Analytics.View" });

            var restricted = db.AppRoles.First(r => r.Name == "Restricted Notes Viewer");
            AssignPermissions(db, restricted, new[] { "Advising.ViewRestricted" });
        }

        private static void AssignPermissions(PawsDbContext db, AppRole role, IEnumerable<string> permissionNames)
        {
            foreach (var name in permissionNames)
            {
                var permission = db.AppPermissions.First(p => p.Name == name);
                if (!db.RolePermissionAssignments.Any(x => x.AppRoleId == role.Id && x.AppPermissionId == permission.Id))
                {
                    db.RolePermissionAssignments.Add(new RolePermissionAssignment
                    {
                        AppRoleId = role.Id,
                        AppPermissionId = permission.Id
                    });
                }
            }
        }

        private static void SeedIfMissing<T>(IQueryable<T> query, IEnumerable<string> values, Func<string, T> factory) where T : class
        {
            var dbSet = query as Microsoft.EntityFrameworkCore.DbSet<T>;
            if (dbSet == null) return;

            foreach (var value in values)
            {
                var exists = query.Any(x => EFPropertyString(x, "Name") == value);
                if (!exists)
                {
                    dbSet.Add(factory(value));
                }
            }
        }

        private static string EFPropertyString<T>(T entity, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return property?.GetValue(entity)?.ToString() ?? string.Empty;
        }
    }
}
