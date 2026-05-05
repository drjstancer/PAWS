export const DEMO_USERS = [
  {
    id: 1,
    name: 'Dr. Joel Stancer',
    email: 'admin@paws.demo',
    role: 'System Admin',
    password: 'Password123!',
    permissions: ['Students.View','Students.Edit','Academic.View','Academic.Edit','Requirements.View','Requirements.Edit','Requirements.Generate','Compliance.View','Shadowing.View','Shadowing.Edit','Advising.View','Advising.Create','Advising.ViewRestricted','Events.View','Events.Edit','Alumni.View','Alumni.Edit','Reports.View','Reports.Export','Analytics.View','Users.Manage','Audit.View']
  },
  {
    id: 2,
    name: 'Program Admin Demo',
    email: 'program@paws.demo',
    role: 'Program Admin',
    password: 'Password123!',
    permissions: ['Students.View','Students.Edit','Academic.View','Academic.Edit','Requirements.View','Requirements.Edit','Requirements.Generate','Compliance.View','Shadowing.View','Shadowing.Edit','Advising.View','Advising.Create','Events.View','Events.Edit','Alumni.View','Alumni.Edit','Reports.View','Reports.Export','Analytics.View']
  },
  {
    id: 3,
    name: 'Advisor Demo',
    email: 'advisor@paws.demo',
    role: 'Advisor / HTM',
    password: 'Password123!',
    permissions: ['Students.View','Academic.View','Compliance.View','Advising.View','Advising.Create','Shadowing.View','Reports.View']
  },
  {
    id: 4,
    name: 'Leadership Viewer Demo',
    email: 'leadership@paws.demo',
    role: 'Leadership Viewer',
    password: 'Password123!',
    permissions: ['Students.View','Compliance.View','Reports.View','Analytics.View']
  },
  {
    id: 5,
    name: 'Restricted Notes Viewer Demo',
    email: 'restricted@paws.demo',
    role: 'Restricted Notes Viewer',
    password: 'Password123!',
    permissions: ['Students.View','Academic.View','Compliance.View','Advising.View','Advising.Create','Advising.ViewRestricted','Reports.View']
  }
]

export const SEEDED_STUDENTS = [
  { id: 1, muId: '12345678', firstName: 'Aaliyah', lastName: 'Brooks', email: 'abrooks@missouri.edu', programTrack: 'JPAWS', classification: 'Sophomore', cohortYear: 2024, cumulativeGpa: 3.72, scienceGpa: 3.61, rucaCode: 2, rucaCategory: 'Urban/Metropolitan', htmAdvisor: 'Dr. J', status: 'Active' },
  { id: 2, muId: '22345678', firstName: 'Marcus', lastName: 'Carter', email: 'mcarter@missouri.edu', programTrack: 'PAWS Achiever', classification: 'Junior', cohortYear: 2023, cumulativeGpa: 3.28, scienceGpa: 3.04, rucaCode: 7, rucaCategory: 'Rural/Nonmetropolitan', htmAdvisor: 'Dr. Simmons', status: 'Active' },
  { id: 3, muId: '32345678', firstName: 'Sofia', lastName: 'Nguyen', email: 'snguyen@missouri.edu', programTrack: 'PAWS Pre-Admissions', classification: 'Senior', cohortYear: 2022, cumulativeGpa: 3.86, scienceGpa: 3.79, rucaCode: 1, rucaCategory: 'Urban/Metropolitan', htmAdvisor: 'Dr. Kelley', status: 'Active' },
  { id: 4, muId: '42345678', firstName: 'Elijah', lastName: 'Reed', email: 'ereed@missouri.edu', programTrack: 'PAWS', classification: 'Junior', cohortYear: 2021, cumulativeGpa: 2.91, scienceGpa: 2.74, rucaCode: 9, rucaCategory: 'Rural/Nonmetropolitan', htmAdvisor: 'Dr. J', status: 'Active' },
  { id: 5, muId: '52345678', firstName: 'Camila', lastName: 'Torres', email: 'ctorres@missouri.edu', programTrack: 'PAWS Scholar', classification: 'M2', cohortYear: 2020, cumulativeGpa: 3.64, scienceGpa: 3.55, rucaCode: 4, rucaCategory: 'Rural/Nonmetropolitan', htmAdvisor: 'Dr. Simmons', status: 'Active' }
]

export const SEEDED_COMPLIANCE = [
  { id: 1, studentId: 1, requirement: 'Annual Contract', status: 'Completed' },
  { id: 2, studentId: 1, requirement: 'HTM Meeting', status: 'Completed' },
  { id: 3, studentId: 2, requirement: 'Retreat Attendance', status: 'In Progress' },
  { id: 4, studentId: 3, requirement: 'MCAT Prep Attendance', status: 'Completed' },
  { id: 5, studentId: 4, requirement: 'CASE Meeting', status: 'Not Started' },
  { id: 6, studentId: 4, requirement: 'Well-Being Meeting', status: 'Not Started' },
  { id: 7, studentId: 5, requirement: 'MedPrep 2', status: 'Completed' }
]

export const SEEDED_ADVISING = [
  { id: 1, studentId: 1, date: '2026-02-12', type: 'HTM', concern: 'Low', summary: 'Discussed course planning and summer goals.', restricted: false },
  { id: 2, studentId: 2, date: '2026-03-04', type: 'CASE', concern: 'Moderate', summary: 'Needs structured MCAT study accountability.', restricted: false },
  { id: 3, studentId: 4, date: '2026-03-18', type: 'Well-Being', concern: 'High', summary: 'Sensitive well-being context. Visible only to users with restricted-note permission.', restricted: true }
]

export const SEEDED_SHADOWING = [
  { studentId: 1, vetting: 'Cleared', ready: true, match: 'Pending Match', specialty: '—', provider: '—' },
  { studentId: 2, vetting: 'In Progress', ready: false, match: 'Not Ready', specialty: '—', provider: '—' },
  { studentId: 3, vetting: 'Cleared', ready: true, match: 'Matched', specialty: 'Primary Care', provider: 'Dr. Henderson' },
  { studentId: 4, vetting: 'Cleared', ready: true, match: 'Pending Match', specialty: '—', provider: '—' },
  { studentId: 5, vetting: 'Completed', ready: true, match: 'Completed', specialty: 'Internal Medicine', provider: 'Dr. Clay' }
]

export const SEEDED_AUDIT = [
  { action: 'LOGIN', object: 'Demo Session', user: 'System Admin', time: 'Today, 2:04 PM' },
  { action: 'EXPORT', object: 'PAWS Workbook', user: 'Program Admin', time: 'Today, 1:42 PM' },
  { action: 'UPDATE', object: 'Student Requirement Status', user: 'Program Staff', time: 'Today, 11:08 AM' },
  { action: 'VIEW_RESTRICTED', object: 'Advising Note', user: 'Restricted Notes Viewer', time: 'Yesterday, 4:19 PM' },
  { action: 'IMPORT', object: 'Student Records', user: 'System Admin', time: 'Yesterday, 9:34 AM' }
]

export const GPA_TREND = [
  { term: 'Fall 2024', cumulative: 3.18, science: 3.05 },
  { term: 'Spring 2025', cumulative: 3.27, science: 3.12 },
  { term: 'Fall 2025', cumulative: 3.34, science: 3.21 },
  { term: 'Spring 2026', cumulative: 3.48, science: 3.36 }
]
