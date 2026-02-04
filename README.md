# Orders API (Microservice Demo)

This repository contains a layered .NET 9 microservice that exposes order creation and listing endpoints.
It is intentionally structured to demonstrate **zero-downtime** database migrations and **expand/contract**
API contracts.

## Architecture

Layers:
- **Orders.Api**: HTTP endpoints, wiring, and composition root.
- **Orders.Application**: Use cases, contracts (DTOs), and service orchestration.
- **Orders.Domain**: Entities and domain rules.
- **Orders.Infrastructure**: EF Core (Postgres), RabbitMQ publisher, and repositories.

Key design notes:
- **EF Core + Postgres**: `OrdersDbContext` maps the `orders` table.
- **RabbitMQ**: publishes an `OrderCreatedEvent` on creation.
- **Migrations**: `Database:MigrateOnStartup` is enabled only for Development.

## Endpoints

- `POST /orders`
- `GET /orders`
- `GET /orders/{orderId}`

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

2. Set connection strings:
- `ConnectionStrings:Orders` in `src/Orders.Api/appsettings.json`

3. Run the API:
```powershell
dotnet run --project src/Orders.Api
```

If you keep `Database:MigrateOnStartup` enabled in Development, the service will auto-apply migrations.

## Running with Docker Compose

There is a compose setup under `docker/docker-compose.yml` that brings up:
- `orders-api`
- `postgres`
- `rabbitmq` (management UI exposed on port 15672)

From the repo root:
```powershell
docker compose -f docker/docker-compose.yml up --build
```

You can tweak local settings in `docker/.env`.

## Migrations (recommended flow)

Create the initial migration:
```powershell
dotnet ef migrations add InitialCreate -p src/Orders.Infrastructure -s src/Orders.Api
dotnet ef database update -p src/Orders.Infrastructure -s src/Orders.Api
```

## Notes

- RabbitMQ connection settings live under `RabbitMq` in `appsettings.json`.
- In production, keep `Database:MigrateOnStartup` set to `false`.
