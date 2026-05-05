import { useEffect, useMemo, useState } from 'react'
import './styles.css'

const PROGRAMS = ['', 'JPAWS', 'PAWS', 'PAWS Achiever', 'PAWS Pre-Admissions', 'PAWS Scholar']
const CLASSIFICATIONS = ['', 'Freshman', 'Sophomore', 'Junior', 'Senior', 'Post-Bacc', 'M1', 'M2', 'M3', 'M4', 'Resident', 'Graduate', 'Alum']
const CYCLE = '2025-2026'

export default function App() {
  const [students, setStudents] = useState([])
  const [compliance, setCompliance] = useState(null)
  const [missing, setMissing] = useState([])
  const [program, setProgram] = useState('')
  const [classification, setClassification] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const loadStudents = async () => {
    setLoading(true)
    setError('')
    try {
      const params = new URLSearchParams()
      if (program) params.set('program', program)
      if (classification) params.set('classification', classification)
      const url = `/api/students${params.toString() ? `?${params.toString()}` : ''}`
      const res = await fetch(url)
      if (!res.ok) throw new Error('Unable to load students')
      setStudents(await res.json())
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  const loadCompliance = async () => {
    try {
      const res = await fetch(`/api/compliance/dashboard?cycle=${CYCLE}`)
      if (!res.ok) throw new Error('Unable to load compliance dashboard')
      setCompliance(await res.json())
    } catch (err) {
      setError(err.message)
    }
  }

  const loadMissing = async () => {
    try {
      const res = await fetch(`/api/compliance/missing?cycle=${CYCLE}`)
      if (!res.ok) throw new Error('Unable to load missing requirements')
      setMissing(await res.json())
    } catch (err) {
      setError(err.message)
    }
  }

  const refreshAll = async () => {
    await Promise.all([loadStudents(), loadCompliance(), loadMissing()])
  }

  useEffect(() => {
    refreshAll()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const averageGpa = useMemo(() => {
    const values = students.map(s => Number(s.cumulativeGpa)).filter(Boolean)
    if (!values.length) return '—'
    return (values.reduce((a, b) => a + b, 0) / values.length).toFixed(2)
  }, [students])

  const interventionCount = missing.length
  const complianceRate = compliance?.complianceRate ?? 0

  const generateRequirements = async () => {
    if (!program || !classification) {
      setError('Choose both a program and classification before bulk generation.')
      return
    }

    setLoading(true)
    setError('')
    try {
      const params = new URLSearchParams({ program, classification, cycle: CYCLE })
      const res = await fetch(`/api/requirements/bulk/generate?${params.toString()}`, { method: 'POST' })
      if (!res.ok) throw new Error('Unable to generate requirements')
      await refreshAll()
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  const exportStudents = () => {
    const headers = ['MU ID', 'First Name', 'Last Name', 'Program', 'Classification', 'Cumulative GPA', 'Science GPA', 'RUCA Code', 'RUCA Category', 'HTM']
    const rows = students.map(s => [s.muId, s.firstName, s.lastName, s.programTrack, s.classification, s.cumulativeGpa ?? '', s.scienceGpa ?? '', s.rucaCode ?? '', s.rucaCategory ?? '', s.htmAdvisor ?? ''])
    const csv = [headers, ...rows].map(row => row.map(value => `"${String(value).replace(/"/g, '""')}"`).join(',')).join('\n')
    const blob = new Blob([csv], { type: 'text/csv' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `paws_students_${CYCLE}.csv`
    link.click()
    URL.revokeObjectURL(url)
  }

  return (
    <div className="app-shell">
      <nav className="topbar">
        <div className="brand-block">
          <div className="brand-mark">MU</div>
          <div>
            <div className="eyebrow">School of Medicine</div>
            <div className="brand-title">PAWS Program Admin</div>
          </div>
        </div>
        <div className="user-block">
          <span>Pathways & Outreach</span>
          <button className="link-button">Sign Out</button>
        </div>
      </nav>

      <header className="hero">
        <div>
          <p className="eyebrow dark">JPAWS / PAWS Data Management</p>
          <h1>Program Compliance Dashboard</h1>
          <p>Track pathway participation, requirements, GPA indicators, and intervention needs for the {CYCLE} cycle.</p>
        </div>
        <div className="hero-actions">
          <button className="btn secondary" onClick={refreshAll} disabled={loading}>{loading ? 'Refreshing...' : 'Refresh'}</button>
          <button className="btn primary" onClick={exportStudents}>Export Students</button>
        </div>
      </header>

      {error && <div className="alert">{error}</div>}

      <section className="metric-grid">
        <Metric title="Active Records Loaded" value={students.length} note="Current filtered student list" />
        <Metric title="Compliance Rate" value={`${complianceRate}%`} note="Completed or waived requirements" />
        <Metric title="Missing Items" value={interventionCount} note="Current intervention queue" />
        <Metric title="Average GPA" value={averageGpa} note="From currently loaded records" />
      </section>

      <section className="panel filters-panel">
        <div>
          <h2>Filters & Actions</h2>
          <p>Use these controls to narrow the student list or bulk-generate requirements for a program/classification group.</p>
        </div>
        <div className="filter-row">
          <label>
            Program
            <select value={program} onChange={e => setProgram(e.target.value)}>
              {PROGRAMS.map(p => <option key={p} value={p}>{p || 'All Programs'}</option>)}
            </select>
          </label>
          <label>
            Classification
            <select value={classification} onChange={e => setClassification(e.target.value)}>
              {CLASSIFICATIONS.map(c => <option key={c} value={c}>{c || 'All Classifications'}</option>)}
            </select>
          </label>
          <button className="btn secondary" onClick={loadStudents}>Apply Filters</button>
          <button className="btn warning" onClick={generateRequirements}>Generate Requirements</button>
        </div>
      </section>

      <main className="dashboard-grid">
        <section className="panel wide">
          <div className="panel-header">
            <div>
              <h2>Student Master List</h2>
              <p>Digital replacement for program/classification tabs.</p>
            </div>
          </div>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>MU ID</th>
                  <th>Name</th>
                  <th>Program</th>
                  <th>Classification</th>
                  <th>GPA</th>
                  <th>Science GPA</th>
                  <th>RUCA</th>
                  <th>HTM</th>
                </tr>
              </thead>
              <tbody>
                {students.map(s => (
                  <tr key={s.id}>
                    <td>{s.muId}</td>
                    <td><strong>{s.firstName} {s.lastName}</strong><br /><span>{s.email}</span></td>
                    <td><span className="badge">{s.programTrack}</span></td>
                    <td>{s.classification}</td>
                    <td>{s.cumulativeGpa ?? '—'}</td>
                    <td>{s.scienceGpa ?? '—'}</td>
                    <td>{s.rucaCode ? `${s.rucaCode} · ${s.rucaCategory}` : '—'}</td>
                    <td>{s.htmAdvisor ?? '—'}</td>
                  </tr>
                ))}
                {!students.length && <tr><td colSpan="8" className="empty">No students found.</td></tr>}
              </tbody>
            </table>
          </div>
        </section>

        <section className="panel">
          <h2>Intervention Queue</h2>
          <p>Students with missing or incomplete requirements for {CYCLE}.</p>
          <div className="queue-list">
            {missing.slice(0, 12).map(item => (
              <div className="queue-card" key={item.id}>
                <div>
                  <strong>Student #{item.studentId}</strong>
                  <span>Requirement #{item.requirementId}</span>
                </div>
                <span className="status-pill">{item.status}</span>
              </div>
            ))}
            {!missing.length && <div className="empty-card">No missing requirements found for this cycle.</div>}
          </div>
        </section>
      </main>

      <footer className="footer">Copyright © 2026 — University of Missouri. Internal program administration prototype.</footer>
    </div>
  )
}

function Metric({ title, value, note }) {
  return (
    <div className="metric-card">
      <span>{title}</span>
      <strong>{value}</strong>
      <small>{note}</small>
    </div>
  )
}
