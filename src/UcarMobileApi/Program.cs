using UcarMobileApi.Configuration;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Services;

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

// Register DB connection factory and DbContext
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddAppDbContext();

// Cognito Root User (our new method)
builder.Services.AddCognitoRootUser(builder.Configuration, awsSettings);

// Enable Memory Cache
builder.Services.AddMemoryCache();

// Add services
builder.Services.AddApplicationServices();

// Add infraestructure services
builder.Services.AddInfrastructure(awsSettings);

// Authorization & Policies (Cognito + Dynamic Action)
builder.Services.AddCognitoAuthAndPolicies(awsSettings);

// Cors service
builder.Services.AddCorsServices(builder.Configuration);

builder.Services.AddAuthorizationServices();

// Configure NewtonsoftJson
builder.Services.AddMvcConfiguration();

// Swagger
builder.Services.AddSwaggerDocumentation();

// Register AutoMapper (scan Application assembly)
builder.Services.AddAutoMapperProfiles();

// Configure FluentValidation
builder.Services.AddFluentValidationConfig();

// Configure basic security services such as rate limiting
builder.Services.AddBasicSecurity();

// Build app
var app = builder.Build();

// Apply database migrations before receiving requests
await UcarMobileApi.Infrastructure.Data.DatabaseInitializer.InitializeAsync(app.Services);

// Middleware pipeline
app.UseCustomMiddleware(); // ExceptionHandlingMiddleware

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

// Basic Security Middleware (HSTS, headers, rate limiting)
app.UseBasicSecurity(app.Environment);

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCorsConfiguration();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
