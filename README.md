# Kudos App

A peer-to-peer employee recognition application where employees give each other kudos, earn points, and celebrate contributions.

## Tech Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Backend | .NET (ASP.NET Core Web API) | 10.0 |
| Frontend | Angular (standalone, signals) | 20.x |
| Database | SQLite (via EF Core) | — |
| AI | OpenAI API (content moderation) | gpt-4o-mini |
| Containerization | Docker Compose | — |

### Why this stack?
- **.NET 10**: LTS release, excellent performance, built-in validation, output caching, OpenAPI 3.1
- **Angular 20**: Standalone components, signals for reactive state, new control flow syntax, OnPush by default
- **SQLite**: Zero-config, portable `.db` file, perfect for demo — production would use PostgreSQL
- **Docker Compose**: Single `docker compose up` to run everything

## Architecture

**Backend — 3-layer Clean Architecture**:
- `KudosApp.Api` — Controllers, DTOs, Middleware
- `KudosApp.Core` — Entities, Services, Interfaces, Business logic
- `KudosApp.Infrastructure` — EF Core, Repositories, External services (OpenAI)

**Frontend — Feature-based**:
- `core/` — Guards, interceptors, layout (singletons)
- `shared/` — Domain types, signal stores, API services, reusable UI components
- `features/` — Lazy-loaded routes (auth, feed, profile, leaderboard, admin)

## Setup & Run

### Prerequisites
- Docker & Docker Compose
- (Optional) .NET 10 SDK, Node.js 22+, Angular CLI

### Quick Start (Docker)
```bash
git clone https://github.com/dlondono-applaudo/kudos-app.git
cd kudos-app
cp .env.example .env
# Edit .env and set your OPENAI_API_KEY
docker compose up
```
- **Frontend**: http://localhost:4200
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

### Local Development
```bash
# Backend
cd backend
dotnet run --project src/KudosApp.Api

# Frontend (separate terminal)
cd frontend
npm install
ng serve
```

## Features

### Core (Minimum Requirements)
- [ ] User registration and login (JWT)
- [ ] Give kudos to another employee (recipient, category, message)
- [ ] Public kudos feed
- [ ] Points system (categories have different point values)
- [ ] Role-based access (User / Admin)
- [ ] SQLite database
- [ ] Angular Web UI

### Extended
- [ ] Badges and achievements
- [ ] Leaderboard
- [ ] Admin dashboard with analytics
- [ ] Notifications (in-app)
- [ ] Audit logging

### AI Features
- [ ] AI Content Moderation — validates kudos messages are appropriate, professional, and genuine before posting

## AI Tools Used

| Tool | How it helped |
|------|--------------|
| GitHub Copilot | Code generation, autocompletion, test generation |
| Claude (via VS Code) | Architecture planning, code review, full-file generation |

## AI Artifacts
- [`.github/copilot-instructions.md`](.github/copilot-instructions.md) — Project context for Copilot
- [`CLAUDE.md`](CLAUDE.md) — Project rules for Claude

## What I Would Do Next
- Migrate to PostgreSQL for production (SQLite is single-writer)
- Add refresh tokens and token rotation
- Implement WebSocket/SSE for real-time notifications
- Add image attachments on kudos
- Quarterly kudos allocation system
- Gift catalog and point redemption
- E2E tests with Playwright
- CI/CD pipeline with GitHub Actions

## Project Structure
```
kudos-app/
├── backend/                # .NET 10 Web API (Clean Architecture)
│   ├── KudosApp.slnx
│   ├── src/
│   │   ├── KudosApp.Api/
│   │   ├── KudosApp.Core/
│   │   └── KudosApp.Infrastructure/
│   └── tests/
│       └── KudosApp.Tests/
├── frontend/               # Angular 20 (standalone, signals)
│   ├── src/app/
│   │   ├── core/
│   │   ├── shared/
│   │   └── features/
│   └── ...
├── database/               # Reference SQL schema and seed data
├── docs/                   # Additional documentation
├── docker-compose.yml      # Single-command setup
├── .env.example            # Environment variable template
├── .github/
│   └── copilot-instructions.md
└── CLAUDE.md
```