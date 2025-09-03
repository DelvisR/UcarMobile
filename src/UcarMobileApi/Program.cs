using UcarMobileApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.AddSerilogConfiguration();

// Sentry configuration
builder.AddSentryConfiguration();

// Add DbContext
builder.Services.AddAppDbContext(builder.Configuration);

// Add services
builder.Services.AddApplicationServices();

// Configure NewtonsoftJson
builder.Services.AddMvcConfiguration();

// Swagger
builder.Services.AddSwaggerDocumentation();

// Register AutoMapper (scan Application assembly)
builder.Services.AddAutoMapperProfiles();

// Configure FluentValidation
builder.Services.AddFluentValidationConfig();

var app = builder.Build();

// Global exception handlers (outside pipeline)
GlobalExceptionConfiguration.ConfigureGlobalExceptionHandlers();

// Middleware
app.UseCustomMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();