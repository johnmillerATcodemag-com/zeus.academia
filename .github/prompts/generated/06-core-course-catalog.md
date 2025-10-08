# Implementation Prompt: Course Catalog and Subject Management

## Context & Overview

Implement comprehensive course catalog management including course definitions, prerequisites, subject areas, credit systems, and course lifecycle management. This system provides the academic foundation for curriculum management and student course selection.

## Prerequisites

- Database schema implementation completed (01-foundation-database-schema.md)
- Authentication system implemented (02-foundation-authentication.md)
- API infrastructure implemented (03-foundation-api-infrastructure.md)
- Student management system implemented (04-core-student-management.md)
- Faculty management system implemented (05-core-faculty-management.md)
- Understanding of academic course structures and curriculum design

## Implementation Tasks

### Task 1: Course and Subject Entity Extensions

**Task Description**: Extend course and subject entities with comprehensive academic information.

**Technical Requirements**:

- Extend Subject entity with detailed subject information and hierarchy
- Create Course entity with prerequisites, corequisites, and restrictions
- Add course numbering system and level classification
- Implement credit hour system with various credit types
- Add course status management (Active, Inactive, Under Review, Retired)

**Acceptance Criteria**:

- [ ] Subject entity with hierarchical categorization
- [ ] Course entity with comprehensive prerequisite system
- [ ] Course numbering follows institutional standards
- [ ] Credit system supports multiple credit types
- [ ] Course lifecycle status properly managed

### Task 2: Course Catalog Management

**Task Description**: Create comprehensive course catalog management system.

**Technical Requirements**:

- Implement course catalog CRUD operations
- Add course catalog versioning for academic years
- Create course approval workflow system
- Add course description and learning outcome management
- Implement catalog publication and distribution system

**Acceptance Criteria**:

- [ ] Complete course catalog management system
- [ ] Academic year-based catalog versioning
- [ ] Course approval workflow with multiple approval stages
- [ ] Learning outcomes properly documented
- [ ] Catalog publication system operational

### Task 3: Prerequisite and Restriction Management

**Task Description**: Implement complex prerequisite chains and course restrictions.

**Technical Requirements**:

- Create prerequisite management with AND/OR logic
- Implement corequisite requirements
- Add enrollment restrictions (major, class level, permission)
- Create prerequisite validation service
- Add prerequisite override and waiver system

**Acceptance Criteria**:

- [ ] Complex prerequisite logic properly implemented
- [ ] Corequisite requirements enforced
- [ ] Multiple restriction types supported
- [ ] Prerequisite validation working in enrollment
- [ ] Override system with proper authorization

### Task 4: Course Search and Discovery

**Task Description**: Implement comprehensive course search and discovery features.

**Technical Requirements**:

- Create advanced course search with multiple criteria
- Implement course filtering by subject, level, credits, requirements
- Add course recommendation system based on student profile
- Create course comparison functionality
- Add course waitlist and notification system

**Acceptance Criteria**:

- [ ] Advanced search with multiple filter combinations
- [ ] Course filtering performs efficiently
- [ ] Recommendation engine suggests relevant courses
- [ ] Course comparison feature working
- [ ] Waitlist system with automated notifications

### Task 5: Course Planning and Degree Requirements

**Task Description**: Implement course planning tools and degree requirement tracking.

**Technical Requirements**:

- Create degree requirement templates
- Implement course sequence planning
- Add degree audit functionality
- Create graduation requirement checking
- Add course plan validation and optimization

**Acceptance Criteria**:

- [ ] Degree requirement templates configurable
- [ ] Course sequence planning with semester mapping
- [ ] Degree audit shows progress toward requirements
- [ ] Graduation requirement validation complete
- [ ] Course plan optimization suggests efficient paths

### Task 6: Course API Controllers

**Task Description**: Create RESTful API endpoints for course catalog operations.

**Technical Requirements**:

- Create `CoursesController` with comprehensive CRUD operations
- Implement course search and filtering endpoints
- Add prerequisite validation API endpoints
- Create course planning and recommendation endpoints
- Add degree requirement and audit endpoints

**Acceptance Criteria**:

- [ ] Complete course catalog API
- [ ] Advanced search and filtering endpoints
- [ ] Prerequisite validation API working
- [ ] Course planning API endpoints
- [ ] Degree audit API functionality

### Task 7: Course Services and Business Logic

**Task Description**: Implement course-specific services and complex business logic.

**Technical Requirements**:

