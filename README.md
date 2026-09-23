# SPL Project

A full-stack web application for managing acoustic measurement projects, built with **ASP.NET Core**, **Vue.js**, **Entity Framework Core**, and **PostgreSQL**.

The application provides user authentication, JWT-based authorization, project management, dashboard data, and browser-based microphone measurements.

## Features

- User registration and login
- JWT access-token authentication
- Refresh-token authentication flow
- Role-based authorization
- Dashboard with user and project information
- Create and manage projects
- Project-to-measurement domain model
- PostgreSQL persistence through Entity Framework Core
- Repository and service abstractions
- Browser microphone access through the Web Audio API
- Five-second sound-level measurement
- One measurement record per second
- Vue.js reactive UI
- ASP.NET Core cancellation-token support for asynchronous operations

## Architecture

The project follows a layered architecture separating the API, application logic, domain entities, and infrastructure concerns.

```text
┌──────────────────────────┐
│      Vue.js Client       │
│  Vue Router + Axios      │
└────────────┬─────────────┘
             │ HTTP / JSON
             ▼
┌──────────────────────────┐
│   ASP.NET Core API       │
│       Controllers       │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│       Application        │
│ Services / Interfaces   │
│          DTOs            │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│         Domain           │
│ ApplicationUser          │
│ RefreshToken             │
│ DashboardOverview        │
│ Project                  │
│ Measurement              │
└────────────▲─────────────┘
             │
             │
┌────────────┴─────────────┐
│      Infrastructure      │
│ Repositories / EF Core   │
│ Identity / JWT / DB      │
└────────────┬─────────────┘
             │
             ▼
      ┌──────────────┐
      │ PostgreSQL   │
      └──────────────┘
```

### Backend layers

#### API

Contains the ASP.NET Core controllers responsible for handling HTTP requests.

Main controllers:

- `AuthController`
- `DashboardController`
- `ProjectController`

#### Application

Contains application contracts, services, DTOs, and repository interfaces.

Examples:

- `IAuthService`
- `ITokenService`
- `IDashboardService`
- `IProjectService`
- `IDashboardRepository`
- `IProjectRepository`

#### Domain

Contains the core business entities:

- `ApplicationUser`
- `RefreshToken`
- `DashboardOverview`
- `Project`
- `Measurement`

The `Project` entity has a one-to-many relationship with `Measurement`.

#### Infrastructure

Contains implementations for persistence and authentication:

- Entity Framework Core DbContexts
- Repository implementations
- ASP.NET Core Identity
- JWT token generation
- Refresh-token handling
- PostgreSQL integration

## Technology Stack

### Backend

- C#
- ASP.NET Core
- Entity Framework Core
- ASP.NET Core Identity
- JWT Bearer Authentication
- PostgreSQL
- Dependency Injection
- REST API

### Frontend

- Vue.js 3
- JavaScript
- Vite
- Vue Router
- Axios
- Web Audio API

### Development Tools

- Visual Studio
- Node.js / npm
- .NET SDK
- PostgreSQL
- Git / GitHub
- PlantUML for architecture documentation

## Project Structure

The repository is organized around the client and backend applications.

A simplified structure is:

```text
SplProject/
│
├── Src/
│   ├── Client/
│   │   ├── src/
│   │   │   ├── views/
│   │   │   ├── router/
│   │   │   └── ...
│   │   └── package.json
│   │
│   └── API/
│       ├── Controllers/
│       ├── Application/
│       ├── Domain/
│       ├── Infrastructure/
│       └── Program.cs
│
├── UML-DOCUMENTATION*.puml
├── .gitignore
└── ...
```

The exact directory names may vary slightly depending on the current branch/version.

## Authentication

The application uses JWT-based authentication.

The general authentication flow is:

```text
Client
   │
   │ Login credentials
   ▼
AuthController
   │
   ▼
AuthService
   │
   ├── ASP.NET Core Identity
   │
   ├── Create JWT access token
   │
   └── Create refresh token
   │
   ▼
Client
```

Refresh tokens are stored in the database and can be revoked.

Protected API endpoints require a valid JWT:

```http
Authorization: Bearer <access-token>
```

## API Endpoints

### Authentication

| Method | Endpoint | Authentication |
|---|---|---|
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| POST | `/api/auth/refresh` | Public |
| POST | `/api/auth/revoke` | Required |

### Dashboard

| Method | Endpoint | Authentication |
|---|---|---|
| GET | `/api/dashboard` | Required |

### Projects

| Method | Endpoint | Authentication |
|---|---|---|
| POST | `/api/project` | Required |

## Database

PostgreSQL is used as the primary relational database.

Entity Framework Core is responsible for:

- Database access
- Entity mapping
- Relationships
- Migrations
- Persistence

The main domain relationships include:

