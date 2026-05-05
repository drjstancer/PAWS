using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAWS.Api.Migrations
{
    /// <summary>
    /// Initial production schema migration placeholder.
    ///
    /// IMPORTANT FOR IT:
    /// This file marks the migration boundary for the production schema.
    /// Run `dotnet ef migrations remove` and then `dotnet ef migrations add InitialProductionSchema`
    /// in the target development environment if the generated SQL needs to exactly match the deployed SQL Server version.
    ///
    /// The application is configured to run db.Database.Migrate() on startup.
    /// </summary>
    public partial class InitialProductionSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Generated migration operations should be produced by EF Core in the deployment environment.
            // This placeholder is intentionally empty to avoid committing an unverified hand-written schema.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // See note above. IT should regenerate the migration before first production deployment.
        }
    }
}
