# Zeus Academia API - Project Structure and Configuration

This document outlines the implementation of Prompt 3, Task 1: API Project Structure and Configuration.

## ✅ Task 1 Completion Status

### ✅ Implemented Components

1. **Folder Structure** - Organized API project with proper separation of concerns:

   - `/Configuration` - Configuration classes with validation
   - `/Controllers/Base` - Base API controller functionality
   - `/Controllers/V1` - Version 1.0 API endpoints
   - `/Controllers/V2` - Version 2.0 API endpoints
   - `/Extensions` - Service collection extensions

2. **Configuration Management** - Comprehensive configuration system:

   - `ApiConfiguration.cs` - API-specific settings
   - `DatabaseConfiguration.cs` - Database connection and performance settings
   - `LoggingConfiguration.cs` - Structured logging configuration
   - Environment-specific appsettings files (Development, Staging, Production)

3. **Service Registration** - Organized dependency injection:

   - `ServiceCollectionExtensions.cs` - Configuration validation and DI setup
   - Environment-specific service configuration
   - Health checks integration

4. **Base API Controller** - Common functionality:

   - `BaseApiController.cs` - Standardized response handling
   - JWT claims access through `CurrentUser` property
   - Consistent error handling and response formatting

5. **Environment Configuration** - Multi-environment support:

   - Development, Staging, and Production settings
   - Environment variable overrides with `ZEUS_ACADEMIA_` prefix
   - Security-focused production configuration

6. **API Documentation** - Swagger/OpenAPI integration:
   - JWT authentication support in Swagger UI
   - Comprehensive API documentation
   - Version-specific documentation structure

## 🏗️ Architecture Overview

### Configuration Classes

All configuration classes use Data Annotations for validation:

```csharp
public class ApiConfiguration
{
    [Required, MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string Version { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;

    public ContactInfo Contact { get; set; } = new();
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
```

### Service Registration

Environment-specific configuration with validation:

```csharp
public static IServiceCollection ConfigureAndValidate<T>(
    this IServiceCollection services,
    IConfiguration configuration,
    string sectionName) where T : class
{
    var section = configuration.GetSection(sectionName);
    services.Configure<T>(section);

    // Validate configuration on startup
    services.AddSingleton<IValidateOptions<T>, ValidateOptions<T>>();

    return services;
}
```

### API Versioning Approach

Manual versioning through controller routing and namespace organization:

- **V1 Controllers**: `/api/v1/[controller]` - Basic functionality
- **V2 Controllers**: `/api/v2/[controller]` - Enhanced features
- **Versioned Namespaces**: `Zeus.Academia.Api.Controllers.V1|V2`

### Environment Configuration

#### Development

- Detailed error messages
- Sensitive data logging enabled
- Any origin CORS policy
- Swagger UI enabled

#### Staging

- Reduced logging verbosity
- Specific origin CORS restrictions
- Swagger UI enabled for testing
- Connection pooling optimized

#### Production

- Security-focused configuration
- HTTPS enforcement
- Minimal logging for performance
- Strict CORS policy
- No sensitive data exposure

## 🔧 Configuration Examples

### Environment Variables

Override any configuration using the `ZEUS_ACADEMIA_` prefix:

```bash
# Database configuration
ZEUS_ACADEMIA_Database__ConnectionString="Server=prod-server;Database=Zeus_Academia_Prod;..."
ZEUS_ACADEMIA_Database__CommandTimeout=60

# API configuration
ZEUS_ACADEMIA_Api__Title="Zeus Academia Production API"
ZEUS_ACADEMIA_Api__AllowedOrigins__0="https://app.zeus-academia.com"

# Logging configuration
ZEUS_ACADEMIA_AppLogging__MinimumLevel="Warning"
ZEUS_ACADEMIA_AppLogging__EnableSensitiveDataLogging=false
```

### appsettings.json Structure

```json
{
  "Api": {
    "Title": "Zeus Academia API",
    "Version": "1.0",
    "Description": "Academic management system API",
    "Contact": {
      "Name": "Zeus Academia Development Team",
      "Email": "dev@zeus.academia",
      "Url": "https://zeus.academia/support"
    },
    "AllowedOrigins": ["http://localhost:3000", "https://localhost:3001"]
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=Zeus_Academia_Dev;Trusted_Connection=true;TrustServerCertificate=true;",
    "CommandTimeout": 30,
    "MaxRetryCount": 3,
    "MaxRetryDelay": 30,
    "EnablePooling": true,
    "MaxPoolSize": 100
  },
  "AppLogging": {
    "MinimumLevel": "Information",
    "EnableSensitiveDataLogging": true,
    "LogToFile": true,
    "LogFilePath": "Logs/zeus-academia-{Date}.txt",
    "SensitiveDataFields": ["Password", "Token", "Key", "Secret"]
  }
}
```

## 🚀 Usage Examples

### Creating Versioned Controllers

1. **V1 Controller** (Basic functionality):

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class StudentsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetStudentsAsync()
    {
        // Basic student listing
        return Ok(await GetBasicStudentListAsync());
    }
}
```

2. **V2 Controller** (Enhanced functionality):

```csharp
[ApiController]
[Route("api/v2/[controller]")]
public class StudentsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetStudentsAsync(
        [FromQuery] StudentFilters filters,
        [FromQuery] PaginationOptions pagination)
    {
        // Enhanced student listing with filtering and pagination
        return Ok(await GetEnhancedStudentListAsync(filters, pagination));
    }
}
```

### Using Base Controller Features

```csharp
public class ExampleController : BaseApiController
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfileAsync()
    {
        // Access current user claims
        var userId = CurrentUser.UserId;
        var userEmail = CurrentUser.Email;
        var userRoles = CurrentUser.Roles;

        // Use standardized responses
        return SuccessResponse(userProfile, "Profile retrieved successfully");
    }

    [HttpPost]
    public async Task<IActionResult> CreateResourceAsync([FromBody] CreateRequest request)
    {
        try
        {
            var result = await CreateResourceAsync(request);
            return CreatedResponse(result, "Resource created successfully");
        }
        catch (ValidationException ex)
        {
            return ValidationErrorResponse(ex.Message);
        }
        catch (Exception ex)
        {
            return ErrorResponse("An error occurred while creating the resource");
        }
    }
}
```

## 🔍 Health Checks

The API includes health check endpoints:

- `/health` - Basic system health
- `/api/v1/version/health` - V1 API health information
- `/api/v2/version/health` - V2 API health with detailed metrics

## 📋 Next Steps

1. **API Versioning Enhancement**: Consider adding a proper API versioning package when available for .NET 9.0
2. **Advanced Health Checks**: Add database connectivity and external service health checks
3. **Metrics Integration**: Add application metrics collection and monitoring
4. **Request/Response Logging**: Implement structured request/response logging middleware
5. **API Rate Limiting**: Enhance rate limiting with user-specific limits

## ✅ Task 1 Acceptance Criteria Met

- ✅ Proper folder structure for API organization
- ✅ Configuration classes with validation attributes
- ✅ Environment-specific appsettings files
- ✅ Service collection extensions for DI setup
- ✅ Base API controller with common functionality
- ✅ API versioning structure (manual approach)
- ✅ Health check endpoints
- ✅ Swagger/OpenAPI documentation with JWT support
- ✅ Environment variable configuration overrides
- ✅ Security headers and CORS configuration

The API project structure is now properly configured and ready for development across multiple environments with comprehensive configuration management, versioning support, and documentation.
