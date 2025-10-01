using Zeus.Academia.Infrastructure.Extensions;
using Zeus.Academia.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure services including Entity Framework
builder.Services.AddInfrastructureServices(builder.Configuration);

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
}); var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();