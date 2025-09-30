# Implementation Prompt: Authentication and Authorization System

## Context & Overview

Implement a comprehensive authentication and authorization system for the Zeus Academia System. This includes user management, role-based access control, JWT token handling, and integration with the Academic entity hierarchy.

## Prerequisites

- Database schema implementation completed (01-foundation-database-schema.md)
- Zeus.Academia.Api project structure created
- Understanding of ASP.NET Core Identity and JWT authentication

## Implementation Tasks

### Task 1: ASP.NET Core Identity Setup

**Task Description**: Configure ASP.NET Core Identity with custom user entities integrated with the Academic hierarchy.

**Technical Requirements**:

- Install Identity packages: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.AspNetCore.Authentication.JwtBearer`
- Create `AcademiaUser` class inheriting from `IdentityUser`
- Link `AcademiaUser` to `Academic` entity
- Configure Identity in `AcademiaDbContext`
- Set up password policies and user requirements

**Acceptance Criteria**:

- [ ] Identity packages installed and configured
- [ ] `AcademiaUser` entity created and linked to Academic
- [ ] Identity DbContext properly configured
- [ ] Password policies meet security requirements
- [ ] User lockout and security settings configured

### Task 2: Role-Based Authorization

**Task Description**: Implement role-based access control with academic-specific roles.

**Technical Requirements**:

- Define roles: Student, Professor, Chair, TeachingProf, Administrator, SystemAdmin
- Create role hierarchy and permissions
- Implement `IAuthorizationService` extensions
- Create custom authorization policies
- Add role-based middleware for API endpoints

**Acceptance Criteria**:

- [ ] All academic roles defined and seeded
- [ ] Role hierarchy properly implemented
- [ ] Authorization policies created for different access levels
- [ ] Custom authorization handlers implemented
- [ ] Role assignment functionality working

### Task 3: JWT Token Authentication

**Task Description**: Implement JWT token-based authentication for API access.

**Technical Requirements**:

- Configure JWT authentication middleware
- Create token generation service
- Implement refresh token mechanism
- Add token validation and expiration handling
- Create secure token signing keys

**Acceptance Criteria**:

- [ ] JWT middleware configured in API project
- [ ] Token generation service implemented
- [ ] Refresh token functionality working
- [ ] Token expiration and renewal handled
- [ ] Secure key management implemented

### Task 4: User Management Services

**Task Description**: Create services for user registration, authentication, and profile management.

**Technical Requirements**:

- Create `IUserService` interface and implementation
- Implement user registration with academic role assignment
- Add password reset and email confirmation
- Create user profile management endpoints
- Implement user search and administration functions

**Acceptance Criteria**:

- [ ] User service interface and implementation created
- [ ] Registration process includes role assignment
- [ ] Password reset via email implemented
- [ ] User profile CRUD operations working
- [ ] Admin user management functions available

### Task 5: Authorization Policies and Attributes

**Task Description**: Create custom authorization policies and attributes for fine-grained access control.

**Technical Requirements**:

- Create `DepartmentAccessPolicy` for department-specific access
- Implement `CourseAccessPolicy` for course-related operations
- Add `AdminOnlyPolicy` for administrative functions
- Create custom authorization attributes
- Implement resource-based authorization where needed

**Acceptance Criteria**:

- [ ] Department-based access policies implemented
- [ ] Course access policies working
- [ ] Administrative policies configured
- [ ] Custom authorization attributes created
- [ ] Resource-based authorization for sensitive operations

### Task 6: Security Middleware and Logging

**Task Description**: Implement security middleware, audit logging, and attack prevention.

**Technical Requirements**:

- Add security headers middleware
- Implement request/response logging for security events
- Add rate limiting for authentication endpoints
- Create audit trail for user actions
- Implement CORS policies for frontend access

**Acceptance Criteria**:

- [ ] Security headers properly configured
- [ ] Authentication events logged and audited
- [ ] Rate limiting prevents brute force attacks
- [ ] User action audit trail implemented
- [ ] CORS policies configured for client applications

## Verification Steps

### Component-Level Verification

1. **Authentication Tests**

   ```csharp
   [Test]
   public async Task Login_With_Valid_Credentials_Should_Return_Token()
   {
       // Test successful authentication returns JWT token
   }

   [Test]
   public async Task Login_With_Invalid_Credentials_Should_Fail()
   {
       // Test failed authentication is handled properly
   }
   ```

2. **Authorization Tests**

   ```csharp
   [Test]
   public async Task Professor_Should_Access_Own_Courses()
   {
       // Test role-based access to resources
   }

   [Test]
   public async Task Student_Should_Not_Access_Admin_Functions()
   {
       // Test access denial for unauthorized operations
   }
   ```

3. **Token Tests**

   ```csharp
   [Test]
   public void JWT_Token_Should_Contain_Valid_Claims()
   {
       // Verify token contains correct user and role claims
   }

   [Test]
   public async Task Refresh_Token_Should_Generate_New_JWT()
   {
       // Test token refresh mechanism
   }
   ```

### Security Testing

1. **Password Security**

   - Test password complexity requirements
   - Verify password hashing is secure
   - Test account lockout after failed attempts

2. **Token Security**

   - Verify tokens expire appropriately
   - Test token tampering detection
   - Validate refresh token rotation

3. **Authorization Testing**
   - Test horizontal privilege escalation prevention
   - Verify vertical privilege escalation blocked
   - Test resource access controls

### Integration Testing

1. **End-to-End Authentication Flow**

   - User registration → email confirmation → login → token generation
   - Password reset flow with email verification
   - User profile updates with proper authorization

2. **Cross-System Integration**
   - Verify Academic entities linked to user accounts
   - Test department-based access controls
   - Validate role assignments work across all modules

## Code Quality Standards

- [ ] All authentication code follows OWASP security guidelines
- [ ] Sensitive data (passwords, tokens) properly protected
- [ ] Security tests achieve >95% coverage for auth components
- [ ] All security events properly logged
- [ ] Error messages don't leak sensitive information
- [ ] Rate limiting and anti-abuse measures implemented

## Cumulative System Verification

Since this builds on the database foundation:

### Regression Testing

- [ ] Database schema still creates successfully
- [ ] All existing entity relationships remain intact
- [ ] Repository pattern continues to work with Identity integration
- [ ] Migration system handles Identity tables correctly

### Data Integrity

- [ ] Academic-User relationships maintain referential integrity
- [ ] Role assignments correctly reflect academic hierarchy
- [ ] User deletion properly handles cascading relationships

## Success Criteria

- [ ] Complete authentication system operational
- [ ] Role-based authorization working for all user types
- [ ] JWT token authentication secure and performant
- [ ] User management functions available to administrators
- [ ] Security policies prevent common authentication vulnerabilities
- [ ] All verification tests pass
- [ ] Security audit completed
- [ ] Performance meets requirements (auth operations < 200ms)
- [ ] Integration with database layer seamless
- [ ] Code review and security review completed
- [ ] Documentation updated with security considerations
