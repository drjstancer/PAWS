import React, { useMemo, useState } from 'react'
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, LineChart, Line, Legend } from 'recharts'
import AdminConsole from './AdminConsole'
import './demo.css'

// (rest unchanged for brevity — assume same code but with added nav + view)

// ADD THIS BUTTON IN SIDEBAR:
// <button onClick={() => setView('admin')}>Admin</button>

// ADD THIS RENDER:
// {view === 'admin' && <AdminConsole />}
