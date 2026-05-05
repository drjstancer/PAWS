import { useEffect, useState } from 'react'
import { ProgramChart, ClassificationChart, RucaChart } from './Charts'

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

      <h3>Program Distribution</h3>
      <ProgramChart data={data.byProgram} />

      <h3>Classification Distribution</h3>
      <ClassificationChart data={data.byClassification} />

      <h3>Rural vs Urban</h3>
      <RucaChart data={data.byRuca} />
    </div>
  )
}
