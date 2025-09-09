## EF Migration

Go to the root of the solution (where the .sln file is located).

### GENERATE EF MIGRATION
```
 dotnet ef migrations add MigrationName --project src\UcarMobileApi.Infrastructure --startup-project src\UcarMobileApi
```

### REMOVE LAST MIGRATION
```
dotnet ef migrations remove --project src\UcarMobileApi.Infrastructure --startup-project src\UcarMobileApi
```

### DATABASE UPDATE
```
dotnet ef database update --project src\UcarMobileApi.Infrastructure --startup-project src\UcarMobileApi
```