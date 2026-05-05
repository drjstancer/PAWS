# PAWS Staging Environment Setup

## Purpose
This environment provides a full demo system with:
- Fake student data
- Role-based users
- Full workflow (intake → advising → GPA → compliance → analytics → export)

---

## Quick Start

```bash
docker-compose -f docker-compose.staging.yml up --build
```

---

## Access Points

Frontend:
http://localhost:5174

API:
http://localhost:5080

SQL Server:
localhost,14333

---

## Demo Accounts

| Role | Email | Password |
|------|------|---------|
| System Admin | admin@paws.demo | Password123! |
| Program Admin | program@paws.demo | Password123! |
| Advisor | advisor@paws.demo | Password123! |
| Leadership | leadership@paws.demo | Password123! |
| Restricted Viewer | restricted@paws.demo | Password123! |

---

## Demo Flow

1. Login as Program Admin
2. Create student
3. Add course
4. Run GPA calculation
5. Add advising note
6. Add restricted note
7. Switch roles
8. Show permission enforcement
9. Export report
10. Show audit log

---

## Notes
- All data is fictional
- No production systems connected
- Designed for demos, grants, and presentations

---

## Next Step (IT)
Deploy to:
- Azure App Service OR
- Internal MU staging environment

