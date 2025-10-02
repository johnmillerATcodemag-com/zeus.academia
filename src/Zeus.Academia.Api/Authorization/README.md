# Custom Authorization Attributes

This directory contains custom authorization attributes designed to provide fine-grained access control for the Zeus Academia System. These attributes address the **Task 5 Critical Issue** identified in the success criteria verification.

## Overview

The custom authorization attributes provide comprehensive access control beyond basic ASP.NET Core Identity roles, implementing department-specific access, course-level permissions, administrative hierarchies, resource-based authorization, and role inheritance.

## Implemented Attributes

### 1. DepartmentAccessAttribute
**File**: `DepartmentAccessAttribute.cs`

**Purpose**: Provides department-specific access control for users based on department membership.

**Features**:
- Flexible parameter detection (route, query, body)
- Administrative bypass options
- Department ID validation
- Integration with IUserService

**Usage**:
```csharp
[DepartmentAccess] // Auto-detects departmentId parameter
[DepartmentAccess("deptId")] // Custom parameter name
[DepartmentAccess(AllowSystemAdminBypass = false)] // Strict department checking
```

### 2. CourseAccessAttribute
**File**: `CourseAccessAttribute.cs`

**Purpose**: Controls access to course-related operations with configurable access levels.

**Features**:
- CourseAccessLevel enum (Read, Write, Admin)
- Student enrollment checking framework
- Faculty teaching validation
- Parameter flexibility

**Usage**:
```csharp
[CourseAccess] // Read access by default
[CourseAccess(CourseAccessLevel.Write)] // Write access required
[CourseAccess(CourseAccessLevel.Admin)] // Administrative access
```

### 3. AdminOnlyAttribute
**File**: `AdminOnlyAttribute.cs`

**Purpose**: Restricts access to administrative functions with configurable minimum role levels.

**Features**:
- AdminLevel enum (Chair, Administrator, SystemAdmin)
- Active account validation
- Role hierarchy enforcement
- Convenience attributes included

**Convenience Attributes**:
- `SystemAdminOnlyAttribute`
- `AdministratorOnlyAttribute` 
- `ChairOrHigherAttribute`

**Usage**:
```csharp
[AdminOnly] // Chair or higher required
[AdminOnly(AdminLevel.Administrator)] // Administrator or higher
[SystemAdminOnly] // System admin only
```

### 4. ResourceBasedAuthorizationAttribute
**File**: `ResourceBasedAuthorizationAttribute.cs`

**Purpose**: Implements resource-based authorization for sensitive operations requiring resource ownership checks.

**Features**:
- ResourceType enum (UserProfile, AcademicRecord, Grade, Assignment, Course)
- ResourcePermission enum (Read, Write, Delete)
- Resource ownership validation
- Administrative bypass options
- Department chair access controls

**Usage**:
```csharp
[ResourceBasedAuthorization(ResourceType.UserProfile)]
[ResourceBasedAuthorization(ResourceType.Grade, ResourcePermission.Write)]
[ResourceBasedAuthorization(ResourceType.AcademicRecord, ResourcePermission.Read, AllowDepartmentChairAccess = true)]
```

### 5. RoleHierarchyAttribute
**File**: `RoleHierarchyAttribute.cs`

**Purpose**: Enforces role hierarchy relationships with flexible inheritance and access control.

**Features**:
- Role inheritance validation
- Exact role matching options
- Additional allowed/denied roles
- Department matching requirements
- Comprehensive convenience attributes

**Convenience Attributes**:
- `StudentOnlyAttribute`
- `FacultyOnlyAttribute`
- `ProfessorOrHigherAttribute`
- `TeachingProfOrHigherAttribute`
- `AcademicStaffOnlyAttribute`

**Usage**:
```csharp
[RoleHierarchy(AcademicRoleType.Professor)] // Professor or higher
[RoleHierarchy(AcademicRoleType.Chair, false)] // Exact role only
[StudentOnly] // Students only
[FacultyOnly] // Faculty members only
```

