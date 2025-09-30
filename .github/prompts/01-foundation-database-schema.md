# Implementation Prompt: Database Schema and Entity Framework Setup

## Context & Overview

Implement the foundational database schema for the Zeus Academia System based on the existing ORM model. This includes setting up Entity Framework Core, creating entities, configuring relationships, and implementing database migrations.

## Prerequisites

- .NET 8.0 SDK installed
- SQL Server or SQL Server LocalDB available
- Zeus.Academia.Infrastructure project structure created

## Implementation Tasks

### Task 1: Entity Framework Core Setup

**Task Description**: Configure Entity Framework Core in the Infrastructure project with SQL Server provider.

**Technical Requirements**:

- Install Entity Framework Core packages: `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.EntityFrameworkCore.Design`
- Create `AcademiaDbContext` class
- Configure connection string management
- Set up dependency injection registration

**Acceptance Criteria**:

- [ ] EF Core packages installed and referenced
- [ ] `AcademiaDbContext` class created with proper configuration
- [ ] Connection string configurable via appsettings.json
- [ ] DbContext registered in DI container

### Task 2: Core Entity Models

**Task Description**: Create entity models based on the ORM schema for core academic entities.

**Technical Requirements**:

- Create entity classes for: Academic, Professor, Teacher, Chair, TeachingProf, Student
- Implement proper inheritance hierarchy (Academic as base class)
- Add data annotations and fluent API configurations
- Include audit fields (CreatedDate, ModifiedDate, CreatedBy, ModifiedBy)

**Acceptance Criteria**:

- [ ] All core entity classes created with proper properties
- [ ] Inheritance relationships properly configured
- [ ] Primary keys and reference modes implemented
- [ ] Navigation properties defined for relationships
- [ ] Audit fields included in base entity class

### Task 3: Academic Structure Entities

**Task Description**: Implement entities for academic structure and administration.

**Technical Requirements**:

- Create entities for: Department, Subject, Degree, University, Rank
- Implement proper relationships and foreign keys
- Add validation attributes for required fields
- Configure entity relationships using Fluent API

**Acceptance Criteria**:

- [ ] Department entity with proper relationships
- [ ] Subject entity for course subjects
- [ ] Degree entity for academic degrees
- [ ] University entity for institution management
- [ ] Rank entity for academic rankings
- [ ] All relationships properly configured

### Task 4: Infrastructure Entities

**Task Description**: Create supporting entities for system infrastructure.

**Technical Requirements**:

- Create entities for: Building, Room, Extension, AccessLevel
- Implement location and facility management
- Add system access and permission entities
- Configure composite keys where appropriate

**Acceptance Criteria**:

- [ ] Building and Room entities for facility management
- [ ] Extension entity for contact information
- [ ] AccessLevel entity for permissions
- [ ] Proper indexing for performance
- [ ] Composite keys configured correctly

### Task 5: Database Migrations

**Task Description**: Create and test initial database migration.

**Technical Requirements**:

- Generate initial migration using EF Core tools
- Review and optimize generated SQL
- Create seed data for lookup tables
- Implement database initialization strategy

**Acceptance Criteria**:

- [ ] Initial migration created successfully
- [ ] Migration generates correct database schema
- [ ] Seed data included for reference tables
- [ ] Database can be created and updated from migration
- [ ] Migration scripts are idempotent

### Task 6: Repository Pattern Implementation

**Task Description**: Implement repository pattern for data access.

**Technical Requirements**:

- Create generic `IRepository<T>` interface
- Implement `Repository<T>` base class
- Create specific repository interfaces for main entities
- Implement Unit of Work pattern
- Add async/await support throughout

**Acceptance Criteria**:

- [ ] Generic repository interface and implementation
- [ ] Specific repositories for Academic, Department, Subject entities
- [ ] Unit of Work pattern implemented
- [ ] All repository methods support async operations
- [ ] Proper exception handling implemented

## Verification Steps

### Component-Level Verification

1. **Entity Validation Tests**

   ```csharp
   [Test]
   public void Academic_Should_Have_Required_Properties()
   {
       // Verify entity has empNr, name, and audit fields
   }
   ```

2. **Database Connection Test**

   ```csharp
   [Test]
   public async Task DbContext_Should_Connect_Successfully()
   {
       // Verify database connection and context creation
   }
   ```

3. **Migration Test**

   ```csharp
   [Test]
   public void Migration_Should_Create_All_Tables()
   {
       // Verify all expected tables are created
   }
   ```

4. **Repository Tests**
   ```csharp
   [Test]
   public async Task Repository_Should_Perform_CRUD_Operations()
   {
       // Test Create, Read, Update, Delete operations
   }
   ```

### Integration Testing

1. **Database Schema Validation**

   - Run migration against clean database
   - Verify all tables, indexes, and constraints created
   - Check foreign key relationships are correct
   - Validate seed data is inserted properly

2. **Performance Testing**
   - Test query performance with sample data
   - Verify indexes are being used effectively
   - Check connection pooling is working

## Code Quality Standards

- [ ] All code follows C# coding conventions
- [ ] Unit tests achieve >90% code coverage
- [ ] All public APIs have XML documentation
- [ ] Database operations use async/await patterns
- [ ] Proper error handling and logging implemented
- [ ] Security considerations for data access implemented

## Integration with Existing System

This foundational implementation provides the data layer for all subsequent Academia System components. Future implementations will depend on:

- Entity models defined here
- Repository interfaces for data access
- DbContext for database operations
- Migration strategy for schema changes

## Success Criteria

- [ ] Database schema accurately reflects ORM model
- [ ] All entity relationships properly configured
- [ ] Migration system operational and tested
- [ ] Repository pattern provides clean data access layer
- [ ] Performance meets baseline requirements (queries < 100ms)
- [ ] All verification tests pass
- [ ] Code review completed and approved
- [ ] Documentation updated
