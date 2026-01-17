# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Megabin is a waste management system for bin delivery and scheduled rubbish collection with intelligent route optimization. The backend is built with ASP.NET Core Web API (.NET 10.0) using PostgreSQL and Entity Framework Core, with self-hosted OpenRouteService for geocoding and route optimization.

## Essential Commands

### Development
```bash
# Run the application
dotnet run --project "Megabin Web/Megabin Web.csproj"

# Build
dotnet build "Megabin Web/Megabin Web.csproj" -c Release

# Restore dependencies
dotnet restore "Megabin Web/Megabin Web.csproj"
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName --project "Megabin Web"

# Apply migrations
dotnet ef database update --project "Megabin Web"

# Remove last migration (if not applied)
dotnet ef migrations remove --project "Megabin Web"
```

### Docker
```bash
# Build image (from repository root)
docker build -t megabin-web -f "Megabin Web/Dockerfile" .

# Run container
docker run -p 8080:8080 -p 8081:8081 megabin-web
```

## Architecture

### Technology Stack
- **Framework**: ASP.NET Core Web API (.NET 10.0)
- **Database**: PostgreSQL with Entity Framework Core
- **External Service**: Self-hosted OpenRouteService (ORS) at http://localhost:8082
- **Patterns**: N-tier architecture with dependency injection, Options pattern, async/await

