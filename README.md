# UcarMobileApi Solution

## Description
This is a mobile API solution for Ucar developed with .NET 9.0, following a Clean Architecture layered approach.

## Project Structure

```
UcarMobileApi/
├── src/                           # Source code
│   ├── UcarMobileApi/             # Main Web API project
│   ├── UcarMobileApi.Core/        # Domain layer (entities, interfaces)
│   ├── UcarMobileApi.Application/ # Application layer (services, DTOs)
│   └── UcarMobileApi.Infrastructure/ # Infrastructure layer (DbContext, configurations)
├── tests/                         # Test projects
│   └── UcarMobileApi.Tests/       # Unit, Integration, and Functional tests
│       ├─ Unit/                   # Unit tests
│       │   ├─ Services/           # Service tests
│       │   └─ Validators/         # Validator tests
│       ├─ Integration/            # Integration tests (e.g., EF Core in-memory)
│       │   └─ Services/
│       ├─ Functional/             # Functional / endpoint tests
│       │   └─ Endpoints/
│       └─ TestHelpers/            # Helper factories (DbContext, Mapper, Validators)
├── docs/                           # Additional documentation
└── UcarMobileApiSolution.sln      # Solution file
```

---

## Projects

### 1. UcarMobileApi (Web API)
- **Type**: ASP.NET Core Web API
- **Framework**: .NET 9.0
- **Description**: Main project exposing the REST API
- **Dependencies**:
  - UcarMobileApi.Core
  - UcarMobileApi.Application
  - UcarMobileApi.Infrastructure

### 2. UcarMobileApi.Core (Domain)
- **Type**: Class Library
- **Framework**: .NET 9.0
- **Description**: Contains domain entities and interfaces
- **Dependencies**: None (framework-independent layer)

### 3. UcarMobileApi.Application (Application)
- **Type**: Class Library
- **Framework**: .NET 9.0
- **Description**: Contains business logic and services
- **Dependencies**:
  - UcarMobileApi.Core
  - UcarMobileApi.Infrastructure

### 4. UcarMobileApi.Infrastructure (Infrastructure)
- **Type**: Class Library
- **Framework**: .NET 9.0
- **Description**: Implements data access and external services
- **Dependencies**:
  - UcarMobileApi.Core

### 5. UcarMobileApi.Tests (Tests)
- **Type**: Test Project
- **Framework**: .NET 9.0
- **Description**: Contains all project tests
- **Dependencies**: All `src/` projects

---

## Technologies Used

- **.NET 9.0**: Main framework
- **Entity Framework Core 9.0**: ORM for data access
- **PostgreSQL**: Database (with NetTopologySuite support)
- **AutoMapper**: Object mapping
- **FluentValidation**: Model validation
- **Serilog**: Logging
- **Sentry**: Error monitoring
- **Swagger/OpenAPI**: API documentation
- **xUnit**: Test framework
- **Moq**: Mocking framework

---

## Useful Commands

### Build the solution
```bash
dotnet build
```

### Run tests
```bash
dotnet test
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=Functional"
```

### Run the application
```bash
cd src/UcarMobileApi
dotnet run
```

### Restore NuGet packages
```bash
dotnet restore
```

---

## Configuration

The application is configured to use:
- PostgreSQL as the main database
- Serilog for console and file logging
- Sentry for production error monitoring
- Swagger UI for interactive API documentation

---

## Architecture

The project follows Clean Architecture principles:

1. **Core (Domain)**: Business entities and interfaces, framework-independent
2. **Application**: Use cases and business logic
3. **Infrastructure**: Concrete implementations for data access
4. **Web API**: Controllers and web application configuration

This structure allows:
- Clear separation of responsibilities
- Easy testing
- Framework independence
- Maintainability and scalability

---

## Tests Structure

The `UcarMobileApi.Tests` project is organized by test type:

```
UcarMobileApi.Tests/
├─ Unit/                     # Unit tests (services, validators, small components)
│   ├─ Services/
│   └─ Validators/
├─ Integration/              # Integration tests (database, EF Core in-memory, service integration)
│   └─ Services/
├─ Functional/               # Functional / End-to-end tests (API endpoints)
│   └─ Endpoints/
└─ TestHelpers/              # Helper factories for DbContext, Mapper, Validators
```

This organization allows running tests selectively by category:

```bash
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=Functional"
```

---

## Current Status

The solution is properly organized and structured. All projects are linked correctly, and dependencies have been updated for .NET 9.0 compatibility.

**Note**: Some files may require minor adjustments in references and `using` statements to compile completely, but the base structure is correctly established.

