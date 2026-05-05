using ClosedXML.Excel;
using PAWS.Api.Data;

namespace PAWS.Api.Services
{
    public class XlsxExportService
    {
        public byte[] FullWorkbook(PawsDbContext db, string? cycle)
        {
            using var wb = new XLWorkbook();

            AddStudentsSheet(wb, db);
            AddComplianceSheet(wb, db, cycle);
            AddCoursesSheet(wb, db);
            AddShadowingSheet(wb, db, cycle);
            AddAlumniSheet(wb, db);

            foreach (var ws in wb.Worksheets)
            {
                ws.Columns().AdjustToContents();
                ws.SheetView.FreezeRows(1);
                var used = ws.RangeUsed();
                if (used != null)
                {
                    used.SetAutoFilter();
                    ws.Row(1).Style.Font.Bold = true;
                    ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1B82D");
                    ws.Row(1).Style.Font.FontColor = XLColor.Black;
                }
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        private static void AddStudentsSheet(XLWorkbook wb, PawsDbContext db)
        {
            var ws = wb.Worksheets.Add("Students");
            var headers = new[] { "StudentId", "MU ID", "First Name", "Last Name", "Email", "Program", "Classification", "Cohort Year", "Status", "Cumulative GPA", "Science GPA", "RUCA Code", "RUCA Category", "HTM Advisor" };
            WriteHeader(ws, headers);

            var rows = db.Students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            for (int i = 0; i < rows.Count; i++)
            {
                var s = rows[i];
                var r = i + 2;
                ws.Cell(r, 1).Value = s.Id;
                ws.Cell(r, 2).Value = s.MuId;
                ws.Cell(r, 3).Value = s.FirstName;
                ws.Cell(r, 4).Value = s.LastName;
                ws.Cell(r, 5).Value = s.Email;
                ws.Cell(r, 6).Value = s.ProgramTrack;
                ws.Cell(r, 7).Value = s.Classification;
                ws.Cell(r, 8).Value = s.CohortYear;
                ws.Cell(r, 9).Value = s.Status;
                ws.Cell(r, 10).Value = s.CumulativeGpa;
                ws.Cell(r, 11).Value = s.ScienceGpa;
                ws.Cell(r, 12).Value = s.RucaCode;
                ws.Cell(r, 13).Value = s.RucaCategory;
                ws.Cell(r, 14).Value = s.HtmAdvisor;
            }
        }

        private static void AddComplianceSheet(XLWorkbook wb, PawsDbContext db, string? cycle)
        {
            var ws = wb.Worksheets.Add("Compliance");
            var headers = new[] { "StudentRequirementStatusId", "StudentId", "RequirementId", "Cycle", "Status", "Completion Date", "Notes" };
            WriteHeader(ws, headers);

            var rows = db.StudentRequirementStatuses
                .Where(r => cycle == null || r.RequirementCycle == cycle)
                .OrderBy(r => r.StudentId)
                .ThenBy(r => r.RequirementId)
                .ToList();

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                var row = i + 2;
                ws.Cell(row, 1).Value = r.Id;
                ws.Cell(row, 2).Value = r.StudentId;
                ws.Cell(row, 3).Value = r.RequirementId;
                ws.Cell(row, 4).Value = r.RequirementCycle;
                ws.Cell(row, 5).Value = r.Status;
                ws.Cell(row, 6).Value = r.CompletionDate;
                ws.Cell(row, 6).Style.DateFormat.Format = "yyyy-mm-dd";
                ws.Cell(row, 7).Value = r.Notes;
            }
        }

        private static void AddCoursesSheet(XLWorkbook wb, PawsDbContext db)
        {
            var ws = wb.Worksheets.Add("Courses_GPA");
            var headers = new[] { "CourseRecordId", "StudentId", "Academic Year", "Term", "Subject", "Number", "Title", "Credit Hours", "Letter Grade", "Grade Value", "Grade Points", "Science/Math", "Category", "Repeat" };
            WriteHeader(ws, headers);

            var rows = db.CourseRecords.OrderBy(c => c.StudentId).ThenBy(c => c.AcademicYear).ThenBy(c => c.Term).ToList();
            for (int i = 0; i < rows.Count; i++)
            {
                var c = rows[i];
                var r = i + 2;
                ws.Cell(r, 1).Value = c.Id;
                ws.Cell(r, 2).Value = c.StudentId;
                ws.Cell(r, 3).Value = c.AcademicYear;
                ws.Cell(r, 4).Value = c.Term;
                ws.Cell(r, 5).Value = c.CourseSubject;
                ws.Cell(r, 6).Value = c.CourseNumber;
                ws.Cell(r, 7).Value = c.CourseTitle;
                ws.Cell(r, 8).Value = c.CreditHours;
                ws.Cell(r, 9).Value = c.LetterGrade;
                ws.Cell(r, 10).Value = c.PerCreditGradeValue;
                ws.Cell(r, 11).Value = c.GradePointsEarned;
                ws.Cell(r, 12).Value = c.CountsTowardScienceMathGpa;
                ws.Cell(r, 13).Value = c.CourseCategory;
                ws.Cell(r, 14).Value = c.RepeatFlag;
            }
        }

        private static void AddShadowingSheet(XLWorkbook wb, PawsDbContext db, string? cycle)
        {
            var ws = wb.Worksheets.Add("Shadowing");
            var headers = new[] { "ShadowingWorkflowId", "StudentId", "Cycle", "Eligibility", "Vetting Status", "Request Submitted", "HR Clearance", "Ready", "Match Status", "Specialty", "Provider", "Match Date", "Completed Date" };
            WriteHeader(ws, headers);

            var rows = db.ShadowingWorkflows
                .Where(s => cycle == null || s.ShadowingCycle == cycle)
                .OrderBy(s => s.StudentId)
                .ToList();

            for (int i = 0; i < rows.Count; i++)
            {
                var s = rows[i];
                var r = i + 2;
                ws.Cell(r, 1).Value = s.Id;
                ws.Cell(r, 2).Value = s.StudentId;
                ws.Cell(r, 3).Value = s.ShadowingCycle;
                ws.Cell(r, 4).Value = s.EligibilityStatus;
                ws.Cell(r, 5).Value = s.VettingStatus;
                ws.Cell(r, 6).Value = s.VettingRequestSubmittedDate;
                ws.Cell(r, 7).Value = s.HrClearanceReceivedDate;
                ws.Cell(r, 8).Value = s.ReadyForMatching;
                ws.Cell(r, 9).Value = s.MatchStatus;
                ws.Cell(r, 10).Value = s.MatchedSpecialty;
                ws.Cell(r, 11).Value = s.MatchedProvider;
                ws.Cell(r, 12).Value = s.MatchDate;
                ws.Cell(r, 13).Value = s.ShadowingCompletedDate;
                for (int col = 6; col <= 13; col++) ws.Cell(r, col).Style.DateFormat.Format = "yyyy-mm-dd";
            }
        }

        private static void AddAlumniSheet(XLWorkbook wb, PawsDbContext db)
        {
            var ws = wb.Worksheets.Add("Alumni_Outcomes");
            var headers = new[] { "AlumniOutcomeId", "StudentId", "Update Date", "Graduation Date", "Application Cycle", "Applied", "Accepted", "Matriculated", "Matriculated School", "Current Program/Position", "Update Source" };
            WriteHeader(ws, headers);

            var rows = db.AlumniOutcomes.OrderBy(a => a.StudentId).ThenByDescending(a => a.UpdateDate).ToList();
            for (int i = 0; i < rows.Count; i++)
            {
                var a = rows[i];
                var r = i + 2;
                ws.Cell(r, 1).Value = a.Id;
                ws.Cell(r, 2).Value = a.StudentId;
                ws.Cell(r, 3).Value = a.UpdateDate;
                ws.Cell(r, 4).Value = a.GraduationDate;
                ws.Cell(r, 5).Value = a.ApplicationCycle;
                ws.Cell(r, 6).Value = a.AppliedToMedicalSchool;
                ws.Cell(r, 7).Value = a.AcceptedToMedicalSchool;
                ws.Cell(r, 8).Value = a.Matriculated;
                ws.Cell(r, 9).Value = a.MatriculatedSchool;
                ws.Cell(r, 10).Value = a.CurrentProgramOrPosition;
                ws.Cell(r, 11).Value = a.UpdateSource;
                ws.Cell(r, 3).Style.DateFormat.Format = "yyyy-mm-dd";
                ws.Cell(r, 4).Style.DateFormat.Format = "yyyy-mm-dd";
            }
        }

        private static void WriteHeader(IXLWorksheet ws, string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
            }
        }
    }
}