```text
ApplicationUser
      │
      ├─────────── * RefreshToken
      │
      └─────────── * Project
                         │
                         └─────────── * Measurement
```

The current project configuration also uses an EF Core shadow foreign key for the `Project` → `Measurement` relationship.

## Microphone Measurement

The frontend uses the browser's native Web Audio API rather than an external audio-processing package.

The measurement process is approximately:

```text
getUserMedia()
      │
      ▼
MediaStream
      │
      ▼
AudioContext
      │
      ▼
AnalyserNode
      │
      ▼
Audio samples
      │
      ▼
RMS calculation
      │
      ▼
dBFS calculation
      │
      ▼
5 measurements
```

A five-second measurement produces five records:

```javascript
[
  { second: 1, level: -32.45 },
  { second: 2, level: -31.82 },
  { second: 3, level: -30.91 },
  { second: 4, level: -33.10 },
  { second: 5, level: -32.04 }
]
```

### Important audio note

The current implementation calculates **dBFS (decibels relative to full scale)** from the microphone's digital signal.

It should not be interpreted as calibrated **dB SPL**.

For professional acoustic measurements, microphone calibration and an appropriate weighting/filtering process such as A-weighting would be required.

## Prerequisites

Before running the project, install:

- .NET SDK compatible with the solution's target framework
- Node.js and npm
- PostgreSQL
- Visual Studio or another .NET-compatible IDE

Verify the installations:

```bash
dotnet --version
node --version
npm --version
```

## Configuration

The API requires database and authentication configuration.

Create or configure the appropriate application settings for the environment.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SplProject;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Issuer": "SplProject",
    "Audience": "SplProjectClient",
    "SecretKey": "YOUR_SECRET_KEY"
  }
}
```

**Do not commit real passwords, JWT signing keys, or other secrets to Git.**

For local development, use user secrets or environment-specific configuration where appropriate.

## Database Setup

After configuring PostgreSQL, apply the Entity Framework Core migrations.

From the appropriate API project directory:

```bash
dotnet ef database update
```

If the project requires the context to be specified explicitly:

```bash
dotnet ef database update --context ApplicationDbContext
```

Other contexts may be available depending on the current solution configuration.

## Running the Backend

Navigate to the ASP.NET Core API project and run:

```bash
dotnet restore
dotnet build
dotnet run
```

The API URL will be displayed by ASP.NET Core when the application starts.

## Running the Frontend

Navigate to the Vue client:

```bash
npm install
npm run dev
```

Vite will display the local development URL, normally similar to:

```text
http://localhost:5173
```

## Microphone Permissions

The browser must have permission to access the microphone.

When prompted, select **Allow**.

If microphone access fails, check:

1. Browser site permissions.
2. Windows microphone privacy settings.
3. That a microphone is connected and available.
4. That another application is not exclusively using the microphone.

`navigator.mediaDevices.getUserMedia()` requires a secure context. `localhost` is normally treated as a secure development context by modern browsers.

## Development Workflow

A typical development workflow is:

```bash
git pull
dotnet restore
dotnet build

cd <client-directory>
npm install
npm run dev
```

For backend changes:

```bash
dotnet build
dotnet test
```

For Entity Framework changes:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## UML Documentation

The repository contains PlantUML documentation describing the architecture and runtime behavior.

The documentation includes diagrams for:

1. System architecture
2. Backend class diagram
3. Domain model
4. Persistence / EF Core architecture
5. Authentication sequence
6. Refresh-token sequence
7. Dashboard retrieval sequence
8. Create-project sequence
9. Browser microphone measurement
10. Dependency injection
11. API endpoint map
12. Layer/package dependencies

The PlantUML files can be opened with a PlantUML-compatible extension in Visual Studio Code or rendered using PlantUML.

## Design Principles

The backend is structured around several common software-engineering principles:

- Separation of concerns
- Dependency injection
- Interface-based abstractions
- Repository pattern
- Layered architecture
- DTO-based API contracts
- Asynchronous programming
- Cancellation-token support
- JWT authentication
- Refresh-token rotation/revocation
- Entity Framework Core persistence

## Future Improvements

Potential improvements include:

- Persisting microphone measurements through a dedicated API endpoint
- Adding explicit `ProjectId` and `Project` navigation properties to `Measurement`
- Implementing calibrated dB SPL measurements
- Adding A-weighted measurements such as LAeq
- Adding automated integration tests
- Adding frontend unit/component tests
- Adding API documentation with Swagger/OpenAPI
- Adding Docker support for the API and PostgreSQL
- Adding CI/CD through GitHub Actions
- Adding structured application logging
- Adding measurement visualization and historical charts

## Author

**Juan Camilo Avila**

GitHub: https://github.com/Juan-Avila92

## Repository

[GitHub Repository](https://github.com/Juan-Avila92/SplProject/tree/master)

## License

Add the project's license information here if/when a license is selected.
