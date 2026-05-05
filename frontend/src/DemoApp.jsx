import { useMemo, useState } from 'react'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, LineChart, Line, Legend } from 'recharts'
import './demo.css'

const CYCLE = '2025-2026'

const students = [
  { id: 1, muId: '12345678', firstName: 'Aaliyah', lastName: 'Brooks', email: 'abrooks@missouri.edu', programTrack: 'JPAWS', classification: 'Sophomore', cohortYear: 2024, cumulativeGpa: 3.72, scienceGpa: 3.61, rucaCode: 2, rucaCategory: 'Urban/Metropolitan', htmAdvisor: 'Dr. J', status: 'Active' },
  { id: 2, muId: '22345678', firstName: 'Marcus', lastName: 'Carter', email: 'mcarter@missouri.edu', programTrack: 'PAWS Achiever', classification: 'Junior', cohortYear: 2023, cumulativeGpa: 3.28, scienceGpa: 3.04, rucaCode: 7, rucaCategory: 'Rural/Nonmetropolitan', htmAdvisor: 'Dr. Simmons', status: 'Active' },
  { id: 3, muId: '32345678', firstName: 'Sofia', lastName: 'Nguyen', email: 'snguyen@missouri.edu', programTrack: 'PAWS Pre-Admissions', classification: 'Senior', cohortYear: 2022, cumulativeGpa: 3.86, scienceGpa: 3.79, rucaCode: 1, rucaCategory: 'Urban/Metropolitan', htmAdvisor: 'Dr. Kelley', status: 'Active' },
  { id: 4, muId: '42345678', firstName: 'Elijah', lastName: 'Reed', email: 'ereed@missouri.edu', programTrack: 'PAWS', classification: 'Junior', cohortYear: 2021, cumulativeGpa: 2.91, scienceGpa: 2.74, rucaCode: 9, rucaCategory: 'Rural/Nonmetropolitan', htmAdvisor: 'Dr. J', status: 'Active' },
  { id: 5, muId: '52345678', firstName: 'Camila', lastName: 'Torres', email: 'ctorres@missouri.edu', programTrack: 'PAWS Scholar', classification: 'M2', cohortYear: 2020, cumulativeGpa: 3.64, scienceGpa: 3.55, rucaCode: 4, rucaCategory: 'Rural/Nonmetropolitan', htmAdvisor: 'Dr. Simmons', status: 'Active' }
]

const compliance = [
  { studentId: 1, requirement: 'Annual Contract', status: 'Completed' },
  { studentId: 1, requirement: 'HTM Meeting', status: 'Completed' },
  { studentId: 2, requirement: 'Annual Contract', status: 'Completed' },
  { studentId: 2, requirement: 'Retreat Attendance', status: 'In Progress' },
  { studentId: 3, requirement: 'MCAT Prep Attendance', status: 'Completed' },
  { studentId: 4, requirement: 'CASE Meeting', status: 'Not Started' },
  { studentId: 4, requirement: 'Well-Being Meeting', status: 'Not Started' },
  { studentId: 5, requirement: 'MedPrep 2', status: 'Completed' }
]

const advising = [
  { id: 1, studentId: 1, date: '2026-02-12', type: 'HTM', concern: 'Low', followUp: false, summary: 'Discussed course planning and summer goals.', restricted: false },
  { id: 2, studentId: 2, date: '2026-03-04', type: 'CASE', concern: 'Moderate', followUp: true, summary: 'Needs structured MCAT study accountability.', restricted: false },
  { id: 3, studentId: 4, date: '2026-03-18', type: 'Well-Being', concern: 'High', followUp: true, summary: 'Restricted note hidden unless permission is enabled.', restricted: true },
  { id: 4, studentId: 3, date: '2026-04-01', type: 'MedOpp', concern: 'Low', followUp: false, summary: 'Reviewed application readiness and interview preparation.', restricted: false }
]

const shadowing = [
  { studentId: 1, eligibility: 'Eligible', vetting: 'Cleared', ready: true, match: 'Pending Match', specialty: '', provider: '' },
  { studentId: 2, eligibility: 'Eligible', vetting: 'In Progress', ready: false, match: 'Not Ready', specialty: '', provider: '' },
  { studentId: 3, eligibility: 'Eligible', vetting: 'Cleared', ready: true, match: 'Matched', specialty: 'Primary Care', provider: 'Dr. Henderson' },
  { studentId: 4, eligibility: 'Eligible', vetting: 'Cleared', ready: true, match: 'Pending Match', specialty: '', provider: '' },
  { studentId: 5, eligibility: 'Eligible', vetting: 'Completed', ready: true, match: 'Completed', specialty: 'Internal Medicine', provider: 'Dr. Clay' }
]

const gpaTrend = [
  { term: 'Fall 2024', cumulative: 3.18, science: 3.05 },
  { term: 'Spring 2025', cumulative: 3.27, science: 3.12 },
  { term: 'Fall 2025', cumulative: 3.34, science: 3.21 },
  { term: 'Spring 2026', cumulative: 3.48, science: 3.36 }
]

