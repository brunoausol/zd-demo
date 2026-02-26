# Orders API (Expand & Contract Demo)

This repository contains a layered .NET 9 microservice that exposes order creation and listing endpoints.
It is intentionally structured to demonstrate **expand/contract** in multiple contract surfaces:
- HTTP contracts
- Database schema
- RabbitMQ integration events
- Background processing behavior (RabbitMQ consumers and Hangfire jobs)

## ABP Equivalent Solution

This branch also includes an ABP-based equivalent implementation at `Orders.Abp.sln`:
- **Host**: `src/Orders.Abp.HttpApi.Host`
- **HTTP API**: `src/Orders.Abp.HttpApi`
- **Application**: `src/Orders.Abp.Application` + `src/Orders.Abp.Application.Contracts`
- **Domain**: `src/Orders.Abp.Domain` + `src/Orders.Abp.Domain.Shared`
- **Infrastructure (EF Core)**: `src/Orders.Abp.EntityFrameworkCore`

Official ABP integrations used:
- `Volo.Abp.EventBus.RabbitMq` for distributed events (`OrderCreatedEto`)
- `Volo.Abp.BackgroundJobs.HangFire` for delayed jobs (`SendPurchaseSurveyJob`)

Equivalent endpoints:
- `POST /orders`
- `GET /orders`
- `GET /orders/{orderId}`
- `GET /hangfire`
- `GET /health/live`
- `GET /health/ready`

Run:
```powershell
dotnet restore Orders.Abp.sln
dotnet run --project src/Orders.Abp.HttpApi.Host
```

## Architecture

Layers:
- **Orders.Api**: HTTP endpoints, wiring, and composition root.
- **Orders.Application**: Use cases, contracts (DTOs), and service orchestration.
- **Orders.Domain**: Entities and domain rules.
- **Orders.Infrastructure**: EF Core (Postgres), RabbitMQ publisher/consumer, Hangfire scheduler/jobs, and repositories.

Key design notes:
- **EF Core + Postgres**: `OrdersDbContext` maps the `orders` table.
- **RabbitMQ publisher**: API publishes `OrderCreatedEvent` when a new order is created.
- **RabbitMQ consumer**: background worker consumes the event and sends order confirmation e-mail.
- **Hangfire scheduler**: API schedules a purchase survey to be sent after a delay.
- **Hangfire router/envelope pattern**: scheduled jobs are wrapped in envelopes and routed by message type.
- **Reliability**: publisher uses broker confirms and consumer sends failed messages to DLQ.
- **Migrations**: `Database:MigrateOnStartup` is enabled only for Development.

## Baseline V1

This branch represents the **v1 baseline** used as the starting point for expand/contract demos.

Current contracts in v1:
- **HTTP**: `CreateOrderRequest` and `OrderResponse` use `customerName`.
- **Database**: `orders.customer_name` stores the customer full name.
- **RabbitMQ event**: `OrderCreatedEvent` carries `customerName`.
- **Background processing**: RabbitMQ consumer sends order confirmation and Hangfire sends delayed purchase survey.

Versioning strategy for upcoming branches:
- Breaking changes create a new major version (`v2`, `v3`, ...).
- Previous major stays active during migration window.

Expand/contract execution strategy:
- Branch per phase, always starting from this baseline branch.
- First expand with compatibility.
- Then run migration/backfill and dual support window.
- Finally contract by removing the previous major.

## Endpoints

- `POST /orders`
- `GET /orders`
- `GET /orders/{orderId}`
- `GET /hangfire`
- `GET /health/live`
- `GET /health/ready`

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
- Hangfire settings live under `Hangfire` in `appsettings.json`.
- This branch is intentionally conservative and versioned as **v1** across contracts.
- In production, keep `Database:MigrateOnStartup` set to `false`.

## Kubernetes

The API exposes split probes:
- **Liveness**: `/health/live` (process alive)
- **Readiness**: `/health/ready` (database + RabbitMQ reachable)

Graceful shutdown behavior:
- RabbitMQ consumer cancels intake and waits for in-flight messages up to `RabbitMq:ShutdownDrainTimeoutSeconds`.
- Hangfire server uses `Hangfire:ShutdownTimeoutSeconds` to finish in-flight jobs.
- Host shutdown timeout is aligned with these values.

Example deployment probes:
```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 10
readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 5
terminationGracePeriodSeconds: 60
```

Sample manifests are available under `k8s/`:
- `k8s/orders-api-configmap.yaml`
- `k8s/orders-api-secret.yaml`
- `k8s/orders-api-deployment.yaml`
- `k8s/orders-api-service.yaml`

Apply in order:
```powershell
kubectl apply -f k8s/orders-api-configmap.yaml
kubectl apply -f k8s/orders-api-secret.yaml
kubectl apply -f k8s/orders-api-deployment.yaml
kubectl apply -f k8s/orders-api-service.yaml
```

Notes for cluster usage:
- Update image in `k8s/orders-api-deployment.yaml` (`orders-api:dev`) to your registry image.
- Ensure Postgres and RabbitMQ services are reachable as `postgres` and `rabbitmq`, or adjust env values in `k8s/orders-api-configmap.yaml`.
