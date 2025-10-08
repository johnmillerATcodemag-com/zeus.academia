# Implementation Prompt: Core API Infrastructure and Middleware

## Context & Overview

Implement the foundational API infrastructure for Zeus Academia System including API controllers, middleware pipeline, error handling, logging, validation, and API documentation. This creates the backbone for all subsequent API endpoints.

## Prerequisites

- Database schema implementation completed (01-foundation-database-schema.md)
- Authentication system implemented (02-foundation-authentication.md)
- Zeus.Academia.Api project structure ready

## Implementation Tasks

### Task 1: API Project Structure and Configuration

**Task Description**: Set up the API project structure with proper configuration management.

**Technical Requirements**:

- Configure `Program.cs` with proper service registration
- Set up `appsettings.json` with environment-specific configurations
- Create folder structure: Controllers, Models, Services, Middleware
- Configure dependency injection container
- Set up environment-specific settings (Development, Staging, Production)

**Acceptance Criteria**:

- [ ] Program.cs properly configured with all services
- [ ] Configuration system handles multiple environments
- [ ] Folder structure follows clean architecture principles
- [ ] DI container properly configured for all services
- [ ] Environment variables override appsettings values

### Task 2: Global Exception Handling Middleware

**Task Description**: Implement comprehensive error handling and exception middleware.

**Technical Requirements**:

- Create `GlobalExceptionMiddleware` class
- Implement custom exception types: `BusinessLogicException`, `ValidationException`, `NotAuthorizedException`
- Add structured error responses with consistent format
- Implement error logging with correlation IDs
- Add development vs production error detail handling

**Acceptance Criteria**:

- [ ] Global exception middleware catches all unhandled exceptions
- [ ] Custom exception types properly handled
- [ ] Error responses follow consistent JSON format
- [ ] All errors logged with correlation IDs
- [ ] Sensitive information not exposed in production

### Task 3: Request/Response Logging and Monitoring

**Task Description**: Implement comprehensive request/response logging and performance monitoring.

**Technical Requirements**:

- Create `RequestLoggingMiddleware` for HTTP request/response logging
- Add performance monitoring with execution time tracking
- Implement correlation ID generation and propagation
- Add structured logging with Serilog
- Configure log filtering and levels

**Acceptance Criteria**:

- [ ] All HTTP requests/responses logged with details
- [ ] Performance metrics captured for all endpoints
- [ ] Correlation IDs generated and tracked across requests
- [ ] Structured logging configured with proper sinks
- [ ] Log levels configurable per environment

### Task 4: Input Validation and Model Binding

**Task Description**: Implement comprehensive input validation and model binding infrastructure.

**Technical Requirements**:

- Configure FluentValidation for model validation
- Create base validator classes for common patterns
- Implement automatic model validation in API pipeline
- Add custom validation attributes for academic-specific rules
- Create validation error response formatting

**Acceptance Criteria**:

- [ ] FluentValidation integrated into API pipeline
- [ ] Base validator classes created for reuse
- [ ] Model validation occurs automatically before controller actions
- [ ] Custom validators for academic rules (e.g., empNr format)
- [ ] Validation errors returned in consistent format

### Task 5: API Versioning and Documentation

**Task Description**: Implement API versioning strategy and comprehensive documentation.

**Technical Requirements**:

- Configure API versioning with header-based versioning
- Set up Swagger/OpenAPI documentation
- Create API documentation with examples and schemas
- Add XML documentation comments to all public APIs
- Configure Swagger UI with authentication support

**Acceptance Criteria**:

- [ ] API versioning configured and working
- [ ] Swagger documentation generated automatically
- [ ] All endpoints documented with examples
- [ ] Authentication integrated into Swagger UI
- [ ] API documentation includes model schemas

### Task 6: Response Formatting and CORS

**Task Description**: Standardize API responses and configure cross-origin access.

**Technical Requirements**:

- Create standardized response wrapper classes
- Implement consistent pagination for list endpoints
- Configure CORS policies for frontend applications
- Add response compression middleware
- Implement content negotiation (JSON/XML support)

