import { useEffect, useState } from 'react'

export default function App() {
  const [students, setStudents] = useState([])
  const [compliance, setCompliance] = useState(null)
  const [program, setProgram] = useState('')
  const [classification, setClassification] = useState('')

  const loadStudents = async () => {
    let url = '/api/students'
    if (program || classification) {
      url += `?program=${program}&classification=${classification}`
    }
    const res = await fetch(url)
    const data = await res.json()
    setStudents(data)
  }

  const loadCompliance = async () => {
    const res = await fetch('/api/compliance/dashboard?cycle=2025-2026')
    const data = await res.json()
    setCompliance(data)
  }

  useEffect(() => {
    loadStudents()
    loadCompliance()
  }, [])

  return (
    <div style={{ padding: '20px', fontFamily: 'Arial' }}>
      <h1>PAWS Program Dashboard</h1>

      <div style={{ marginBottom: '20px' }}>
        <input placeholder="Program" value={program} onChange={e => setProgram(e.target.value)} />
        <input placeholder="Classification" value={classification} onChange={e => setClassification(e.target.value)} />
        <button onClick={loadStudents}>Filter</button>
      </div>

      {compliance && (
        <div style={{ marginBottom: '20px' }}>
          <h2>Compliance</h2>
          <p>Total: {compliance.total}</p>
          <p>Completed: {compliance.completed}</p>
          <p>Rate: {compliance.complianceRate}%</p>
        </div>
      )}

      <h2>Students</h2>
      <table border="1" cellPadding="5">
        <thead>
          <tr>
            <th>MU ID</th>
            <th>Name</th>
            <th>Program</th>
            <th>Classification</th>
            <th>GPA</th>
          </tr>
        </thead>
        <tbody>
          {students.map(s => (
            <tr key={s.id}>
              <td>{s.muId}</td>
              <td>{s.firstName} {s.lastName}</td>
              <td>{s.programTrack}</td>
              <td>{s.classification}</td>
              <td>{s.cumulativeGpa}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