## Architecture

All attributes implement both:
1. **IAuthorizationFilter** for direct attribute usage
2. **IAuthorizationRequirement/AuthorizationHandler** for policy-based authorization

This dual approach provides maximum flexibility for different authorization scenarios.

## Integration

### Required Services
All attributes depend on:
- `IUserService` - User management and role checking
- `IRoleHierarchyService` - Role hierarchy validation

### ASP.NET Core Integration
Attributes integrate seamlessly with:
- ASP.NET Core Identity system
- Existing role management
- Claims-based authentication
- Policy-based authorization

## Security Features

### Common Security Patterns
- Active account validation
- Null reference protection
- Exception handling with fail-safe defaults
- Parameter validation and sanitization

### Administrative Bypasses
Configurable bypass options for:
- System administrators
- Administrators  
- Department chairs (context-dependent)

### Role Hierarchy Integration
- Leverages existing `AcademicRoleType` system
- Supports role inheritance patterns
- Flexible permission escalation

## Usage Examples

### Controller Method Protection
```csharp
[HttpGet("profile/{userId}")]
[ResourceBasedAuthorization(ResourceType.UserProfile, ResourcePermission.Read)]
public async Task<IActionResult> GetUserProfile(int userId)
{
    // Only resource owner or authorized roles can access
}

[HttpPost("grades")]
[AdminOnly(AdminLevel.Administrator)]
[DepartmentAccess("studentDepartmentId")]
public async Task<IActionResult> CreateGrade(GradeCreateRequest request)
{
    // Only administrators in the student's department can create grades
}

[HttpGet("courses/{courseId}/assignments")]
[CourseAccess(CourseAccessLevel.Read)]
[RoleHierarchy(AcademicRoleType.Student, AdditionalAllowedRoles = new[] { AcademicRoleType.Professor })]
public async Task<IActionResult> GetCourseAssignments(int courseId)
{
    // Students enrolled in course or professors can view assignments
}
```

### Policy-Based Usage
```csharp
// In Startup.cs or Program.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("DepartmentAdmin", policy =>
        policy.Requirements.Add(new RoleHierarchyRequirement(AcademicRoleType.Chair)));
        
    options.AddPolicy("ResourceOwner", policy =>
        policy.Requirements.Add(new ResourceBasedRequirement(ResourceType.UserProfile, 123)));
});
```

## Success Criteria Compliance

These custom authorization attributes directly address **Task 5** success criteria:
- ✅ **Fine-grained authorization attributes** for department access, course access, and administrative functions
- ✅ **Department-specific access control** with flexible bypass options
- ✅ **Course-level permissions** with enrollment and teaching validation
- ✅ **Administrative hierarchy enforcement** with role-based restrictions
- ✅ **Resource-based authorization** for sensitive operations
- ✅ **Role inheritance and hierarchy** validation with convenience attributes

## Technical Implementation Notes

### Performance Considerations
- Asynchronous authorization checks
- Efficient parameter extraction
- Minimal database queries through service layer

### Extensibility
- Attribute inheritance for customization
- Enum-based configurations for flexibility
- Service injection for dependency management

### Testing Integration
All attributes are designed for:
- Unit testing with mocked services
- Integration testing with test databases
- End-to-end testing with real scenarios

## Future Enhancements

### Planned Improvements
1. **Caching Layer**: Add authorization result caching for performance
2. **Audit Logging**: Integrate authorization decisions with audit system
3. **Dynamic Permissions**: Support for runtime permission changes
4. **Course Integration**: Complete course/enrollment validation
5. **Advanced Workflows**: Support for complex approval workflows

### Configuration Options
Future versions may include:
- Configuration-driven bypass rules
- Dynamic role hierarchy definitions
- External authorization provider integration
- Custom permission validators

---

*This authorization system provides enterprise-level access control while maintaining simplicity and flexibility for the Zeus Academia System.*