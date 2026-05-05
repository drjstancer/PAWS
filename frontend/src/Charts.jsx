import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, LineChart, Line, Legend } from 'recharts'

export function ProgramChart({ data }) {
  return (
    <ResponsiveContainer width="100%" height={250}>
      <BarChart data={data}>
        <XAxis dataKey="programTrack" />
        <YAxis />
        <Tooltip />
        <Bar dataKey="count" fill="#f1b82d" />
      </BarChart>
    </ResponsiveContainer>
  )
}

export function ClassificationChart({ data }) {
  return (
    <ResponsiveContainer width="100%" height={250}>
      <BarChart data={data}>
        <XAxis dataKey="classification" />
        <YAxis />
        <Tooltip />
        <Bar dataKey="count" fill="#1f2a44" />
      </BarChart>
    </ResponsiveContainer>
  )
}

export function RucaChart({ data }) {
  const COLORS = ['#f1b82d', '#1f2a44', '#6b7280']
  return (
    <ResponsiveContainer width="100%" height={250}>
      <PieChart>
        <Pie data={data} dataKey="count" nameKey="rucaCategory" outerRadius={80}>
          {data.map((entry, index) => (
            <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
          ))}
        </Pie>
        <Tooltip />
      </PieChart>
    </ResponsiveContainer>
  )
}

export function ComplianceTrendChart({ data }) {
  return (
    <ResponsiveContainer width="100%" height={250}>
      <LineChart data={data}>
        <XAxis dataKey="period" />
        <YAxis />
        <Tooltip />
        <Legend />
        <Line type="monotone" dataKey="complianceRate" stroke="#f1b82d" />
      </LineChart>
    </ResponsiveContainer>
  )
}

export function GpaTrendChart({ data }) {
  return (
    <ResponsiveContainer width="100%" height={250}>
      <LineChart data={data}>
        <XAxis dataKey="period" />
        <YAxis />
        <Tooltip />
        <Legend />
        <Line type="monotone" dataKey="averageCumulativeGpa" stroke="#1f2a44" />
        <Line type="monotone" dataKey="averageScienceMathGpa" stroke="#f1b82d" />
      </LineChart>
    </ResponsiveContainer>
  )
}

export function RiskDistributionChart({ data }) {
  return (
    <ResponsiveContainer width="100%" height={250}>
      <BarChart data={data}>
        <XAxis dataKey="riskLevel" />
        <YAxis />
        <Tooltip />
        <Bar dataKey="count" fill="#6b7280" />
      </BarChart>
    </ResponsiveContainer>
  )
}
