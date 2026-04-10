# CLAUDE.md — Kudos App

## Project
Peer-to-peer employee recognition app. .NET 10 API + Angular 20 + SQLite + Docker Compose.

## Repo Structure
```
kudos-app/
├── backend/           # .NET 10 solution (Clean Architecture)
│   ├── KudosApp.slnx
│   ├── src/
│   │   ├── KudosApp.Api/
│   │   ├── KudosApp.Core/
│   │   └── KudosApp.Infrastructure/
│   └── tests/
│       └── KudosApp.Tests/
├── frontend/          # Angular 20 (standalone, signals)
├── database/          # Schema, seed, scripts
├── docs/
├── docker-compose.yml
└── .env.example
```

## Build & Run
```bash
# Backend
cd backend && dotnet build
cd backend/src/KudosApp.Api && dotnet run

# Frontend
cd frontend && npm install && ng serve

# Full stack
docker compose up
```

## Key Decisions
- 3-layer Clean Architecture (Api → Core ← Infrastructure)
- No MediatR/CQRS — direct service injection
- Signal-based state management (no NgRx)
- SQLite for simplicity (start in-memory, migrate to file)
- JWT auth, no SSO
- OpenAI API for content moderation

## Conventions
- Conventional Commits: `feat:`, `fix:`, `chore:`, `docs:`, `test:`
- Records for DTOs, encapsulated entities, Result<T> pattern
- Angular: standalone, OnPush, signals, functional guards/interceptors
