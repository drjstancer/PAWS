import React, { useState } from 'react'
import { DEMO_USERS } from './demoData'

export default function DemoAuth({ onLogin }) {
  const [email, setEmail] = useState('admin@paws.demo')
  const [error, setError] = useState('')

  const handleLogin = () => {
    const user = DEMO_USERS.find(u => u.email === email)
    if (!user) {
      setError('Invalid demo user')
      return
    }
    onLogin(user)
  }

  return (
    <div className="auth-shell">
      <div className="auth-card">
        <h2>PAWS Demo Login</h2>
        <p>Select a role to explore the system.</p>
        <select value={email} onChange={e => setEmail(e.target.value)}>
          {DEMO_USERS.map(u => (
            <option key={u.id} value={u.email}>{u.role}</option>
          ))}
        </select>
        <button onClick={handleLogin}>Enter System</button>
        {error && <span className="error">{error}</span>}
      </div>
    </div>
  )
}