function countBy(key, data = students) {
  return Object.values(data.reduce((acc, item) => {
    const label = item[key] || 'Unknown'
    acc[label] = acc[label] || { name: label, count: 0 }
    acc[label].count += 1
    return acc
  }, {}))
}

export default function DemoApp() {
  const [view, setView] = useState('dashboard')
  const [selectedId, setSelectedId] = useState(1)
  const [canViewRestricted, setCanViewRestricted] = useState(false)

  const selectedStudent = students.find(s => s.id === Number(selectedId)) || students[0]
  const studentNotes = advising.filter(a => a.studentId === selectedStudent.id && (!a.restricted || canViewRestricted))
  const missingItems = compliance.filter(c => c.status !== 'Completed' && c.status !== 'Waived')
  const completed = compliance.filter(c => c.status === 'Completed' || c.status === 'Waived').length
  const complianceRate = Math.round((completed / compliance.length) * 100)
  const avgGpa = (students.reduce((sum, s) => sum + s.cumulativeGpa, 0) / students.length).toFixed(2)
  const ruralPct = Math.round((students.filter(s => s.rucaCategory === 'Rural/Nonmetropolitan').length / students.length) * 100)

  const riskData = useMemo(() => {
    return students.map(s => {
      const missingCount = compliance.filter(c => c.studentId === s.id && c.status !== 'Completed').length
      const score = (missingCount > 1 ? 2 : 0) + (s.cumulativeGpa < 3.0 ? 2 : 0) + (s.scienceGpa < 2.8 ? 2 : 0)
      const level = score >= 4 ? 'High' : score >= 2 ? 'Moderate' : 'Low'
      return { ...s, missingCount, score, level }
    })
  }, [])

  const riskSummary = countBy('level', riskData)

  return (
    <div className="demo-shell">
      <aside className="demo-sidebar">
        <div className="mu-lockup"><span>MU</span><div><strong>PAWS</strong><small>Program Management System</small></div></div>
        <button className={view === 'dashboard' ? 'active' : ''} onClick={() => setView('dashboard')}>Dashboard</button>
        <button className={view === 'students' ? 'active' : ''} onClick={() => setView('students')}>Students</button>
        <button className={view === 'advising' ? 'active' : ''} onClick={() => setView('advising')}>Advising</button>
        <button className={view === 'shadowing' ? 'active' : ''} onClick={() => setView('shadowing')}>Shadowing</button>
        <button className={view === 'analytics' ? 'active' : ''} onClick={() => setView('analytics')}>Analytics</button>
        <button className={view === 'exports' ? 'active' : ''} onClick={() => setView('exports')}>Exports</button>
      </aside>

      <main className="demo-main">
        <header className="demo-header">
          <div><p>JPAWS / PAWS</p><h1>{titleFor(view)}</h1></div>
          <div className="header-actions"><span>{CYCLE}</span><button>Refresh Demo</button></div>
        </header>

        {view === 'dashboard' && <Dashboard avgGpa={avgGpa} complianceRate={complianceRate} ruralPct={ruralPct} missingItems={missingItems} riskSummary={riskSummary} />}
        {view === 'students' && <Students />}
        {view === 'advising' && <Advising selectedId={selectedId} setSelectedId={setSelectedId} selectedStudent={selectedStudent} studentNotes={studentNotes} canViewRestricted={canViewRestricted} setCanViewRestricted={setCanViewRestricted} />}
        {view === 'shadowing' && <Shadowing />}
        {view === 'analytics' && <Analytics riskSummary={riskSummary} />}
        {view === 'exports' && <Exports />}
      </main>
    </div>
  )
}

