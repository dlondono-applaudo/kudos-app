# Kudos App — Copilot Instructions

## Project Context
Peer-to-peer employee recognition app where employees give kudos, earn points, and celebrate contributions.

## Tech Stack
- **Backend**: .NET 10 Web API (C#), Entity Framework Core 10, SQLite
- **Frontend**: Angular 20 (standalone components, signals, new control flow)
- **Auth**: JWT Bearer tokens (email/password registration)
- **AI**: OpenAI API for content moderation
- **Infra**: Docker Compose (api + ui via nginx)

## Architecture

### Backend (Clean Architecture — 4 layers)
- `KudosApp.Api` — Minimal API Endpoints, Middleware, Extensions, Program.cs
- `KudosApp.Domain` — Entities (encapsulated), DTOs (records), Interfaces, FluentValidation Validators
- `KudosApp.Application` — Service implementations, Business logic, DI registration
- `KudosApp.Infrastructure` — EF Core DbContext, SQLite

**Dependency rule**: Api → Application + Domain. Application → Domain + Infrastructure. Infrastructure → Domain. Domain → nothing.

### Frontend (Feature-based)
- `core/` — Singletons: guards (functional), interceptors (functional), layout, error handler
- `shared/` — Reusable: domain types (with null objects), signal stores (#private WritableSignal), API services, UI components (OnPush + signal inputs)
- `features/` — Lazy-loaded routes: auth, feed, profile, leaderboard, admin

## Coding Standards

### C# / .NET
- Use `record` types for DTOs (immutable)
- Use `Result<T>` pattern for service returns (no throwing for business logic)
- Entities encapsulate behavior (not anemic models) — use `IReadOnlyList` for collections
- `AsNoTracking()` on all read queries
- `CancellationToken` on all async methods
- Conventional Commits in English: `feat:`, `fix:`, `chore:`, `docs:`, `test:`

### Angular / TypeScript
- Standalone components only — no NgModules
- `ChangeDetectionStrategy.OnPush` on every component
- Signal inputs: `input()` instead of `@Input()`
- Signal outputs: `output()` instead of `@Output()`
- New control flow: `@if`, `@for`, `@switch` (not `*ngIf`, `*ngFor`)
- Signal stores with `#private WritableSignal` + public `computed()` + `asReadonly()`
- Path aliases: `@domain/`, `@state/`, `@ui/`, `@services/`, `@env/`
- Functional guards and interceptors
- `inject()` function instead of constructor injection

## File Naming
- Components: `[name].component.ts`
- Services: `[name].service.ts`
- Guards: `[name].guard.ts`
- Interceptors: `[name].interceptor.ts`
- Stores: `[name].store.ts`
- Types: `[name].type.ts`
- Kebab-case for folders

## API Patterns
- Minimal API Endpoints with `MapGroup()` and `WithTags()`
- FluentValidation for request validation
- Global exception middleware → ProblemDetails
- Output caching on GET endpoints
- Role-based authorization: `.RequireAuthorization()` / `.RequireRole("Admin")`
- Scalar (`/scalar/v1`) for interactive API docs
