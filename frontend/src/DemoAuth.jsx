import React, { useState } from 'react'
import { DEMO_USERS } from './demoData'

export default function DemoAuth({ onLogin }) {
  const [email, setEmail] = useState('admin@paws.demo')

  const handleLogin = () => {
    const user = DEMO_USERS.find(u => u.email === email)
    if (user) {
      onLogin(user)
    }
  }

  return (
    <div style={{display:'flex',justifyContent:'center',alignItems:'center',height:'100vh',background:'#111'}}>
      <div style={{background:'#1c1c1c',padding:'30px',borderRadius:'12px',color:'white',width:'320px'}}>
        <h2>PAWS Demo Login</h2>
        <p>Select a role</p>

        <select
          value={email}
          onChange={e => setEmail(e.target.value)}
          style={{width:'100%',marginBottom:'15px'}}
        >
          {DEMO_USERS.map(u => (
            <option key={u.id} value={u.email}>
              {u.role}
            </option>
          ))}
        </select>

        <button onClick={handleLogin} style={{width:'100%'}}>
          Enter System
        </button>
      </div>
    </div>
  )
}