using Zeus.Academia.Infrastructure.Extensions;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Api.Middleware;
using Zeus.Academia.Api.Extensions;
using Zeus.Academia.Api.Configuration;
using Zeus.Academia.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Enrichers.CorrelationId;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog early
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .Enrich.WithProperty("ApplicationName", "Zeus.Academia.Api")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Clear existing logging providers since we're using Serilog
builder.Logging.ClearProviders();

// Add environment variables support
builder.Configuration.AddEnvironmentVariables("ZEUS_ACADEMIA_");

// Validate configuration
builder.Services.AddApplicationConfiguration(builder.Configuration);
builder.Services.ConfigureEnvironmentSettings(builder.Environment, builder.Configuration);

// Add API services
builder.Services.AddApiServices();

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var apiConfig = builder.Configuration.GetSection(ApiConfiguration.SectionName).Get<ApiConfiguration>()
        ?? new ApiConfiguration();

    // Configure multiple API versions for Swagger
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = apiConfig.Title,
        Version = "1.0",
        Description = $"{apiConfig.Description} - Version 1.0 with core functionality",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = apiConfig.Contact.Name,
            Email = apiConfig.Contact.Email,
            Url = new Uri(apiConfig.Contact.Url)
        }
    });

    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = apiConfig.Title,
        Version = "2.0",
        Description = $"{apiConfig.Description} - Version 2.0 with enhanced features",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = apiConfig.Contact.Name,
            Email = apiConfig.Contact.Email,
            Url = new Uri(apiConfig.Contact.Url)
        }
    });

    // Add API versioning header to Swagger
    options.AddSecurityDefinition("ApiVersion", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "X-API-Version",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "API Version Header (e.g., '1.0', '2.0')"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: Allow any origin for testing
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // Production: Restrict to specific origins
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "https://zeus-academia.com", "https://app.zeus-academia.com" };

            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Configure Rate Limiting
builder.Services.Configure<RateLimitOptions>(
    builder.Configuration.GetSection(RateLimitOptions.SectionName));

// Add Infrastructure services including Entity Framework
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add ASP.NET Core Identity
builder.Services.AddIdentity<AcademiaUser, AcademiaRole>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 6;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;

        // Sign-in settings
        options.SignIn.RequireConfirmedEmail = false; // Set to true in production
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<AcademiaDbContext>()
    .AddDefaultTokenProviders();

// Add additional Infrastructure Identity services
builder.Services.AddInfrastructureIdentityServices(builder.Configuration);

// Configure JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            // Generate a secure key for development if not configured
            var key = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(key);
            secretKey = Convert.ToBase64String(key);

            // Log warning in development
            if (builder.Environment.IsDevelopment())
            {
                Console.WriteLine("Warning: JWT SecretKey not configured. Using generated key for development.");
            }
        }

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "Zeus.Academia.Api",
            ValidAudience = jwtSettings["Audience"] ?? "Zeus.Academia.Client",
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.FromMinutes(1), // Allow 1 minute clock skew
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };

        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException))
                {
                    context.Response.Headers["Token-Expired"] = "true";
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new { error = "You are not authorized" });
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new { error = "You do not have permission to access this resource" });
                return context.Response.WriteAsync(result);
            }
        };
    });

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    // Default policy requiring authenticated users
    options.FallbackPolicy = options.DefaultPolicy;

    // Role-based policies
    options.AddPolicy(AcademiaPolicyNames.RequireStudentRole, policy =>
        policy.RequireRole(AcademicRoleType.Student.ToString()));

    options.AddPolicy(AcademiaPolicyNames.RequireProfessorRole, policy =>
        policy.RequireRole(AcademicRoleType.Professor.ToString()));

    options.AddPolicy(AcademiaPolicyNames.RequireTeachingProfRole, policy =>
        policy.RequireRole(AcademicRoleType.TeachingProf.ToString()));

    options.AddPolicy(AcademiaPolicyNames.RequireChairRole, policy =>
        policy.RequireRole(AcademicRoleType.Chair.ToString()));

    options.AddPolicy(AcademiaPolicyNames.RequireAdministratorRole, policy =>
        policy.RequireRole(AcademicRoleType.Administrator.ToString()));

    options.AddPolicy(AcademiaPolicyNames.RequireSystemAdminRole, policy =>
        policy.RequireRole(AcademicRoleType.SystemAdmin.ToString()));

    // Hierarchical policies
    options.AddPolicy(AcademiaPolicyNames.FacultyOrHigher, policy =>
        policy.RequireRole(
            AcademicRoleType.Professor.ToString(),
            AcademicRoleType.TeachingProf.ToString(),
            AcademicRoleType.Chair.ToString(),
            AcademicRoleType.Administrator.ToString(),
            AcademicRoleType.SystemAdmin.ToString()));

    options.AddPolicy(AcademiaPolicyNames.AdministrativeStaff, policy =>
        policy.RequireRole(
            AcademicRoleType.Administrator.ToString(),
            AcademicRoleType.SystemAdmin.ToString()));

    options.AddPolicy(AcademiaPolicyNames.DepartmentLeadership, policy =>
        policy.RequireRole(
            AcademicRoleType.Chair.ToString(),
            AcademicRoleType.SystemAdmin.ToString()));

    options.AddPolicy(AcademiaPolicyNames.AcademicStaff, policy =>
        policy.RequireRole(
            AcademicRoleType.Student.ToString(),
            AcademicRoleType.Professor.ToString(),
            AcademicRoleType.TeachingProf.ToString(),
            AcademicRoleType.Chair.ToString()));
});

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Configure Swagger UI for multiple versions
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Zeus Academia API v1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Zeus Academia API v2.0");

        // Set default version
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.EnableDeepLinking();
        options.DisplayOperationId();
        options.DisplayRequestDuration();

        // Add custom CSS for better styling
        options.InjectStylesheet("/css/swagger-custom.css");
    });
}

// Enable static files for custom CSS
app.UseStaticFiles();

// Global exception handling (must be early in pipeline)
app.UseMiddleware<Zeus.Academia.Api.Middleware.GlobalExceptionMiddleware>();

// API versioning middleware (before other API processing)
app.UseMiddleware<Zeus.Academia.Api.Versioning.ApiVersioningMiddleware>();

// Request/response logging and monitoring middleware
app.UseMiddleware<Zeus.Academia.Api.Middleware.RequestLoggingMiddleware>();

// Security middleware (Order is important!)
app.UseSecurityHeaders();          // Add security headers first
app.UseHttpsRedirection();         // Force HTTPS
app.UseCors("DefaultCorsPolicy");  // CORS configuration
app.UseRateLimiting();            // Rate limiting for brute force protection
app.UseSecurityLogging();         // Security event logging

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health Check endpoint
app.MapHealthChecks("/health");

app.Run();