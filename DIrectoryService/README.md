# Directory Service

Enterprise-grade directory management system built with .NET 10 and PostgreSQL, demonstrating Clean Architecture and CQRS patterns.

## Architecture

The solution implements **Clean Architecture** with strict dependency rules:

```
DirectoryService.Domain                    # Entities & business rules (no dependencies)
DirectoryService.Application               # Use cases & CQRS handlers
DirectoryService.Contracts                 # DTOs & API contracts
DirectoryService.Infrastructure.Postgres   # EF Core, Dapper, transaction management
DirectoryService.Presenters                # Web API & endpoints
DirectoryService.IntegrationTests          # End-to-end tests
```

### Key Architectural Decisions

**CQRS Pattern**: Commands and queries separated for scalability and optimization flexibility

**Optimistic Concurrency**: Version-based conflict detection for concurrent updates

**Feature-Oriented Organization**: Code organized by business features (Departments, Positions, Locations) rather than technical layers

**Functional Error Handling**: Railway-oriented programming with `CSharpFunctionalExtensions` for Result types

**Hybrid ORM Approach**:
- EF Core for writes and complex operations
- Dapper for high-performance read queries

**Transaction Management**: Custom `ITransactionManager` abstraction with Unit of Work pattern

## Technology Stack

**Core**
- .NET 10 with C# 13
- PostgreSQL database
- Entity Framework Core 10
- Dapper (micro-ORM)

**Infrastructure**
- Serilog (structured logging)
- Seq (log aggregation)
- Docker Compose (local dev environment)

**API**
- ASP.NET Core Web API
- Swashbuckle (OpenAPI/Swagger)
- Minimal API style endpoints

**Quality**
- xUnit (integration testing)
- .NET Analyzers + StyleCop
- Nullable reference types enforced

## Getting Started

**Prerequisites**: .NET 10 SDK, Docker

```bash
# Start infrastructure
docker-compose up -d

# Apply migrations
cd src/DirectoryService.Presenters
dotnet ef database update --project ../DirectoryService.Infrastructure.Postgres

# Run application
dotnet run
```

**Endpoints**:
- API: `https://localhost:<port>/swagger`
- Seq: `http://localhost:8081`

## Domain Model

Three core entities with optimistic concurrency control:
- **Departments**: Organizational units
- **Positions**: Job positions with department associations
- **Locations**: Physical/logical locations

All entities include version tracking and indexed queries for performance.

## Development Practices

**Design Patterns**: Repository, Unit of Work, CQRS, Result Pattern, Dependency Injection

**Code Quality**: Enabled .NET analyzers (all modes), StyleCop enforcement, EditorConfig standards

**Performance**: Strategic database indexes, async/await throughout, connection pooling, Dapper for reads

**Testing**: Integration tests with `WebApplicationFactory`, isolated test database per test

## Configuration

PostgreSQL runs on port 5434, credentials in `docker-compose.yml`. Connection string in `appsettings.json`.

## Contact

https://www.linkedin.com/in/vladimir-shevelevich-2a0a93ba/

---

Portfolio project demonstrating modern .NET architecture and enterprise development practices.
