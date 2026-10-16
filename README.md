# AcxiomCRM

ASP.NET Core MVC CRM using ASP.NET Core Identity, EF Core, SQLite, Bootstrap, Chart.js, role-based authorization, audit logging, and secured REST APIs.

The local SQLite database is intentionally excluded from Git. It is created and migrated automatically from the committed EF Core migrations.

## Run

From `AcxiomCRM`:

```powershell
$env:PATH="$env:USERPROFILE\.dotnet;$env:PATH"
dotnet restore
dotnet ef database update
$env:SeedAdmin__Email="admin@example.com"
$env:SeedAdmin__Password="ChangeMe!123"
dotnet run
```

The application creates the SQLite database (`acxiomcrm.db`) and the `Admin`, `Manager`, and `Sales Executive` roles on startup. Only use `SeedAdmin` during development; do not put production credentials in source control.

Protected MVC pages require login. REST endpoints are available under `/api/customers`, `/api/leads`, and `/api/opportunities` and require an authenticated Identity cookie.

The application enforces the exact assignment validation messages for invalid email, phone, opportunity amount/probability/close date, and includes client-side unobtrusive validation plus server-side validation.
