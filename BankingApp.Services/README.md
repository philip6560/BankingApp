# Banking App Backend Service

## Requirements

- [.NET v8.0.x SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- PostgresSql 15 or later
- [PgAdmin](https://www.pgadmin.org/download/) or any 3rd party PostgresSql client
- VS Code/Visual Studio 2022/Rider

## Setting up the backend server

1. Ensure .NET sdk and PostgresSql have been properly setup on your machine
2. Create a local database for the project using pgAdmin or psql: `CREATE DATABASE banking-app`
3. Update `BankingApp/BankingApp/appsettings.json` with your database connection string, eg: `"Default": "User ID=xxxx;Password=xxxx;Host=localhost;Port=5432;Database=banking-app;Include Error Detail=true;"`
4. Update database schema with EF migration from the Package Manager Console: `UPDATE-DATABASE` Note: To select `BankingApp\BankingApp.Data` project directory to run this command successfully.
