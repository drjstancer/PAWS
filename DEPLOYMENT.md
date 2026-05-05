# PAWS Deployment Guide

## Backend (.NET)
1. Install .NET 8 SDK
2. Configure connection string in appsettings.json
3. Run migrations (if added):
   dotnet ef database update
4. Run API:
   dotnet run

## Frontend
1. cd frontend
2. npm install
3. npm run build

## Hosting
- Host API on IIS or Kestrel behind reverse proxy
- Serve frontend as static files OR integrate into existing portal

## SSO Integration
- Ensure headers are passed from portal:
  x-forwarded-user
  x-forwarded-name

## Production Notes
- Use HTTPS only
- Restrict API behind campus network or gateway
- Add logging and monitoring (App Insights or equivalent)
- Enable database backups

## Final Step for IT
"Connect this app behind the Medical Education Portal authentication and deploy to internal server"
