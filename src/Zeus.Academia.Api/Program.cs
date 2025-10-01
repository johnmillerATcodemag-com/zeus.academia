using Zeus.Academia.Infrastructure.Extensions;
using Zeus.Academia.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure services including Entity Framework
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configure Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
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