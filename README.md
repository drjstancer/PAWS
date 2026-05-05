# PAWS/JPAWS Data Management System

A full-stack starter application for the JPAWS/PAWS program data system.

This repo gives IT/programmers a buildable foundation for:

- student profile management
- compliance and requirement tracking
- shadowing workflow tracking
- advising/support note tracking
- academic indicators
- alumni/outcomes tracking
- CSV exports
- role-based access scaffolding
- campus SSO/reverse-proxy authentication handoff

## Current campus system references

The attached landing page samples suggest the existing environment uses a mixture of:

- a React single-page app deployed under `/PathwaysAndOutreachAdmin/`
- an ASP.NET/WebForms-style Medical Education Portal shell
- Bootstrap/DataTables/toastr/select2 frontend conventions
- server-managed session behavior with `KeepAlive` and `LogOut`
- a displayed authenticated user name from the portal shell
- an Applications navigation menu that links to Pathways And Outreach Admin

This system is designed so IT can place it behind the same campus authentication boundary or adapt it to the existing portal wrapper.

## Recommended deployment model

### Option A: Campus portal/reverse proxy auth
Use the existing Medical Education Portal or campus gateway to authenticate users. The backend reads identity from trusted headers set by the campus proxy.

Expected headers can be configured in `.env`:

```bash
SSO_HEADER_EMAIL=x-forwarded-user
SSO_HEADER_NAME=x-forwarded-name
SSO_HEADER_ID=x-forwarded-uid
```

### Option B: OIDC/SAML adapter
Replace the included `campusAuth` middleware with your institution's OIDC/SAML library while keeping the same user/role model.

## Repo structure

```text
apps/
  api/      Express + Prisma backend API
  web/      React/Vite frontend
prisma/
  schema.prisma
  seed.ts
docs/
  API_SPEC.md
  SECURITY_HANDOFF.md
```

## Quick start for local development

```bash
npm install
cp apps/api/.env.example apps/api/.env
docker compose up -d db
npm run db:push
npm run seed
npm run dev
```

Frontend: http://localhost:5173  
Backend: http://localhost:4000/api/health

## Dev login behavior

In development, the API accepts mock campus identity headers. Production should only accept these headers from a trusted reverse proxy or portal host.

Example:

```bash
x-forwarded-user: stancerj@missouri.edu
x-forwarded-name: Joel Stancer
x-forwarded-uid: stancerj
```

## Important warning

This repo does **not** complete the campus security integration by itself. IT must connect authentication/session management to campus SSO or the existing Medical Education Portal security boundary before production use.

The application is intentionally structured so campus IT can drop in their authentication middleware without rebuilding the data model.
