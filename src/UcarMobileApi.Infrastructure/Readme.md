## EF Migrations

Go to the root of the solution (where the `.sln` file is located).

---

### GENERATE A MIGRATION

```bash
dotnet ef migrations add MigrationName \
  --project src\UcarMobileApi.Infrastructure \
  --startup-project src\UcarMobileApi
```

---

### ROLLBACK DATABASE TO PREVIOUS MIGRATION (when the last migration is already applied)

1. List migrations to identify the previous one:

```bash
dotnet ef migrations list \
  --project src\UcarMobileApi.Infrastructure \
  --startup-project src\UcarMobileApi
```

2. Update the database to the previous migration:

```bash
dotnet ef database update PreviousMigrationName \
  --project src\UcarMobileApi.Infrastructure \
  --startup-project src\UcarMobileApi
```

> This executes the `Down()` method of the last migration and reverts the database schema.

---

### REMOVE LAST MIGRATION (code only)

After the database has been rolled back:

```bash
dotnet ef migrations remove \
  --project src\UcarMobileApi.Infrastructure \
  --startup-project src\UcarMobileApi
```

---

### APPLY MIGRATIONS TO DATABASE

```bash
dotnet ef database update \
  --project src\UcarMobileApi.Infrastructure \
  --startup-project src\UcarMobileApi
```

---

### ⚠️ Important Notes

* `migrations remove` **cannot be used** if the migration is already applied to the database.
* In **local/dev environments**, rollback + remove is safe.
* In **staging/production**, **do not remove migrations** — create a new corrective migration instead.
