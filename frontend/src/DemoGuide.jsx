import React from 'react'

export default function DemoGuide({ user }) {
  return (
    <section className="demo-guide">
      <div className="demo-guide-main">
        <span className="eyebrow">Guided Demo Mode</span>
        <h2>PAWS Program Management System</h2>
        <p>
          This demonstration uses fictional student records to show how PAWS can support student tracking,
          advising, compliance, shadowing, analytics, exports, and administrative governance.
        </p>
      </div>
      <div className="demo-guide-steps">
        <div><strong>1</strong><span>Start with Dashboard</span></div>
        <div><strong>2</strong><span>Open Workspace</span></div>
        <div><strong>3</strong><span>Check Advising permissions</span></div>
        <div><strong>4</strong><span>Finish in Admin</span></div>
      </div>
      <div className="demo-guide-role">
        <small>Current demo role</small>
        <strong>{user?.role || 'Demo User'}</strong>
      </div>
    </section>
  )
}