function Dashboard({ avgGpa, complianceRate, ruralPct, missingItems, riskSummary }) {
  return <>
    <section className="metric-row">
      <Metric label="Active Students" value={students.length} helper="Demo cohort records" />
      <Metric label="Compliance Rate" value={`${complianceRate}%`} helper="Completed or waived" />
      <Metric label="Average GPA" value={avgGpa} helper="Current cumulative GPA" />
      <Metric label="Rural Reach" value={`${ruralPct}%`} helper="RUCA 4–10" />
    </section>
    <section className="grid two">
      <Card title="Program Distribution"><ResponsiveContainer width="100%" height={260}><BarChart data={countBy('programTrack')}><XAxis dataKey="name" /><YAxis /><Tooltip /><Bar dataKey="count" radius={[8,8,0,0]} /></BarChart></ResponsiveContainer></Card>
      <Card title="Risk Signal Distribution"><ResponsiveContainer width="100%" height={260}><BarChart data={riskSummary}><XAxis dataKey="name" /><YAxis /><Tooltip /><Bar dataKey="count" radius={[8,8,0,0]} /></BarChart></ResponsiveContainer></Card>
    </section>
    <Card title="Intervention Queue"><div className="queue-list">{missingItems.map((m, i) => <div className="queue-item" key={i}><span>Student #{m.studentId}</span><strong>{m.requirement}</strong><em>{m.status}</em></div>)}</div></Card>
  </>
}

function Students() {
  return <Card title="Student Master List"><table><thead><tr><th>Name</th><th>Program</th><th>Classification</th><th>GPA</th><th>Science GPA</th><th>RUCA</th><th>HTM</th></tr></thead><tbody>{students.map(s => <tr key={s.id}><td><strong>{s.firstName} {s.lastName}</strong><br/><span>{s.email}</span></td><td>{s.programTrack}</td><td>{s.classification}</td><td>{s.cumulativeGpa}</td><td>{s.scienceGpa}</td><td>{s.rucaCode} · {s.rucaCategory}</td><td>{s.htmAdvisor}</td></tr>)}</tbody></table></Card>
}

function Advising({ selectedId, setSelectedId, selectedStudent, studentNotes, canViewRestricted, setCanViewRestricted }) {
  return <section className="grid two uneven">
    <Card title="Advising Controls">
      <label>Student<select value={selectedId} onChange={e => setSelectedId(e.target.value)}>{students.map(s => <option key={s.id} value={s.id}>{s.lastName}, {s.firstName}</option>)}</select></label>
      <div className="toggle-row"><span>Restricted Note Permission</span><button className={canViewRestricted ? 'toggle on' : 'toggle'} onClick={() => setCanViewRestricted(!canViewRestricted)}>{canViewRestricted ? 'Enabled' : 'Hidden'}</button></div>
      <div className="student-card"><h3>{selectedStudent.firstName} {selectedStudent.lastName}</h3><p>{selectedStudent.programTrack} · {selectedStudent.classification}</p><p>HTM: {selectedStudent.htmAdvisor}</p></div>
    </Card>
    <Card title="Advising Notes">
      <table><thead><tr><th>Date</th><th>Type</th><th>Concern</th><th>Summary</th><th>Restricted</th></tr></thead><tbody>{studentNotes.map(n => <tr key={n.id} className={n.restricted ? 'restricted' : ''}><td>{n.date}</td><td>{n.type}</td><td>{n.concern}</td><td>{n.summary}</td><td>{n.restricted ? 'Yes' : 'No'}</td></tr>)}</tbody></table>
      {!canViewRestricted && <p className="notice">Restricted notes are hidden for this permission state.</p>}
    </Card>
  </section>
}

function Shadowing() {
  return <Card title="Shadowing Pipeline"><table><thead><tr><th>Student</th><th>Vetting</th><th>Ready</th><th>Match</th><th>Specialty</th><th>Provider</th></tr></thead><tbody>{shadowing.map(row => { const s = students.find(x => x.id === row.studentId); return <tr key={row.studentId}><td>{s?.firstName} {s?.lastName}</td><td>{row.vetting}</td><td>{row.ready ? 'Yes' : 'No'}</td><td>{row.match}</td><td>{row.specialty || '—'}</td><td>{row.provider || '—'}</td></tr> })}</tbody></table></Card>
}

function Analytics({ riskSummary }) {
  return <section className="grid two">
    <Card title="GPA Trend"><ResponsiveContainer width="100%" height={260}><LineChart data={gpaTrend}><XAxis dataKey="term" /><YAxis domain={[2.5, 4]} /><Tooltip /><Legend /><Line type="monotone" dataKey="cumulative" strokeWidth={3} /><Line type="monotone" dataKey="science" strokeWidth={3} /></LineChart></ResponsiveContainer></Card>
    <Card title="Rurality Distribution"><ResponsiveContainer width="100%" height={260}><PieChart><Pie data={countBy('rucaCategory')} dataKey="count" nameKey="name" outerRadius={90} label>{countBy('rucaCategory').map((_, i) => <Cell key={i} />)}</Pie><Tooltip /></PieChart></ResponsiveContainer></Card>
    <Card title="Risk Signals"><ResponsiveContainer width="100%" height={260}><BarChart data={riskSummary}><XAxis dataKey="name" /><YAxis /><Tooltip /><Bar dataKey="count" radius={[8,8,0,0]} /></BarChart></ResponsiveContainer></Card>
  </section>
}

function Exports() {
  return <Card title="Export Center"><div className="export-grid"><ExportButton title="Full Excel Workbook" desc="Students, compliance, GPA, shadowing, alumni" /><ExportButton title="Student CSV" desc="Clean student dataset" /><ExportButton title="Compliance CSV" desc="Requirement completion by cycle" /><ExportButton title="Faculty PDF Report" desc="Leadership and grant-ready summary" /></div></Card>
}

function ExportButton({ title, desc }) { return <button className="export-card"><strong>{title}</strong><span>{desc}</span></button> }
function Card({ title, children }) { return <section className="demo-card"><h2>{title}</h2>{children}</section> }
function Metric({ label, value, helper }) { return <div className="demo-metric"><span>{label}</span><strong>{value}</strong><small>{helper}</small></div> }
function titleFor(view) { return ({ dashboard: 'Executive Dashboard', students: 'Student Records', advising: 'Advising & Restricted Notes', shadowing: 'Shadowing Pipeline', analytics: 'Analytics & Outcomes', exports: 'Export Center' })[view] || 'Dashboard' }
