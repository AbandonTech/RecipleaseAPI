# Reciplease API

### Database Migrations
The `TempData/reciplease.db` directory and file will be created on database update.

Install the dotnet ef tool in order to generate and apply database migrations.
```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add <migrationName>
dotnet ef database update
```