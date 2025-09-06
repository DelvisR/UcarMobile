using UcarMobileApi.Configuration;
using UcarMobileApi.Infrastructure.Configurations.Settings;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.AddSerilogConfiguration();

// Sentry configuration
builder.AddSentryConfiguration();

// Global exception handlers (outside HTTP pipeline)
GlobalExceptionConfiguration.ConfigureGlobalExceptionHandlers();

// App settings (Configuration POCOs)
builder.Services.AddAppSettings(builder.Configuration);

// Get typed settings instance
var awsSettings = builder.Configuration.GetSection("AWS").Get<AwsSettings>()!;

// Register AWS Secrets Manager
builder.Services.AddAwsSecretsManager(awsSettings);

// Add DbContext
builder.Services.AddAppDbContext(builder.Configuration);

// Enable Memory Cache
builder.Services.AddMemoryCache();

// Add services
builder.Services.AddApplicationServices();

// Authorization & Policies (Cognito + Dynamic Permissions)
builder.Services.AddCognitoAuthAndPolicies(awsSettings);


builder.Services.AddAuthorizationServices();

// Configure NewtonsoftJson
builder.Services.AddMvcConfiguration();

// Swagger
builder.Services.AddSwaggerDocumentation();

// Register AutoMapper (scan Application assembly)
builder.Services.AddAutoMapperProfiles();

// Configure FluentValidation
builder.Services.AddFluentValidationConfig();

// Build app
var app = builder.Build();

// Aplicar migraciones de base de datos antes de recibir solicitudes
await UcarMobileApi.Infrastructure.Data.DatabaseInitializer.InitializeAsync(app.Services);

// Middleware pipeline
app.UseCustomMiddleware(); // ExceptionHandlingMiddleware

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();