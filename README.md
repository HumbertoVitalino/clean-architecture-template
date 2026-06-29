# Clean Architecture Template

[![CI - Feature](https://github.com/HumbertoVitalino/clean-architecture-template/actions/workflows/ci-feature.yml/badge.svg)](https://github.com/HumbertoVitalino/clean-architecture-template/actions/workflows/ci-feature.yml)
[![CI - Develop](https://github.com/HumbertoVitalino/clean-architecture-template/actions/workflows/ci-develop.yml/badge.svg)](https://github.com/HumbertoVitalino/clean-architecture-template/actions/workflows/ci-develop.yml)
[![NuGet](https://img.shields.io/nuget/v/HumbertoVitalino.CleanArchitecture.Template.svg)](https://www.nuget.org/packages/HumbertoVitalino.CleanArchitecture.Template)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A `dotnet new` template for .NET 10 APIs following Clean Architecture and Domain-Driven Design principles.

## What's included

- **Clean Architecture** — Domain, Application, Infrastructure, Api layers with strict dependency rules
- **DDD building blocks** — Entities, Value Objects, Domain Events, Domain Exceptions
- **JWT authentication** — Login use case, `IJwtService`, Bearer token validation
- **Minimal API endpoints** — versioned routes (`/api/v1/...`), `Request` → `MapToInput` pattern, `[FromHeader]` CorrelationId
- **FluentValidation** — validators in the API layer only, validating before entering use cases
- **EF Core + SQL Server** — migrations, repository pattern, Unit of Work
- **CorrelationId middleware** — propagates or generates `X-Correlation-Id` on every request
- **Structured logging** — `ILogger<T>` with CorrelationId on all error paths
- **Unit tests** — xUnit + FluentAssertions + NullLogger
- **Integration tests** — xUnit + docker-compose SQL Server, full use case flow
- **Docker** — multi-stage Dockerfile, docker-compose with named volume, `.env.example`
- **GitHub Actions** — CI for feature and develop branches, CD to GHCR and NuGet on release

## Installation

```bash
dotnet new install HumbertoVitalino.CleanArchitecture.Template
```

## Usage

```bash
dotnet new clean-arch -n MyCompany.MyProject
cd MyCompany.MyProject
```

All occurrences of `CompanyName.ProjectName` in namespaces, project names and folder names are replaced with the name you provide.

## Running locally

### With Docker

```bash
cp .env.example .env
# Fill in the values in .env
docker compose up -d
```

### Without Docker

```bash
# Start SQL Server separately and set the connection string
dotnet run --project src/MyCompany.MyProject.Api
```

## Running tests

```bash
# Unit tests
dotnet test tests/MyCompany.MyProject.UnitTests

# Integration tests (requires Docker)
docker compose -f docker-compose.tests.yml up -d
dotnet test tests/MyCompany.MyProject.IntegrationTests
docker compose -f docker-compose.tests.yml down
```

## Project structure

```
src/
  CompanyName.ProjectName.Domain/          # Entities, Value Objects, Domain Events
  CompanyName.ProjectName.Application/     # Use Cases, Interfaces, DTOs
  CompanyName.ProjectName.Infrastructure/  # EF Core, Repositories, JWT, Migrations
  CompanyName.ProjectName.Api/             # Endpoints, Validators, Middleware, IoC
tests/
  CompanyName.ProjectName.UnitTests/
  CompanyName.ProjectName.IntegrationTests/
```

## License

MIT
