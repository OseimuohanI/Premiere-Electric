https://charming-paprenjak-ca099d.netlify.app/
## Database Migrations

Automatic migrations are **disabled by default** on startup to prevent database errors when running without a valid connection string (e.g., on Railway).

### Enable migrations on Railway

Set the environment variable in Railway dashboard:

- **Variable**: `ENABLE_MIGRATIONS`
- **Value**: `true`

This allows `dbContext.Database.Migrate()` to run on app startup, applying any pending migrations to your database.

**Important**: Ensure your `ConnectionStrings__DefaultConnection` is valid **before** enabling this to avoid startup failures.

### Run migrations manually (development)

```bash
dotnet ef database update
```

### Create a new migration

```bash
dotnet ef migrations add MigrationName
```
