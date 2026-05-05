namespace PAWS.Api.Models
{
    public class AcademicRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public decimal? CumulativeGpa { get; set; }
        public decimal? ScienceMathGpa { get; set; }
        public decimal? ScienceMathGpaOverride { get; set; }
        public string? GpaDataSource { get; set; }
        public int? McatTotal { get; set; }
        public DateTime? McatTestDate { get; set; }
        public string? McatSource { get; set; }
        public string? AmcasId { get; set; }
        public string? AcademicStandingFlag { get; set; }
        public string? AcademicNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CourseRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string Term { get; set; } = string.Empty;
        public string CourseSubject { get; set; } = string.Empty;
        public string CourseNumber { get; set; } = string.Empty;
        public string? CourseTitle { get; set; }
        public decimal CreditHours { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public decimal? PerCreditGradeValue { get; set; }
        public decimal? GradePointsEarned { get; set; }
        public bool CountsTowardScienceMathGpa { get; set; }
        public string? CourseCategory { get; set; }
        public bool? RepeatFlag { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ShadowingWorkflow
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string ShadowingCycle { get; set; } = string.Empty;
        public string EligibilityStatus { get; set; } = "Not Eligible";
        public DateTime? EligibilityDate { get; set; }
        public DateTime? VettingRequestSubmittedDate { get; set; }
        public string VettingStatus { get; set; } = "Not Started";
        public DateTime? HrClearanceReceivedDate { get; set; }
        public bool ReadyForMatching { get; set; }
        public string MatchStatus { get; set; } = "Not Ready";
        public string? MatchedSpecialty { get; set; }
        public string? MatchedProvider { get; set; }
        public DateTime? MatchDate { get; set; }
        public DateTime? ShadowingCompletedDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AdvisingMeeting
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AdvisorId { get; set; }
        public string MeetingType { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; }
        public string? MeetingMode { get; set; }
        public bool RequiredMeeting { get; set; }
        public string? MeetingSummary { get; set; }
        public bool FollowUpNeeded { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string? ConcernLevel { get; set; }
        public bool? ReferralMade { get; set; }
        public string? ReferralType { get; set; }
        public bool RestrictedNote { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public bool Required { get; set; }
        public int? RelatedRequirementId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class StudentEventParticipation
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int EventId { get; set; }
        public string ParticipationStatus { get; set; } = "Registered";
        public DateTime? ParticipationDate { get; set; }
        public int? VerifiedByAdvisorId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AlumniOutcome
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public string? ApplicationCycle { get; set; }
        public bool? AppliedToMedicalSchool { get; set; }
        public bool? AcceptedToMedicalSchool { get; set; }
        public bool? Matriculated { get; set; }
        public string? MatriculatedSchool { get; set; }
        public string? CurrentProgramOrPosition { get; set; }
        public string? UpdateSource { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
