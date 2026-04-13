# Supervisor module setup

## What is included
- Supervisor login
- Research area / expertise selection
- Blind review dashboard with no student identity fields
- Express interest workflow
- UI polish for supervisor pages
- Unit test project for blind filtering logic

## Before running
Update the SQL Server instance in `appsettings.json` if your machine does not use `localhost\\SQLEXPRESS`.

## Commands to run locally
```bash
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

## Seeded demo credentials
- Supervisor: `supervisor@matchboard.local` / `Supervisor@123`
- Student 1: `student1@matchboard.local` / `Student@123`
- Student 2: `student2@matchboard.local` / `Student@123`
- Admin: `admin@matchboard.local` / `Admin@123`

## Test command
```bash
dotnet test MatchBoard.Tests/MatchBoard.Tests.csproj
```
