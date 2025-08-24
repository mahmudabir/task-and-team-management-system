# Task and Team Management System

A .NET 9 Web API for managing tasks and teams.

- Main project: `src/WebApi`
- Target framework: .NET 9.0
- Default URL: `http://localhost:5000`
- Database: SQL Server (LocalDB by default)

## Prerequisites

- .NET SDK 9.0+
- SQL Server LocalDB (Windows) or a SQL Server instance
- (Optional) Docker Desktop for containerized run
- (Optional) Seq for structured logs at `http://localhost:5341`

## First run: seed data

Before the very first run, enable seeding so default roles/users and demo data are created.

1) Open `src/WebApi/appsettings.json` and set:

```
"SeedDataOnStartup": true
```

2) After the app starts once and seeding completes, change it back to:

```
"SeedDataOnStartup": false
```

This prevents reseeding on every start.

## Local development (CLI)

1Run the Web API

```
dotnet run --project src/WebApi
```

- Swagger UI: `http://localhost:5000/swagger`
- Health/diagnostics may be available depending on environment configuration.

Default accounts (from appsettings):
- Admin: `admin` / `Admin123!`
- Manager: `manager` / `Manager123!`
- Employee: `employee` / `Employee123!`

## Configuration

Key settings in `src/WebApi/appsettings.json`:
- ConnectionStrings.LiveDatabase – default local connection (LocalDB)
- JwtSettings – issuer/audience/keys for auth
- Serilog – console/file/Seq sinks
- SeedDataOnStartup – set to `true` only on first run to seed initial data

If you don’t use Seq, either run a Seq instance at `http://localhost:5341` or remove/disable the Seq sink in Serilog configuration.

## Troubleshooting

- LocalDB not found: install SQL Server Express LocalDB or update `ConnectionStrings:LiveDatabase` to a reachable SQL Server.
- Migrations fail: ensure you ran `dotnet restore` and invoked `dotnet ef database update` with `--project src/Infrastructure --startup-project src/WebApi`.

