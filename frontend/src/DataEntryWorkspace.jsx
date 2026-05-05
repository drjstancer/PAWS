import React, { useState } from 'react'

export default function DataEntryWorkspace() {
  const [saved, setSaved] = useState('')

  const fakeSave = label => {
    setSaved(`${label} saved in demo mode.`)
    setTimeout(() => setSaved(''), 3000)
  }

  return (
    <div className="data-entry-workspace">
      <section className="admin-hero">
        <div>
          <span>Operational Workspace</span>
          <h2>Data Entry & Student Updates</h2>
          <p>Model how staff enter students, notes, courses, compliance updates, and import batches.</p>
        </div>
        <button className="btn-like" onClick={() => fakeSave('Demo draft')}>Save Draft</button>
      </section>

      {saved && <div className="notice success">{saved}</div>}

      <section className="grid two">
        <FormCard title="Student Intake">
          <div className="form-grid two-col">
            <label>First Name<input placeholder="Aaliyah" /></label>
            <label>Last Name<input placeholder="Brooks" /></label>
            <label>MU ID<input placeholder="12345678" /></label>
            <label>MU Email<input placeholder="student@missouri.edu" /></label>
            <label>Program Track<select><option>JPAWS</option><option>PAWS Achiever</option><option>PAWS Pre-Admissions</option><option>PAWS Scholar</option><option>Legacy PAWS</option></select></label>
            <label>Classification<select><option>Freshman</option><option>Sophomore</option><option>Junior</option><option>Senior</option><option>M1</option><option>M2</option><option>M3</option><option>M4</option><option>Resident</option></select></label>
            <label>RUCA Code<select>{[1,2,3,4,5,6,7,8,9,10].map(n => <option key={n}>{n}</option>)}</select></label>
            <label>High Touch Mentor<input placeholder="Dr. J" /></label>
          </div>
          <button className="btn-like" onClick={() => fakeSave('Student intake record')}>Save Student</button>
        </FormCard>

        <FormCard title="Advising Note">
          <div className="form-grid two-col">
            <label>Student<select><option>Aaliyah Brooks</option><option>Marcus Carter</option><option>Elijah Reed</option></select></label>
            <label>Meeting Type<select><option>HTM</option><option>CASE</option><option>MedOpp</option><option>Well-Being</option><option>Other</option></select></label>
            <label>Meeting Date<input type="date" /></label>
            <label>Concern Level<select><option>Low</option><option>Moderate</option><option>High</option></select></label>
          </div>
          <label>Meeting Summary<textarea placeholder="Briefly summarize student progress, needs, and next steps." /></label>
          <div className="check-row">
            <label><input type="checkbox" /> Follow-up needed</label>
            <label><input type="checkbox" /> Restricted note</label>
          </div>
          <button className="btn-like" onClick={() => fakeSave('Advising note')}>Save Note</button>
        </FormCard>
      </section>

      <section className="grid two">
        <FormCard title="Course & GPA Entry">
          <div className="form-grid two-col">
            <label>Student<select><option>Aaliyah Brooks</option><option>Marcus Carter</option><option>Sofia Nguyen</option></select></label>
            <label>Academic Year<input placeholder="2025-2026" /></label>
            <label>Term<select><option>Fall</option><option>Spring</option><option>Summer</option></select></label>
            <label>Course Subject<input placeholder="BIO" /></label>
            <label>Course Number<input placeholder="1500" /></label>
            <label>Credit Hours<input type="number" placeholder="3" /></label>
            <label>Letter Grade<select><option>A</option><option>A-</option><option>B+</option><option>B</option><option>B-</option><option>C+</option><option>C</option></select></label>
            <label>Per-Credit Grade Value<input type="number" step="0.01" placeholder="4.00" /></label>
          </div>
          <div className="check-row">
            <label><input type="checkbox" /> Counts toward science/math GPA</label>
            <label><input type="checkbox" /> Repeat course</label>
          </div>
          <button className="btn-like" onClick={() => fakeSave('Course record')}>Save Course</button>
        </FormCard>

        <FormCard title="Compliance Status Update">
          <div className="form-grid two-col">
            <label>Student<select><option>Aaliyah Brooks</option><option>Marcus Carter</option><option>Elijah Reed</option></select></label>
            <label>Cycle<input value="2025-2026" readOnly /></label>
            <label>Requirement<select><option>Annual Contract</option><option>Retreat Attendance</option><option>HTM Meeting</option><option>MedPrep 1</option><option>MedPrep 2</option></select></label>
            <label>Status<select><option>Not Started</option><option>In Progress</option><option>Completed</option><option>Waived</option><option>Not Applicable</option></select></label>
            <label>Completion Date<input type="date" /></label>
          </div>
          <label>Notes<textarea placeholder="Required for waivers or unusual updates." /></label>
          <button className="btn-like" onClick={() => fakeSave('Compliance update')}>Update Requirement</button>
        </FormCard>
      </section>

      <section className="demo-card">
        <h2>Import Preview</h2>
        <p className="muted-copy">
          Bulk imports would validate MU ID, required fields, GPA ranges, RUCA codes, and cohort year before saving.
        </p>
        <div className="import-preview">
          <div><strong>Accepted</strong><span>42 records</span></div>
          <div><strong>Updated</strong><span>8 records</span></div>
          <div><strong>Rejected</strong><span>3 records</span></div>
          <button className="btn-like" onClick={() => fakeSave('Import preview')}>Run Demo Import Validation</button>
        </div>
      </section>
    </div>
  )
}

function FormCard({ title, children }) {
  return (
    <section className="demo-card data-form-card">
      <h2>{title}</h2>
      <div className="form-stack">{children}</div>
    </section>
  )
}