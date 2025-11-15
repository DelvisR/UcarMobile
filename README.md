# Backend RESTful API

REST API developed in .NET 9

## Useful Commands

### Restore NuGet packages
```bash
dotnet restore
```

### Build the solution
```bash
dotnet build
```

### Run the application
```bash
cd src/UcarMobileApi

# You can define environment variables to match the configuration files appsettings.Development.json and appsettings.Production.json:

SET ASPNETCORE_ENVIRONMENT=Development or SET ASPNETCORE_ENVIRONMENT=Production

# Or if you want to use the files appsettings.Development.local.json and appsettings.Production.local.json

SET LOCAL=true

# then

dotnet run

# You can also run using a profile defined in the launchSettings.json file, for example:

dotnet run --launch-profile "UcarMobileApi-Dev"

dotnet run --launch-profile "UcarMobileApi-Dev-local"
```

### Publish the application
```bash
# Self Contained:

# Linux: 

dotnet publish src/UcarMobileApi/UcarMobileApi.csproj -c Release -r linux-x64 --self-contained true -o ./publish/Linux

# Windows: 

dotnet publish src/UcarMobileApi/UcarMobileApi.csproj -c Release -r win-x64 --self-contained true -o ./publish/Win

# Framework Dependent:

dotnet publish src/UcarMobileApi/UcarMobileApi.csproj -c Release -o ./publish/Framework-Dep


```

## [See the complete documentation on the wiki](https://github.com/UcarMobile/ucm-api/wiki)