**Acceptance Criteria**:

- [ ] All API responses use consistent wrapper format
- [ ] Pagination implemented with skip/take parameters
- [ ] CORS configured for development and production origins
- [ ] Response compression reduces payload sizes
- [ ] Content negotiation supports multiple formats

### Task 7: Health Checks and Monitoring Endpoints

**Task Description**: Implement health checking and monitoring endpoints for system observability.

**Technical Requirements**:

- Add health check endpoints for database, external services
- Create detailed health check responses
- Implement metrics collection endpoint
- Add readiness and liveness probes
- Configure health check UI for monitoring

**Acceptance Criteria**:

- [ ] Health check endpoints return database connectivity status
- [ ] Detailed health information available for monitoring
- [ ] Metrics endpoint provides performance data
- [ ] Kubernetes-compatible readiness/liveness probes
- [ ] Health check UI accessible for system monitoring

## Verification Steps

### Component-Level Verification

1. **Middleware Pipeline Tests**

   ```csharp
   [Test]
   public async Task ExceptionMiddleware_Should_Handle_Unhandled_Exceptions()
   {
       // Test exception handling and error response format
   }

   [Test]
   public async Task RequestLogging_Should_Log_All_Requests()
   {
       // Verify request/response logging functionality
   }
   ```

2. **Validation Tests**

   ```csharp
   [Test]
   public async Task Invalid_Model_Should_Return_Validation_Errors()
   {
       // Test model validation and error response
   }

   [Test]
   public async Task Valid_Model_Should_Pass_Validation()
   {
       // Test successful validation flow
   }
   ```

3. **API Documentation Tests**
   ```csharp
   [Test]
   public void Swagger_Should_Generate_Valid_OpenAPI_Spec()
   {
       // Verify Swagger documentation generation
   }
   ```

### Integration Testing

1. **End-to-End API Flow**

   - Request → Logging → Authentication → Validation → Controller → Response
   - Error scenarios with proper error handling
   - Performance monitoring and correlation ID tracking

2. **Configuration Testing**
   - Environment-specific configuration loading
   - Override behavior with environment variables
   - Service registration and DI container resolution

### Performance Testing

1. **Middleware Performance**

   - Logging middleware adds < 5ms overhead
   - Exception handling doesn't impact normal flow
   - Response compression improves payload efficiency

2. **Memory and Resource Usage**
   - No memory leaks in middleware pipeline
   - Proper disposal of resources
   - Acceptable CPU usage under load

## Code Quality Standards

- [ ] All middleware follows ASP.NET Core conventions
- [ ] Error handling doesn't expose sensitive information
- [ ] Logging follows structured logging best practices
- [ ] API documentation is comprehensive and accurate
- [ ] Performance impact of middleware is minimal
- [ ] Code coverage >90% for infrastructure components

## Cumulative System Verification

Building on previous implementations:

### Database Integration

- [ ] API can successfully connect to database
- [ ] Repository pattern works through API controllers
- [ ] Database migrations run successfully in API context

### Authentication Integration

- [ ] Authentication middleware properly integrated
- [ ] JWT tokens validated in API pipeline
- [ ] Authorization policies enforced in middleware

### End-to-End System Tests

- [ ] Complete request flow: Auth → Validation → Business Logic → Response
- [ ] Error scenarios handled gracefully across all layers
- [ ] Performance remains acceptable with full middleware stack

## Success Criteria

- [ ] Complete API infrastructure operational
- [ ] All middleware components working together seamlessly
- [ ] Comprehensive error handling and logging implemented
- [ ] API documentation accessible and accurate
- [ ] Health monitoring and observability features functional
- [ ] Performance meets requirements (< 100ms overhead)
- [ ] All verification tests pass
- [ ] Security review completed for middleware pipeline
- [ ] Code review approved
- [ ] Documentation updated with API standards and conventions
- [ ] Integration with previous components (database, auth) verified
- [ ] System ready for business logic implementation
