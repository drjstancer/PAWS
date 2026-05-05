import React from 'react'

const users = [
  { name: 'Dr. Joel Stancer', email: 'stancerj@health.missouri.edu', role: 'System Admin', status: 'Active' },
  { name: 'Dr. Andrea Simmons', email: 'simmonsa@health.missouri.edu', role: 'Program Admin', status: 'Active' },
  { name: 'Dr. Laura Henderson Kelley', email: 'kelleyLH@health.missouri.edu', role: 'Leadership Viewer', status: 'Active' },
  { name: 'Advisor Demo', email: 'advisor@missouri.edu', role: 'Advisor', status: 'Limited' }
]

const requirements = [
  { name: 'Annual Contract Submission', category: 'Form', appliesTo: 'All active participants', status: 'Active' },
  { name: 'Retreat Attendance', category: 'Event', appliesTo: 'Juniors', status: 'Active' },
  { name: 'HTM Meeting', category: 'Meeting', appliesTo: 'All tracks', status: 'Active' },
  { name: 'MCAT Prep Attendance', category: 'Academic Support', appliesTo: 'PAWS Achievers / Pre-Admissions', status: 'Active' }
]

const auditEvents = [
  { action: 'EXPORT', object: 'PAWS Workbook', user: 'Dr. Joel Stancer', time: 'Today, 1:42 PM' },
  { action: 'UPDATE', object: 'Student Requirement Status', user: 'Program Staff', time: 'Today, 11:08 AM' },
  { action: 'VIEW_RESTRICTED', object: 'Advising Note', user: 'Restricted Notes Viewer', time: 'Yesterday, 4:19 PM' },
  { action: 'IMPORT', object: 'Student Records', user: 'System Admin', time: 'Yesterday, 9:34 AM' }
]

export default function AdminConsole() {
  return (
    <div className="admin-console">
      <section className="admin-hero">
        <div>
          <span>Administration Layer</span>
          <h2>System Admin Console</h2>
          <p>Manage users, roles, requirement rules, imports, exports, reference tables, and audit visibility.</p>
        </div>
        <button className="btn-like">Run System Check</button>
      </section>

      <section className="metric-row">
        <div className="demo-metric"><span>Active Users</span><strong>4</strong><small>Role-assigned demo users</small></div>
        <div className="demo-metric"><span>Requirements</span><strong>9</strong><small>Seeded compliance rules</small></div>
        <div className="demo-metric"><span>Audit Events</span><strong>128</strong><small>Tracked system actions</small></div>
        <div className="demo-metric"><span>System Status</span><strong>OK</strong><small>API, DB, exports available</small></div>
      </section>

      <section className="grid two">
        <AdminCard title="User & Role Management">
          <table>
            <thead><tr><th>User</th><th>Role</th><th>Status</th></tr></thead>
            <tbody>
              {users.map(u => (
                <tr key={u.email}>
                  <td><strong>{u.name}</strong><br /><span>{u.email}</span></td>
                  <td>{u.role}</td>
                  <td><span className="admin-pill">{u.status}</span></td>
                </tr>
              ))}
            </tbody>
          </table>
        </AdminCard>

        <AdminCard title="Requirement Catalog">
          <table>
            <thead><tr><th>Requirement</th><th>Category</th><th>Applies To</th><th>Status</th></tr></thead>
            <tbody>
              {requirements.map(r => (
                <tr key={r.name}>
                  <td><strong>{r.name}</strong></td>
                  <td>{r.category}</td>
                  <td>{r.appliesTo}</td>
                  <td><span className="admin-pill">{r.status}</span></td>
                </tr>
              ))}
            </tbody>
          </table>
        </AdminCard>
      </section>

      <section className="grid two">
        <AdminCard title="Import & Export Center">
          <div className="admin-action-grid">
            <button className="export-card"><strong>Import Students</strong><span>Upload student master list</span></button>
            <button className="export-card"><strong>Generate Requirements</strong><span>Create cycle requirements</span></button>
            <button className="export-card"><strong>Export Workbook</strong><span>Download full XLSX dataset</span></button>
            <button className="export-card"><strong>Faculty PDF</strong><span>Create grant-ready report</span></button>
          </div>
        </AdminCard>

        <AdminCard title="Audit Log Preview">
          <div className="queue-list">
            {auditEvents.map((event, index) => (
              <div className="queue-item" key={index}>
                <span>{event.time}</span>
                <strong>{event.action} · {event.object}</strong>
                <em>{event.user}</em>
              </div>
            ))}
          </div>
        </AdminCard>
      </section>
    </div>
  )
}

function AdminCard({ title, children }) {
  return <section className="demo-card"><h2>{title}</h2>{children}</section>
}