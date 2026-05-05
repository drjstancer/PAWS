import { useEffect, useState } from 'react'

export default function Analytics() {
  const [data, setData] = useState(null)

  useEffect(() => {
    fetch('/api/analytics/overview')
      .then(r => r.json())
      .then(setData)
  }, [])

  if (!data) return <div>Loading analytics...</div>

  return (
    <div style={{ padding: 20 }}>
      <h2>Analytics Dashboard</h2>
      <p>Total Students: {data.totalStudents}</p>
      <p>Active Students: {data.activeStudents}</p>

      <h3>By Program</h3>
      <ul>
        {data.byProgram.map(p => (
          <li key={p.programTrack}>
            {p.programTrack}: {p.count} (Avg GPA: {p.averageCumulativeGpa})
          </li>
        ))}
      </ul>

      <h3>By Classification</h3>
      <ul>
        {data.byClassification.map(c => (
          <li key={c.classification}>
            {c.classification}: {c.count}
          </li>
        ))}
      </ul>

      <h3>Rurality</h3>
      <ul>
        {data.byRuca.map(r => (
          <li key={r.rucaCategory}>
            {r.rucaCategory}: {r.count}
          </li>
        ))}
      </ul>
    </div>
  )
}
