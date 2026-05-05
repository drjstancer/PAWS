import { useEffect, useMemo, useState } from 'react'

export default function AdvisingDashboard({ students = [] }) {
  const [currentUser, setCurrentUser] = useState(null)
  const [selectedStudentId, setSelectedStudentId] = useState('')
  const [meetings, setMeetings] = useState([])
  const [summary, setSummary] = useState({ total: 0, followUps: 0, highConcern: 0 })
  const [error, setError] = useState('')
  const [form, setForm] = useState({
    advisorId: '',
    meetingType: 'HTM',
    meetingMode: 'In Person',
    meetingDate: '',
    requiredMeeting: true,
    meetingSummary: '',
    followUpNeeded: false,
    followUpDate: '',
    concernLevel: 'Low',
    referralMade: false,
    referralType: '',
    restrictedNote: false
  })

  useEffect(() => {
    fetch('/api/v1/me')
      .then(r => r.json())
      .then(setCurrentUser)
      .catch(() => setCurrentUser({ permissions: [] }))
  }, [])

  const permissions = currentUser?.permissions || currentUser?.Permissions || []
  const canCreate = permissions.includes('Advising.Create')
  const canViewRestricted = permissions.includes('Advising.ViewRestricted')

  const selectedStudent = useMemo(
    () => students.find(s => String(s.id) === String(selectedStudentId)),
    [students, selectedStudentId]
  )

  const loadMeetings = async studentId => {
    if (!studentId) return
    setError('')
    const res = await fetch(`/api/v1/advising/${studentId}`)
    if (!res.ok) {
      setError('Unable to load advising meetings.')
      setMeetings([])
      return
    }
    const data = await res.json()
    setMeetings(data)
    setSummary({
      total: data.length,
      followUps: data.filter(m => m.followUpNeeded).length,
      highConcern: data.filter(m => String(m.concernLevel).toLowerCase() === 'high').length
    })
  }

  const handleStudentChange = e => {
    setSelectedStudentId(e.target.value)
    loadMeetings(e.target.value)
  }

  const submitMeeting = async e => {
    e.preventDefault()
    if (!selectedStudentId) {
      setError('Select a student before adding an advising meeting.')
      return
    }
    if (form.restrictedNote && !canViewRestricted) {
      setError('You do not have permission to create restricted notes.')
      return
    }
    if (form.followUpNeeded && !form.followUpDate) {
      setError('Follow-up date is required when follow-up is needed.')
      return
    }

    const payload = {
      studentId: Number(selectedStudentId),
      advisorId: Number(form.advisorId || 0),
      meetingType: form.meetingType,
      meetingDate: form.meetingDate,
      meetingMode: form.meetingMode,
      requiredMeeting: form.requiredMeeting,
      meetingSummary: form.meetingSummary,
      followUpNeeded: form.followUpNeeded,
      followUpDate: form.followUpNeeded ? form.followUpDate : null,
      concernLevel: form.concernLevel,
      referralMade: form.referralMade,
      referralType: form.referralMade ? form.referralType : null,
      restrictedNote: form.restrictedNote
    }

    const res = await fetch('/api/v1/advising', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })

    if (!res.ok) {
      setError('Unable to save advising meeting. Check permissions and required fields.')
      return
    }

    setForm({ ...form, meetingSummary: '', followUpNeeded: false, followUpDate: '', restrictedNote: false })
    await loadMeetings(selectedStudentId)
  }

  return (
    <section className="panel advising-dashboard">
      <div className="panel-header">
        <div>
          <h2>Advising Dashboard</h2>
          <p>Track HTM, CASE, MedOpp, and well-being meetings with restricted-note safeguards.</p>
        </div>
      </div>

      {error && <div className="alert">{error}</div>}

      <div className="filter-row">
        <label>
          Student
          <select value={selectedStudentId} onChange={handleStudentChange}>
            <option value="">Select student</option>
            {students.map(s => (
              <option key={s.id} value={s.id}>{s.lastName}, {s.firstName} — {s.programTrack}</option>
            ))}
          </select>
        </label>
        <div className="permission-note">
          Restricted notes: <strong>{canViewRestricted ? 'Enabled' : 'Hidden'}</strong>
        </div>
      </div>

      {selectedStudent && (
        <div className="metric-grid compact">
          <MetricLite title="Meetings" value={summary.total} />
          <MetricLite title="Follow-ups" value={summary.followUps} />
          <MetricLite title="High Concern" value={summary.highConcern} />
        </div>
      )}

      {canCreate && selectedStudentId && (
        <form className="advising-form" onSubmit={submitMeeting}>
          <h3>Add Advising Meeting</h3>
          <div className="filter-row">
            <label>Advisor ID<input value={form.advisorId} onChange={e => setForm({ ...form, advisorId: e.target.value })} /></label>
            <label>Meeting Type
              <select value={form.meetingType} onChange={e => setForm({ ...form, meetingType: e.target.value })}>
                <option>HTM</option><option>CASE</option><option>MedOpp</option><option>Well-Being</option><option>Other</option>
              </select>
            </label>
            <label>Date<input type="date" value={form.meetingDate} onChange={e => setForm({ ...form, meetingDate: e.target.value })} /></label>
            <label>Concern
              <select value={form.concernLevel} onChange={e => setForm({ ...form, concernLevel: e.target.value })}>
                <option>Low</option><option>Moderate</option><option>High</option>
              </select>
            </label>
          </div>
          <label>Meeting Summary<textarea value={form.meetingSummary} onChange={e => setForm({ ...form, meetingSummary: e.target.value })} /></label>
          <div className="check-row">
            <label><input type="checkbox" checked={form.followUpNeeded} onChange={e => setForm({ ...form, followUpNeeded: e.target.checked })} /> Follow-up needed</label>
            {form.followUpNeeded && <label>Follow-up date<input type="date" value={form.followUpDate} onChange={e => setForm({ ...form, followUpDate: e.target.value })} /></label>}
            {canViewRestricted && <label><input type="checkbox" checked={form.restrictedNote} onChange={e => setForm({ ...form, restrictedNote: e.target.checked })} /> Restricted note</label>}
          </div>
          <button className="btn primary" type="submit">Save Meeting</button>
        </form>
      )}

      <div className="table-wrap">
        <table>
          <thead><tr><th>Date</th><th>Type</th><th>Concern</th><th>Follow-up</th><th>Summary</th><th>Restricted</th></tr></thead>
          <tbody>
            {meetings.map(m => (
              <tr key={m.id} className={m.restrictedNote ? 'restricted-row' : ''}>
                <td>{m.meetingDate ? String(m.meetingDate).slice(0, 10) : '—'}</td>
                <td>{m.meetingType}</td>
                <td>{m.concernLevel || '—'}</td>
                <td>{m.followUpNeeded ? (m.followUpDate ? String(m.followUpDate).slice(0, 10) : 'Needed') : 'No'}</td>
                <td>{m.meetingSummary || '—'}</td>
                <td>{m.restrictedNote ? 'Restricted' : 'No'}</td>
              </tr>
            ))}
            {!meetings.length && <tr><td colSpan="6" className="empty">No advising meetings found.</td></tr>}
          </tbody>
        </table>
      </div>
    </section>
  )
}

function MetricLite({ title, value }) {
  return <div className="metric-card"><span>{title}</span><strong>{value}</strong></div>
}
