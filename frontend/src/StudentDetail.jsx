import { useEffect, useState } from 'react'

export default function StudentDetail({ id }) {
  const [data, setData] = useState(null)

  useEffect(() => {
    fetch(`/api/student-detail/${id}`)
      .then(r => r.json())
      .then(setData)
  }, [id])

  if (!data) return <div>Loading...</div>

  const { student, requirementSummary } = data

  return (
    <div style={{ padding: 20 }}>
      <h2>{student.firstName} {student.lastName}</h2>
      <p><strong>MU ID:</strong> {student.muId}</p>
      <p><strong>Program:</strong> {student.programTrack}</p>
      <p><strong>Classification:</strong> {student.classification}</p>
      <p><strong>GPA:</strong> {student.cumulativeGpa}</p>

      <h3>Compliance</h3>
      <p>Total: {requirementSummary.total}</p>
      <p>Completed: {requirementSummary.completed}</p>
      <p>Rate: {requirementSummary.complianceRate}%</p>
    </div>
  )
}
