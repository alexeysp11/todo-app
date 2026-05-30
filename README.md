# Task Management System (WinForms / .NET Framework 4.8)

[English](README.md) | [Русский](README.ru.md)

A classic To-Do application built with WinForms and PostgreSQL using pure ADO.NET.

## Technical Stack & Architecture

- **UI Layer**: WinForms (.NET Framework 4.8), Autofac for Form resolution.
- **BLL Layer**: Service pattern with strict domain validation rules.
- **DAL Layer**: Raw SQL queries via `Npgsql` (v4.1.14) with async/await and `ConfigureAwait(false)`.
- **DI Container**: Autofac with explicit Lifetime Scope management for child windows.
- **Logging**: Serilog with file sink and daily rolling interval.
- **Testing**: xUnit, Moq, AutoFixture, and FluentAssertions.

---

## Infrastructure Setup

The local environment is orchestrated via Docker Compose. Run the following command in the root directory:

```bash
docker-compose up -d
```

This starts a PostgreSQL instance on port **54321** and runs an initialization script that configures two independent databases:
1. `todo_management_db` — Runtime database for the WinForms application.
2. `todo_tests_db` — Isolated database for integration tests.

---

## Configuration

### Main Application (`TodoApp.WinForms/App.config`)
```xml
<connectionStrings>
  <add name="TodoDbConnection" 
       connectionString="Server=localhost;Port=54321;Database=todo_management_db;User Id=postgres;Password=postgres;" 
       providerName="Npgsql" />
</connectionStrings>
```

### Integration Tests (`TodoApp.Tests.Integration/App.config`)
```xml
<connectionStrings>
  <add name="TodoDbConnection" 
       connectionString="Server=localhost;Port=54321;Database=todo_tests_db;User Id=postgres;Password=postgres;" 
       providerName="Npgsql" />
</connectionStrings>
```

---

## Test Execution

1. Build the solution (`Ctrl + Shift + B`).
2. Open **Test Explorer** in Visual Studio.
3. Run all tests. 

*Note: Integration tests clear tables via `TRUNCATE ... RESTART IDENTITY CASCADE` before each test run using xUnit `IAsyncLifetime` fixtures.*
