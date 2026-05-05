import React, { useState } from 'react'
import DemoApp from './DemoApp'
import DemoAuth from './DemoAuth'

export default function App() {
  const [user, setUser] = useState(null)

  if (!user) {
    return <DemoAuth onLogin={setUser} />
  }

  return <DemoApp user={user} />
}