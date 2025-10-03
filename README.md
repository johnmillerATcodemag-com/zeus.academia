# Zeus Academia System

A comprehensive academic management system built with .NET 9, Entity Framework Core, and ASP.NET Core, providing complete functionality for managing academic institutions.

## Overview

Zeus Academia is an enterprise-grade academic management system that handles all aspects of university operations including faculty management, student enrollment, course catalog management, academic records, and institutional infrastructure.

## Implemented Features

### ✅ Prompt 6 - Course and Subject Entity Extensions (Task 1)

**Status: Completed** - Tagged as `prompt6-task1`

- **Course Management**: Complete course entity with prerequisites, corequisites, restrictions, and offerings
- **Subject Enhancement**: Extended subject entities with comprehensive validation and relationships
- **Credit System**: Flexible credit type management supporting various academic structures
- **Course History**: Status tracking and audit trails for course changes
- **Offering Management**: Course offering scheduling with instructor and facility assignments

**Key Entities:**

- `Course` - Core course information with metadata and relationships
- `CoursePrerequisite` - Prerequisite course management with flexible logic
- `CourseCorequisite` - Concurrent course requirements
- `CourseRestriction` - Enrollment restrictions and limitations
- `CreditType` - Various credit type classifications
- `CourseStatusHistory` - Audit trail for course changes
- `CourseOffering` - Scheduled course instances

### ✅ Prompt 6 - Course Catalog Management (Task 2)

**Status: Completed** - Tagged as `prompt6-task2`

- **Catalog Management**: Academic year-based course catalog creation and management
- **Versioning System**: Comprehensive version control with change tracking and comparison
- **Approval Workflows**: Multi-stage approval process (Department → Curriculum Committee → Academic Senate → Provost → Board)
- **Learning Outcomes**: Bloom's taxonomy integration with assessment alignment
- **Publication System**: Multiple format support (PDF, HTML, Mobile, EPUB) with distribution channels

**Key Entities:**

- `CourseCatalog` - Main catalog entity with versioning and publication management
- `CourseApprovalWorkflow` - Multi-stage approval process management
- `LearningOutcome` - Learning objectives with Bloom's taxonomy classification
- `CatalogPublication` - Publication management in multiple formats
- `CatalogVersion` - Version control with change tracking and comparison

**Key Features:**

- Academic year-based catalog organization
- Comprehensive approval workflow with role-based permissions
- Learning outcome assessment with rubric criteria
- Publication in multiple formats (PDF, HTML, Mobile, EPUB)
- Distribution through various channels (Website, Portal, Mobile, Print)
- Version comparison and change tracking
- Catalog metadata and analytics

### 🔄 Previously Implemented

#### Prompt 4 - Student Management System

- **Task 1**: Student Enrollment Management - Comprehensive enrollment application process
- **Task 2**: Student Profile Management - Complete student information management
- **Task 3**: Academic Record Management - Grade tracking and degree progress monitoring

#### Prompt 5 - Faculty Management System

- **Task 1**: Faculty Employment and Career Management - Employment history and promotions
- **Task 2**: Faculty Profile Management - Comprehensive faculty information system
- **Task 3**: Academic Rank and Promotion System - Tenure track and promotion workflows
- **Task 4**: Department Assignment and Administration - Administrative role management

## Architecture

### Technology Stack

- **.NET 9**: Latest framework with enhanced performance and features
- **Entity Framework Core**: Advanced ORM with comprehensive relationship mapping
- **ASP.NET Core**: High-performance web API framework
- **SQL Server**: Enterprise database with advanced indexing and constraints
- **Identity Framework**: Comprehensive authentication and authorization

### Database Design

- **Table Per Hierarchy (TPH)**: Efficient academic inheritance modeling
- **Comprehensive Relationships**: Full foreign key relationships with proper cascade behavior
- **Advanced Indexing**: Performance-optimized indexes for all query patterns
- **Constraint Validation**: Database-level data integrity enforcement
- **Audit Trails**: Complete change tracking and historical data preservation

### Key Design Patterns

- **Repository Pattern**: Clean data access abstraction
- **Configuration Pattern**: Centralized Entity Framework configuration
- **Enum-Driven Design**: Type-safe enumeration usage throughout
- **Comprehensive Validation**: Multi-layer validation (database, entity, API)

## Development Status

### Completed Tasks (6/6)

1. ✅ **Student Enrollment Management** - Full enrollment application workflow
2. ✅ **Student Profile Management** - Complete student information system
3. ✅ **Academic Record Management** - Grade and progress tracking
4. ✅ **Faculty Employment Management** - Career progression and employment history
5. ✅ **Course and Subject Extensions** - Enhanced course management system
6. ✅ **Course Catalog Management** - Comprehensive catalog and versioning system

### Implementation Highlights

#### Comprehensive Entity Modeling

- **70+ Entities**: Complete academic domain modeling
- **Complex Relationships**: Advanced many-to-many and hierarchical relationships
- **Inheritance Hierarchies**: Efficient academic role modeling
- **Audit Capabilities**: Complete change tracking across all entities

#### Advanced Course Catalog System

- **Version Control**: Git-like versioning for course catalogs
- **Approval Workflows**: Configurable multi-stage approval processes
- **Learning Outcomes**: Bloom's taxonomy integration with assessment mapping
- **Publication Pipeline**: Automated publication in multiple formats
- **Distribution Management**: Multi-channel distribution with analytics

#### Database Features

- **500+ Database Constraints**: Comprehensive data integrity
- **100+ Indexes**: Optimized query performance
- **JSON Support**: Flexible metadata and configuration storage
- **Computed Columns**: Derived data calculation
- **Check Constraints**: Business rule enforcement

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server 2022 or later
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
2. Update connection strings in `appsettings.json`
3. Run database migrations: `dotnet ef database update`
4. Build and run: `dotnet run --project src/Zeus.Academia.Api`

### Database Migration

```bash
# Generate migration
dotnet ef migrations add MigrationName --project src/Zeus.Academia.Infrastructure

# Update database
dotnet ef database update --project src/Zeus.Academia.Infrastructure
```

## API Features

### Authentication & Authorization

- **JWT Token Authentication**: Secure API access
- **Role-Based Authorization**: Granular permission control
- **Identity Integration**: Complete user management
- **Refresh Token Support**: Seamless token renewal

### RESTful API Design

- **OpenAPI/Swagger**: Complete API documentation
- **Versioned Endpoints**: API version management
- **Error Handling**: Comprehensive error responses
- **Validation**: Multi-layer input validation

## Testing

### Test Coverage

- **Unit Tests**: Entity validation and business logic
- **Integration Tests**: API endpoint testing
- **Database Tests**: Entity Framework functionality
- **Performance Tests**: Load and stress testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Contributing

### Development Workflow

1. Follow test-driven development (TDD) practices
2. Implement comprehensive entity validation
3. Create Entity Framework configurations
4. Generate and test database migrations
5. Update documentation and commit with descriptive tags

### Code Standards

- **Clean Architecture**: Separation of concerns
- **SOLID Principles**: Maintainable and extensible design
- **Comprehensive Documentation**: XML documentation for all public APIs
- **Performance Optimization**: Efficient query patterns and indexing

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

Built following enterprise-grade patterns and practices for academic institution management systems.
