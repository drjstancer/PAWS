using ClosedXML.Excel;
using PAWS.Api.Data;

namespace PAWS.Api.Services
{
    public class XlsxExportService
    {
        public byte[] FullWorkbook(PawsDbContext db, string? cycle)
        {
            using var wb = new XLWorkbook();

            var students = wb.Worksheets.Add("Students");
            students.Cell(1,1).Value = "StudentId";
            students.Cell(1,2).Value = "Name";
            students.Cell(1,3).Value = "Program";
            students.Cell(1,4).Value = "GPA";

            var sData = db.Students.ToList();
            for(int i=0;i<sData.Count;i++)
            {
                var s = sData[i];
                students.Cell(i+2,1).Value = s.Id;
                students.Cell(i+2,2).Value = s.FirstName + " " + s.LastName;
                students.Cell(i+2,3).Value = s.ProgramTrack;
                students.Cell(i+2,4).Value = s.CumulativeGpa;
            }

            var compliance = wb.Worksheets.Add("Compliance");
            compliance.Cell(1,1).Value = "StudentId";
            compliance.Cell(1,2).Value = "Requirement";
            compliance.Cell(1,3).Value = "Status";

            var cData = db.StudentRequirementStatuses
                .Where(r => cycle == null || r.RequirementCycle == cycle)
                .ToList();

            for(int i=0;i<cData.Count;i++)
            {
                var r = cData[i];
                compliance.Cell(i+2,1).Value = r.StudentId;
                compliance.Cell(i+2,2).Value = r.RequirementId;
                compliance.Cell(i+2,3).Value = r.Status;
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
