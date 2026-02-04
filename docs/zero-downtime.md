# Zero-Downtime + Expand/Contract Demo

This service already includes **v1** and **v2** endpoints to demonstrate expand/contract:

- `POST /api/v1/orders` + `GET /api/v1/orders` (no email)
- `POST /api/v2/orders` + `GET /api/v2/orders` (adds `customerEmail`)

## Suggested migration flow
1. **Expand**: add a nullable `customer_email` column.
2. **Dual-write**: v2 writes `customer_email`; v1 still works (nulls allowed).
3. **Backfill**: populate `customer_email` where possible.
4. **Contract**: once all callers are on v2, drop v1 endpoints and old columns if any.

## Notes
- `OrdersDbContext` maps `customer_email` as nullable to support step 1.
- Keep `Database:MigrateOnStartup` disabled for production to control rollout.