### Project Structure
- **Entities/**: Database entities (Users, Addresses, Driver, ScheduleContract, ScheduledCollections)
- **Data/**: AppDbContext for EF Core
- **Services/**: Business logic (OpenRouteService)
- **Interfaces/**: Service contracts for DI
- **DTOs/**: Data transfer objects
  - `DTOs/Routing/`: Public domain models for route optimization
  - `DTOs/OpenRouteService/`: Internal API models for ORS integration
- **Configuration/**: Strongly-typed configuration classes (OpenRouteServiceOptions)
- **Controllers/**: API endpoints (currently only BaseController)
- **Migrations/**: EF Core database migrations

### Core Domain Model

The application manages a waste collection workflow with these relationships:

```
Users (Central entity)
├─> Addresses (1:N) - Users can have multiple service addresses
│   └─> ScheduleContract (1:N) - Each address can have collection schedules
├─> Driver (1:1) - A user can be assigned a driver profile
└─> ScheduledCollections (1:N) - Driver's daily collection assignments

Driver
└─> HomeAddressID → Addresses - Driver's starting location for route optimization
```

**Key entity notes:**
- All entities use Guid primary keys
- `ScheduledCollections.UserId` actually stores the Driver ID (naming could be clearer)
- `Addresses.Status` defaults to "Request_Bin" (string-based status, not enum)
- Driver.VehicleCapacity represents max stops before depot return is needed

### Business Workflow

1. **Bin Requests**: Users request bins for addresses → Admins approve based on external payment verification
2. **Schedule Contracts**: Users create collection schedules (weekly/bi-weekly/tri-weekly/monthly) → Admins approve
3. **Daily Route Optimization** (Core Feature): System generates optimal routes for all drivers considering:
   - Driver starting locations (home addresses)
   - Vehicle capacity constraints
   - Depot locations for garbage disposal
   - Multi-driver area distribution
4. **Driver Operations**: Drivers view daily schedule, mark collections complete, add notes

**Important**: Payment is handled completely externally. No integrated payment system. Admins approve requests based on external communication.

## OpenRouteService Integration

The `OpenRouteService` (Services/OpenRouteService.cs) provides three critical features:

### 1. Address Autocomplete
- Interactive suggestions as user types (focused on South Africa: country="ZA")
- Returns up to 20 suggestions with coordinates
- Prevents address typos and ensures valid geocoding

### 2. Address Geocoding
- Converts address strings to lat/long coordinates
- Used during address registration/verification

### 3. Route Optimization (Most Complex)
- Uses VROOM (Vehicle Routing Open-source Optimization Machine) via ORS `/optimization` endpoint
- **Input**: ~1000 daily collection jobs, ~100 drivers, depot locations
- **Output**: Optimized routes with ordered stops per driver
- Handles capacity constraints with automatic depot insertion
- Returns unassigned jobs if capacity exceeded
- Uses Haversine formula to assign each driver to nearest depot

**Configuration** (appsettings.json):
```json
"OpenRouteService": {
  "BaseUrl": "http://localhost:8082",
  "TimeoutSeconds": 300,
  "MaxRetries": 3
}
```

### External Service Requirements
1. **PostgreSQL Database**: Connection string in appsettings.Development.json or User Secrets
2. **OpenRouteService**: Must be running at configured BaseUrl with support for:
   - `/geocode/search`
   - `/geocode/autocomplete`
   - `/optimization` (VROOM API)

## Important Conventions

### Coding Patterns
- **Namespace**: `Megabin_Web` (underscore due to space in project folder name)
- **Entity Names**: Plural (Users, Addresses, etc.)
- **DTOs**: Immutable records with extensive XML documentation
- **Required Properties**: Heavy use of `required` keyword (.NET 7+)
- **Dependency Injection**: Services registered in Program.cs, injected via interfaces
- **Configuration**: Strongly-typed options classes bound via `IOptions<T>`

### File Naming
- Entities use plural names (Addresses.cs, not Address.cs)
- Quote project name in CLI commands due to space: `"Megabin Web/..."`

### Error Handling
- Extensive `ILogger<T>` usage throughout services
- Returns null for missing geocoding results
- Throws `InvalidOperationException` for critical failures
- Validates inputs before external API calls

## Development Notes

### Current Implementation Status
- Complete entity model with initial migration
- Full OpenRouteService integration implemented
- Database context and DI configured
- Docker support ready

### Not Yet Implemented
- API controllers and endpoints (only BaseController exists)
- Authentication/authorization system (roles stored as strings)
- Input validation on entities
- Global error handling middleware
- Background jobs for daily route generation
- Unit/integration tests
- Frontend UI
- API versioning and CORS configuration

### When Adding Features
- Controllers should inherit from `BaseController` which sets route convention to `[Route("api/[controller]")]`
- Use dependency injection for all services
- Follow async/await pattern consistently
- Add XML documentation to public interfaces
- Register services in Program.cs using appropriate lifetime (Scoped/Singleton/Transient)

## Configuration

### appsettings.json
Contains OpenRouteService configuration and logging levels.

### appsettings.Development.json (gitignored)
Contains PostgreSQL connection string. Use User Secrets for sensitive data:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=...;Username=...;Password=..."
```

UserSecretsId: `7480588a-4c58-4df7-ba02-3ef1eb63e256`

### Launch Profiles
- **http**: http://localhost:5250
- **https**: https://localhost:7012 / http://localhost:5250
- **Docker**: Ports 8080/8081

## Key Files for Understanding

Priority reading order:
1. `Megabin Web/readme.md` - Business domain and workflow
2. `Megabin Web/Program.cs` - Application bootstrapping and DI
3. `Megabin Web/Entities/*.cs` - Database schema
4. `Megabin Web/Interfaces/IOpenRouteService.cs` - Core service contract
5. `Megabin Web/Services/OpenRouteService.cs` - Route optimization implementation
6. `Megabin Web/Data/AppDbContext.cs` - EF Core configuration

## Complex Challenges

The most complex algorithmic challenge is **multi-driver area optimization**: When multiple drivers operate in the same geographic area, the system must intelligently distribute collection jobs while:
- Minimizing total travel time/distance
- Respecting vehicle capacity constraints
- Ensuring drivers return to depot when capacity is reached
- Balancing workload across drivers
- Considering driver starting locations

This is solved using the VROOM optimization engine via OpenRouteService, which receives all jobs and drivers and produces optimal route assignments with ordered stop lists.

---

# Frontend (Angular) Development Guidelines

## TypeScript Best Practices

- Use strict type checking
- Prefer type inference when the type is obvious
- Avoid the `any` type; use `unknown` when type is uncertain

## Angular Best Practices

- Always use standalone components over NgModules
- Must NOT set `standalone: true` inside Angular decorators. It's the default in Angular v20+.
- Use signals for state management
- Implement lazy loading for feature routes
- Do NOT use the `@HostBinding` and `@HostListener` decorators. Put host bindings inside the `host` object of the `@Component` or `@Directive` decorator instead
- Use `NgOptimizedImage` for all static images.
  - `NgOptimizedImage` does not work for inline base64 images.

## Accessibility Requirements

- It MUST pass all AXE checks.
- It MUST follow all WCAG AA minimums, including focus management, color contrast, and ARIA attributes.

### Components

- Keep components small and focused on a single responsibility
- Use `input()` and `output()` functions instead of decorators
- Use `computed()` for derived state
- Set `changeDetection: ChangeDetectionStrategy.OnPush` in `@Component` decorator
- Prefer inline templates for small components
- Prefer Reactive forms instead of Template-driven ones
- Do NOT use `ngClass`, use `class` bindings instead
- Do NOT use `ngStyle`, use `style` bindings instead
- When using external templates/styles, use paths relative to the component TS file.

## State Management

- Use signals for local component state
- Use `computed()` for derived state
- Keep state transformations pure and predictable
- Do NOT use `mutate` on signals, use `update` or `set` instead

## Templates

- Keep templates simple and avoid complex logic
- Use native control flow (`@if`, `@for`, `@switch`) instead of `*ngIf`, `*ngFor`, `*ngSwitch`
- Use the async pipe to handle observables
- Do not assume globals like (`new Date()`) are available.
- Do not write arrow functions in templates (they are not supported).

## Services

- Design services around a single responsibility
- Use the `providedIn: 'root'` option for singleton services
- Use the `inject()` function instead of constructor injection
