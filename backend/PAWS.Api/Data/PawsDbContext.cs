using Microsoft.EntityFrameworkCore;
using PAWS.Api.Models;

namespace PAWS.Api.Data
{
    public class PawsDbContext : DbContext
    {
        public PawsDbContext(DbContextOptions<PawsDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Requirement> Requirements => Set<Requirement>();
        public DbSet<StudentRequirementStatus> StudentRequirementStatuses => Set<StudentRequirementStatus>();
        public DbSet<RequirementApplicability> RequirementApplicabilities => Set<RequirementApplicability>();

        public DbSet<TrackStatus> TrackStatuses => Set<TrackStatus>();
        public DbSet<StudentStatus> StudentStatuses => Set<StudentStatus>();
        public DbSet<Advisor> Advisors => Set<Advisor>();
        public DbSet<AdvisorRole> AdvisorRoles => Set<AdvisorRole>();
        public DbSet<AdvisorRoleAssignment> AdvisorRoleAssignments => Set<AdvisorRoleAssignment>();
        public DbSet<StudentAdvisorAssignment> StudentAdvisorAssignments => Set<StudentAdvisorAssignment>();
        public DbSet<RucaCategory> RucaCategories => Set<RucaCategory>();
        public DbSet<RucaCode> RucaCodes => Set<RucaCode>();

        public DbSet<AcademicRecord> AcademicRecords => Set<AcademicRecord>();
        public DbSet<CourseRecord> CourseRecords => Set<CourseRecord>();
        public DbSet<ShadowingWorkflow> ShadowingWorkflows => Set<ShadowingWorkflow>();
        public DbSet<AdvisingMeeting> AdvisingMeetings => Set<AdvisingMeeting>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<StudentEventParticipation> StudentEventParticipations => Set<StudentEventParticipation>();
        public DbSet<AlumniOutcome> AlumniOutcomes => Set<AlumniOutcome>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasIndex(s => s.MuId).IsUnique();
            modelBuilder.Entity<StudentRequirementStatus>().HasIndex(s => new { s.StudentId, s.RequirementId, s.RequirementCycle }).IsUnique();
            modelBuilder.Entity<ShadowingWorkflow>().HasIndex(s => new { s.StudentId, s.ShadowingCycle }).IsUnique();
            modelBuilder.Entity<StudentEventParticipation>().HasIndex(s => new { s.StudentId, s.EventId }).IsUnique();
        }
    }
}
