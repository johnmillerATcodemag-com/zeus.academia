# Implementation Prompt: Student Management System

## Context & Overview

Implement comprehensive student management functionality including student enrollment, profile management, academic records, and student-specific operations. This builds upon the authentication system and provides core student lifecycle management.

## Prerequisites

- Database schema implementation completed (01-foundation-database-schema.md)
- Authentication system implemented (02-foundation-authentication.md)
- API infrastructure implemented (03-foundation-api-infrastructure.md)
- Understanding of student academic lifecycle

## Implementation Tasks

### Task 1: Student Entity Extensions and Services

**Task Description**: Extend the Student entity and create comprehensive student management services.

**Technical Requirements**:

- Extend Student entity with enrollment status, academic standing, GPA tracking
- Create `IStudentService` interface and implementation
- Add student-specific business logic and validation rules
- Implement student search and filtering capabilities
- Add student academic status management

**Acceptance Criteria**:

- [ ] Student entity extended with enrollment and academic fields
- [ ] Student service handles all student lifecycle operations
- [ ] Business rules enforced (enrollment status, academic standing)
- [ ] Search functionality with multiple criteria (name, ID, department, status)
- [ ] Academic status transitions properly managed

### Task 2: Student Enrollment Management

**Task Description**: Implement student enrollment processes including application, admission, and registration.

**Technical Requirements**:

- Create enrollment application workflow
- Implement admission decision tracking
- Add enrollment status management (Applied, Admitted, Enrolled, Graduated, Withdrawn)
- Create enrollment history tracking
- Add enrollment date and term management

**Acceptance Criteria**:

- [ ] Enrollment application process implemented
- [ ] Admission workflow with approval/rejection handling
- [ ] Enrollment status properly tracked and managed
- [ ] Historical enrollment records maintained
- [ ] Term-based enrollment supported

### Task 3: Student Profile Management

**Task Description**: Create comprehensive student profile management with personal and academic information.

**Technical Requirements**:

- Implement student profile CRUD operations
- Add emergency contact management
- Include academic advisor assignment
- Add student photo and document upload
- Implement profile completion tracking

**Acceptance Criteria**:

- [ ] Complete student profile management available
- [ ] Emergency contacts can be added/updated
- [ ] Academic advisor assignment functionality
- [ ] Document upload and management working
- [ ] Profile completion percentage tracked

### Task 4: Academic Record Management

**Task Description**: Implement student academic record tracking including courses, grades, and transcripts.

**Technical Requirements**:

- Create student course enrollment tracking
- Implement grade recording and GPA calculation
- Add transcript generation functionality
- Include academic honors and awards tracking
- Add degree progress tracking

**Acceptance Criteria**:

- [ ] Course enrollment history maintained
- [ ] Grade recording with GPA calculation
- [ ] Official transcript generation available
- [ ] Academic honors/awards properly tracked
- [ ] Degree progress monitoring implemented

### Task 5: Student API Controllers

**Task Description**: Create RESTful API endpoints for all student management operations.

**Technical Requirements**:

- Create `StudentsController` with full CRUD operations
- Implement student search and filtering endpoints
- Add student enrollment management endpoints
- Create student academic record endpoints
- Add file upload endpoints for documents/photos

**Acceptance Criteria**:

- [ ] RESTful student API with all CRUD operations
- [ ] Advanced search and filtering endpoints
- [ ] Enrollment management API endpoints
- [ ] Academic record access API
- [ ] Document upload API with proper validation

### Task 6: Student Request/Response Models and Validation

**Task Description**: Create comprehensive Request/Response models for student operations with robust validation following modern ASP.NET Core patterns.

**Technical Requirements**:

- Create student request models: `CreateStudentRequest`, `UpdateStudentRequest`
- Create student response models: `StudentSummaryResponse`, `StudentDetailResponse`
- Implement comprehensive validation rules using FluentValidation
- Add custom validators for academic business rules (GPA ranges, enrollment status, date validations)
- Create AutoMapper profiles for entity-to-response model mapping
- Implement paginated response models for efficient student data transfer

**Acceptance Criteria**:

- [x] Complete set of student Request/Response models created
- [x] Comprehensive validation rules implemented with FluentValidation
- [x] Custom academic validators working (email uniqueness, phone format, date ranges)
- [x] AutoMapper profiles configured for Student entity mapping
- [x] Pagination models integrated with API responses

### Task 7: Student Notification System

**Task Description**: Implement notification system for student-related events and communications.

**Technical Requirements**:

- Create student notification service
- Implement email notifications for enrollment events
- Add SMS notifications for urgent communications
- Create in-app notification system
- Add notification preferences management

**Acceptance Criteria**:

- [ ] Student notification service operational
- [ ] Email notifications for key events (enrollment, grade posting)
- [ ] SMS notification capability
- [ ] In-app notification system
- [ ] Student notification preferences configurable

## Verification Steps

### Component-Level Verification

1. **Student Service Tests**

   ```csharp
   [Test]
   public async Task CreateStudent_Should_Create_Valid_Student()
   {
       // Test student creation with validation
   }

   [Test]
   public async Task EnrollStudent_Should_Update_Status_And_Send_Notification()
   {
       // Test enrollment process with status update and notification
   }

   [Test]
   public async Task CalculateGPA_Should_Return_Correct_Value()
   {
       // Test GPA calculation logic
   }
   ```

2. **Student API Tests**

   ```csharp
   [Test]
   public async Task GetStudents_Should_Return_Paginated_Results()
   {
       // Test student listing with pagination
   }

   [Test]
   public async Task SearchStudents_Should_Filter_By_Criteria()
   {
       // Test student search functionality
   }
   ```

3. **Validation Tests**
   ```csharp
   [Test]
   public void StudentCreateDto_Should_Require_Essential_Fields()
   {
       // Test validation rules for student creation
   }
   ```

### Integration Testing

1. **End-to-End Student Lifecycle**

   - Application → Admission → Enrollment → Course Registration → Grade Recording → Graduation
   - Student profile updates with notifications
   - Academic standing calculations and status changes

2. **Cross-System Integration**
   - Student authentication with academic records
   - Student-course relationship management
   - Student-advisor assignment integration

### Performance Testing

1. **Student Search Performance**

   - Search with various criteria performs < 200ms
   - Large dataset handling (10,000+ students)
   - Pagination efficiency testing

2. **Batch Operations**
   - Bulk student import performance
   - Mass notification sending efficiency
   - Report generation performance

## Code Quality Standards

- [ ] All student services follow domain-driven design principles
- [ ] Student data properly validated and sanitized
- [ ] Privacy and FERPA compliance considerations implemented
- [ ] Audit trails for all student record changes
- [ ] Performance optimized for large student populations
- [ ] Code coverage >90% for student management components

## Cumulative System Verification

Building on foundation components:

### Database Integration

- [ ] Student entities properly integrated with Academic base class
- [ ] Repository patterns work correctly for student operations
- [ ] Database migrations handle student-specific schema changes

### Authentication Integration

- [ ] Student users can authenticate and access appropriate resources
- [ ] Student role-based authorization working correctly
- [ ] Student profile linked to authentication identity

### API Infrastructure Integration

- [ ] Student endpoints follow established API patterns
- [ ] Validation middleware works with student DTOs
- [ ] Error handling proper for student operations
- [ ] Logging captures student-related activities

### Data Integrity

- [ ] Student records maintain referential integrity
- [ ] Academic status transitions follow business rules
- [ ] Grade records properly linked to students and courses
- [ ] Enrollment history accurately maintained

## Success Criteria

- [ ] Complete student management system operational
- [ ] Student lifecycle fully supported (application to graduation)
- [ ] All student API endpoints functional and documented
- [ ] Student notifications working for key events
- [ ] Academic record management complete
- [ ] Search and reporting capabilities meet requirements
- [ ] Performance targets met (API responses < 200ms)
- [ ] All verification tests pass
- [ ] Privacy and compliance requirements satisfied
- [ ] Integration with existing foundation systems verified
- [ ] Code review and security review completed
- [ ] Documentation updated with student management features
- [ ] System ready for faculty and course management integration
