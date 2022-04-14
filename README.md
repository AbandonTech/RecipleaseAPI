# Reciplease API

## Setup
### Environment
Some environment variables must be added to run this project. These can be added to `launchSettings.json`
```
// JWT configuration
"ValidAudience": The valid audience for the JWT, should be set to the url of this server i.e "http://localhost:8000"
"ValidIssuer": Valid issuer for JWT, should be the same var as the above
"Secret": JWT secret, can be literally anything, used to generate the JWT signature

// Google OAuth configuration
"GOOGLE_OAUTH_CLIENT_ID": This is the google cloud platform client ID for the application
"GOOGLE_OAUTH_CLIENT_SECRET": This is the google cloud platform client secret for the application
```

## Database Migrations
The `TempData/reciplease.db` directory and file will be created on database update.

Install the dotnet ef tool in order to generate and apply database migrations.
```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add <migrationName>
dotnet ef database update
```