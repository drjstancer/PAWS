export const DEMO_USERS = [
  {
    id: 1,
    name: 'Dr. Joel Stancer',
    email: 'admin@paws.demo',
    role: 'System Admin',
    permissions: [
      'Students.View',
      'Students.Edit',
      'Academic.View',
      'Academic.Edit',
      'Requirements.View',
      'Requirements.Edit',
      'Requirements.Generate',
      'Compliance.View',
      'Shadowing.View',
      'Shadowing.Edit',
      'Advising.View',
      'Advising.Create',
      'Advising.ViewRestricted',
      'Events.View',
      'Events.Edit',
      'Alumni.View',
      'Alumni.Edit',
      'Reports.View',
      'Reports.Export',
      'Analytics.View',
      'Users.Manage',
      'Audit.View'
    ]
  },
  {
    id: 2,
    name: 'Program Admin Demo',
    email: 'program@paws.demo',
    role: 'Program Admin',
    permissions: [
      'Students.View',
      'Students.Edit',
      'Academic.View',
      'Academic.Edit',
      'Requirements.View',
      'Requirements.Edit',
      'Requirements.Generate',
      'Compliance.View',
      'Shadowing.View',
      'Shadowing.Edit',
      'Advising.View',
      'Advising.Create',
      'Events.View',
      'Events.Edit',
      'Alumni.View',
      'Alumni.Edit',
      'Reports.View',
      'Reports.Export',
      'Analytics.View'
    ]
  },
  {
    id: 3,
    name: 'Advisor Demo',
    email: 'advisor@paws.demo',
    role: 'Advisor / HTM',
    permissions: [
      'Students.View',
      'Academic.View',
      'Compliance.View',
      'Advising.View',
      'Advising.Create',
      'Shadowing.View',
      'Reports.View'
    ]
  },
  {
    id: 4,
    name: 'Leadership Viewer Demo',
    email: 'leadership@paws.demo',
    role: 'Leadership Viewer',
    permissions: [
      'Students.View',
      'Compliance.View',
      'Reports.View',
      'Analytics.View'
    ]
  },
  {
    id: 5,
    name: 'Restricted Notes Viewer Demo',
    email: 'restricted@paws.demo',
    role: 'Restricted Notes Viewer',
    permissions: [
      'Students.View',
      'Academic.View',
      'Compliance.View',
      'Advising.View',
      'Advising.Create',
      'Advising.ViewRestricted',
      'Reports.View'
    ]
  }
]