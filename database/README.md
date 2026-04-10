# Database

SQLite database managed by Entity Framework Core 10 migrations.

## Files
- `schema/` — Reference SQL schema (EF Core generates actual migrations)
- `seed/` — Reference seed data (EF Core `DataSeeder` handles actual seeding)
- `scripts/` — Utility scripts for development

## Notes
- EF Core migrations are the source of truth for schema
- SQL files are for documentation and quick reference
- Database file: `backend/src/KudosApp.Api/data/kudos.db`
