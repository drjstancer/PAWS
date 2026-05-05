using System.Text;
using PAWS.Api.Data;

namespace PAWS.Api.Services
{
    public class CsvExportService
    {
        private static string Escape(object? value)
        {
            var text = value?.ToString() ?? string.Empty;
            return $"\"{text.Replace("\"", "\"\"")}\"";
        }

        private static byte[] ToBytes(IEnumerable<string[]> rows)
        {
            var csv = string.Join("\n", rows.Select(row => string.Join(",", row.Select(Escape))));
            return Encoding.UTF8.GetBytes(csv);
        }

        public byte[] Students(PawsDbContext db)
        {
            var rows = new List<string[]>
            {
                new[] { "StudentId", "MU ID", "First Name", "Last Name", "Email", "Program Track", "Classification", "Cohort Year", "Status", "Cumulative GPA", "Science GPA", "RUCA Code", "RUCA Category", "HTM Advisor" }
            };

            rows.AddRange(db.Students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).Select(s => new[]
            {
                s.Id.ToString(), s.MuId, s.FirstName, s.LastName, s.Email, s.ProgramTrack, s.Classification, s.CohortYear.ToString(), s.Status,
                s.CumulativeGpa?.ToString() ?? string.Empty, s.ScienceGpa?.ToString() ?? string.Empty, s.RucaCode?.ToString() ?? string.Empty, s.RucaCategory ?? string.Empty, s.HtmAdvisor ?? string.Empty
            }));

            return ToBytes(rows);
        }

        public byte[] Compliance(PawsDbContext db, string cycle)
        {
            var rows = new List<string[]>
            {
                new[] { "StudentId", "RequirementId", "Cycle", "Status", "Completion Date", "Notes" }
            };

            rows.AddRange(db.StudentRequirementStatuses.Where(r => r.RequirementCycle == cycle).Select(r => new[]
            {
                r.StudentId.ToString(), r.RequirementId.ToString(), r.RequirementCycle, r.Status, r.CompletionDate?.ToString("yyyy-MM-dd") ?? string.Empty, r.Notes ?? string.Empty
            }));

            return ToBytes(rows);
        }

        public byte[] Courses(PawsDbContext db)
        {
            var rows = new List<string[]>
            {
                new[] { "StudentId", "Academic Year", "Term", "Subject", "Number", "Title", "Credit Hours", "Letter Grade", "Grade Value", "Grade Points", "Science/Math", "Category" }
            };

            rows.AddRange(db.CourseRecords.Select(c => new[]
            {
                c.StudentId.ToString(), c.AcademicYear, c.Term, c.CourseSubject, c.CourseNumber, c.CourseTitle ?? string.Empty, c.CreditHours.ToString(), c.LetterGrade,
                c.PerCreditGradeValue?.ToString() ?? string.Empty, c.GradePointsEarned?.ToString() ?? string.Empty, c.CountsTowardScienceMathGpa.ToString(), c.CourseCategory ?? string.Empty
            }));

            return ToBytes(rows);
        }

        public byte[] Shadowing(PawsDbContext db, string cycle)
        {
            var rows = new List<string[]>
            {
                new[] { "StudentId", "Cycle", "Eligibility", "Vetting Status", "HR Clearance Date", "Ready For Matching", "Match Status", "Specialty", "Provider", "Match Date", "Completed Date" }
            };

            rows.AddRange(db.ShadowingWorkflows.Where(s => s.ShadowingCycle == cycle).Select(s => new[]
            {
                s.StudentId.ToString(), s.ShadowingCycle, s.EligibilityStatus, s.VettingStatus, s.HrClearanceReceivedDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                s.ReadyForMatching.ToString(), s.MatchStatus, s.MatchedSpecialty ?? string.Empty, s.MatchedProvider ?? string.Empty, s.MatchDate?.ToString("yyyy-MM-dd") ?? string.Empty, s.ShadowingCompletedDate?.ToString("yyyy-MM-dd") ?? string.Empty
            }));

            return ToBytes(rows);
        }

        public byte[] Alumni(PawsDbContext db)
        {
            var rows = new List<string[]>
            {
                new[] { "StudentId", "Update Date", "Graduation Date", "Application Cycle", "Applied", "Accepted", "Matriculated", "Matriculated School", "Current Program/Position", "Update Source" }
            };

            rows.AddRange(db.AlumniOutcomes.Select(a => new[]
            {
                a.StudentId.ToString(), a.UpdateDate.ToString("yyyy-MM-dd"), a.GraduationDate?.ToString("yyyy-MM-dd") ?? string.Empty, a.ApplicationCycle ?? string.Empty,
                a.AppliedToMedicalSchool?.ToString() ?? string.Empty, a.AcceptedToMedicalSchool?.ToString() ?? string.Empty, a.Matriculated?.ToString() ?? string.Empty,
                a.MatriculatedSchool ?? string.Empty, a.CurrentProgramOrPosition ?? string.Empty, a.UpdateSource ?? string.Empty
            }));

            return ToBytes(rows);
        }
    }
}