- Create `ICourseService` with comprehensive course operations
- Implement course equivalency management
- Add transfer credit evaluation service
- Create course capacity and availability management
- Add course analytics and reporting services

**Acceptance Criteria**:

- [ ] Course service handles all business operations
- [ ] Course equivalency system working
- [ ] Transfer credit evaluation automated
- [ ] Capacity management with waitlist integration
- [ ] Course analytics providing actionable insights

## Verification Steps

### Component-Level Verification

1. **Course Service Tests**

   ```csharp
   [Test]
   public async Task ValidatePrerequisites_Should_Check_Complex_Requirements()
   {
       // Test prerequisite validation with AND/OR logic
   }

   [Test]
   public async Task RecommendCourses_Should_Return_Relevant_Suggestions()
   {
       // Test course recommendation algorithm
   }

   [Test]
   public async Task CheckDegreeProgress_Should_Calculate_Accurate_Completion()
   {
       // Test degree audit calculations
   }
   ```

2. **Course API Tests**

   ```csharp
   [Test]
   public async Task SearchCourses_Should_Handle_Complex_Filters()
   {
       // Test advanced course search functionality
   }

   [Test]
   public async Task GetCoursesBySubject_Should_Return_Hierarchical_Results()
   {
       // Test subject-based course filtering
   }
   ```

3. **Prerequisite Logic Tests**
   ```csharp
   [Test]
   public void ValidatePrerequisiteChain_Should_Handle_Circular_Dependencies()
   {
       // Test prerequisite chain validation
   }
   ```

### Integration Testing

1. **Course-Student Integration**

   - Course recommendations based on student academic history
   - Prerequisite validation during course registration
   - Degree progress tracking with completed courses

2. **Course-Faculty Integration**

   - Faculty course assignment based on expertise
   - Course approval workflow with faculty input
   - Course evaluation and improvement tracking

3. **Catalog Management Flow**
   - Course creation → Approval → Catalog inclusion → Publication
   - Academic year transitions with catalog versioning
   - Course retirement and replacement workflows

### Performance Testing

1. **Search and Discovery Performance**

   - Course search with complex criteria < 250ms
   - Large catalog browsing remains responsive
   - Recommendation engine performs efficiently

2. **Prerequisite Validation Performance**
   - Complex prerequisite chains validate quickly
   - Bulk prerequisite checking for course planning
   - Degree audit calculations complete efficiently

## Code Quality Standards

- [ ] Course business logic follows academic standards
- [ ] Complex prerequisite relationships properly modeled
- [ ] Academic integrity maintained in all operations
- [ ] Performance optimized for large course catalogs
- [ ] Audit trails for all catalog changes
- [ ] Code coverage >90% for course management components

## Cumulative System Verification

Building on all previous implementations:

### Database Integration

- [ ] Course-Subject relationships working correctly
- [ ] Course-Academic relationships (prerequisites) maintain integrity
- [ ] Complex course hierarchies perform well

### Student Management Integration

- [ ] Students can search and discover appropriate courses
- [ ] Course recommendations align with student academic profiles
- [ ] Degree progress tracking accurate with completed courses

### Faculty Management Integration

- [ ] Faculty expertise linked to course subject areas
- [ ] Course approval workflows include appropriate faculty
- [ ] Teaching assignments consider faculty qualifications

### Authentication and Authorization

- [ ] Course access based on student enrollment status
- [ ] Faculty can modify courses they're authorized to teach
- [ ] Administrative access for catalog management

### API Infrastructure Integration

- [ ] Course endpoints follow established patterns
- [ ] Complex search queries perform efficiently
- [ ] Validation rules enforce academic policies

### Data Integrity and Business Rules

- [ ] Prerequisite chains prevent circular dependencies
- [ ] Course numbering follows institutional standards
- [ ] Credit hour calculations remain consistent
- [ ] Academic year transitions maintain data integrity

## Success Criteria

- [ ] Complete course catalog system operational
- [ ] Course search and discovery meeting user expectations
- [ ] Prerequisite system handles complex academic requirements
- [ ] Degree planning and audit functionality working
- [ ] Course recommendation engine providing value
- [ ] Catalog management workflow efficient for administrators
- [ ] Integration with student and faculty systems seamless
- [ ] Performance targets met (search < 250ms, complex validations < 500ms)
- [ ] All verification tests pass
- [ ] Academic policy compliance verified
- [ ] Data migration from existing systems completed
- [ ] Code review and academic review completed
- [ ] Documentation updated with course catalog features
- [ ] System ready for scheduling and enrollment implementation
