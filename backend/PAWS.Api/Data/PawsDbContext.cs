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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasIndex(s => s.MuId).IsUnique();
        }
    }
}
