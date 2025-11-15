using UcarMobileApi.Configuration;
using UcarMobileApi.Infrastructure.Configurations.Settings;
using UcarMobileApi.Infrastructure.Factories;

var builder = WebApplication.CreateBuilder(args);

// Load appsettings.{Environment}.local.json (optional)
var isLocalEnv = builder.Configuration.GetValue("LOCAL", false);

if (isLocalEnv)
{
    builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.local.json", true, true);
}

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
builder.Services.AddAppDbContext(builder.Environment);

// Register Stripe factory and service
builder.Services.AddSingleton<StripeClientFactory>();

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

// Register health checks for AWS Elastic Load Balancer (ELB) or any monitoring tool
builder.Services.AddHealthChecks();

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

builder.Services.AddGridifyConfiguration();

// Build app
var app = builder.Build();

// Apply database migrations before receiving requests
await UcarMobileApi.Infrastructure.Data.DatabaseInitializer.InitializeAsync(app.Services);

// Initializes Stripe API configuration
await app.Services.InitializeStripeAsync();

// Middleware pipeline
app.UseCustomMiddleware(); // ExceptionHandlingMiddleware

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

// Basic Security Middleware (HSTS, headers, rate limiting)
app.UseBasicSecurity(app.Environment);

if (!app.Environment.IsDevelopment() && !isLocalEnv)
{
    app.UseHttpsRedirection();
}

app.UseCorsConfiguration();

app.UseAuthentication();
app.UseAuthorization();

// Public Health check for AWS ELB or any monitoring tool
app.MapHealthChecks("/health").AllowAnonymous();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Urls.Count > 0
        ? string.Join(", ", app.Urls)
        : "No addresses found (might be behind a proxy)";

    Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
    Console.WriteLine($"Application started and listening on: {addresses}");
});

app.Run();
