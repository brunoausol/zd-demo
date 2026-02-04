# Orders API (Microservice Demo)

This repository contains a layered .NET 9 microservice that exposes order creation and listing endpoints.
It is intentionally structured to demonstrate **zero-downtime** database migrations and **expand/contract**
API contracts with versioned endpoints.

## Architecture

Layers:
- **Orders.Api**: HTTP endpoints, wiring, and composition root.
- **Orders.Application**: Use cases, contracts (DTOs), and service orchestration.
- **Orders.Domain**: Entities and domain rules.
- **Orders.Infrastructure**: EF Core (Postgres), RabbitMQ publisher, and repositories.

Key design notes:
- **API versioning by route**: `v1` and `v2` endpoints allow expand/contract behavior.
- **EF Core + Postgres**: `OrdersDbContext` maps the `orders` table with nullable `customer_email`.
- **RabbitMQ**: publishes an `OrderCreatedEvent` on creation.
- **Migrations**: `Database:MigrateOnStartup` is enabled only for Development.

## Endpoints

- `POST /api/v1/orders` (no email)
- `GET /api/v1/orders`
- `POST /api/v2/orders` (includes `customerEmail`)
- `GET /api/v2/orders`

See examples in `src/Orders.Api/Orders.Api.http`.

## Running Locally

Prerequisites:
- .NET SDK 9.x
- PostgreSQL
- RabbitMQ

1. Restore and build:
```powershell
dotnet restore Orders.slnx
dotnet build Orders.slnx
```

2. Set connection strings (optional override):
- `ConnectionStrings:Orders` in `src/Orders.Api/appsettings.json`

3. Run the API:
```powershell
dotnet run --project src/Orders.Api
```

If you keep `Database:MigrateOnStartup` enabled in Development, the service will auto-apply migrations.

## Migrations (recommended flow)

Create the initial migration:
```powershell
dotnet ef migrations add InitialCreate -p src/Orders.Infrastructure -s src/Orders.Api
dotnet ef database update -p src/Orders.Infrastructure -s src/Orders.Api
```

## Zero-Downtime Expand/Contract

See `docs/zero-downtime.md` for a concrete migration flow, including:
- Expand schema (add nullable columns)
- Dual-write using v2
- Backfill
- Contract old endpoints/columns

## Notes

- RabbitMQ connection settings live under `RabbitMq` in `appsettings.json`.
- In production, keep `Database:MigrateOnStartup` set to `false`.
