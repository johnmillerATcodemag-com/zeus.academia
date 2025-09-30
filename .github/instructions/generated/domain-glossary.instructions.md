# Domain Glossary

## Purpose

This document provides a comprehensive terminology reference for the Academic Management System, establishing consistent vocabulary across academic domain concepts, technical implementation terms, CQRS architectural patterns, and system-specific definitions. This glossary serves as the authoritative source for understanding and communicating about system components, business rules, and technical architecture.

## Scope

This document covers:

- Academic domain terminology and business concepts
- Technical architecture terms and patterns
- CQRS and Domain-Driven Design terminology
- System-specific entities, value objects, and services
- Azure cloud platform terminology
- Development and operational terminology

This document does not cover:

- General software engineering terms unless system-specific
- Third-party vendor terminology unless directly integrated
- Historical or deprecated terminology
- User interface terminology and labels

## How to Use This Glossary

- Terms are organized alphabetically within categories
- Each term includes definition, context, and examples where applicable
- Cross-references link related concepts using → notation
- Code examples demonstrate implementation patterns
- Synonyms and alternative terms are noted in parentheses

## Academic Domain Terminology

### Academic Program Management

| Term                                  | Definition                                                                                           | Context                   | Example                                                                                                      |
| ------------------------------------- | ---------------------------------------------------------------------------------------------------- | ------------------------- | ------------------------------------------------------------------------------------------------------------ |
| **Academic Advisor**                  | Faculty or staff member assigned to guide students in academic planning and decision-making          | Student support services  | "Each student must meet with their Academic Advisor before enrollment periods to plan their course schedule" |
| **Academic Calendar**                 | Official schedule of academic terms, holidays, deadlines, and important dates                        | System-wide scheduling    | "The Academic Calendar defines registration periods, add/drop deadlines, and examination schedules"          |
| **Academic Probation**                | Temporary status assigned to students whose academic performance falls below acceptable standards    | Student status management | "Students with GPA below 2.0 are placed on Academic Probation and must meet with an advisor"                 |
| **Academic Standing**                 | Student's current academic status based on performance metrics and compliance with academic policies | Performance tracking      | "Academic Standing categories include Good Standing, Probation, Suspension, and Dismissal"                   |
| **Academic Term** (Semester, Quarter) | Fixed period during which courses are conducted and grades are recorded                              | Scheduling and enrollment | "Fall 2024 term runs from August 26 to December 16, 2024"                                                    |
| **Academic Transcript**               | Official record of student's academic history including courses, grades, and degree progress         | Academic records          | "The Academic Transcript shows all completed courses with final grades and cumulative GPA"                   |
| **Academic Year**                     | Complete cycle of academic terms, typically fall through summer                                      | Planning and reporting    | "The 2024-2025 Academic Year includes Fall 2024, Spring 2025, and Summer 2025 terms"                         |
| **Accreditation**                     | Formal recognition that an academic program meets quality standards                                  | Quality assurance         | "The Computer Science program maintains ABET accreditation requiring specific curriculum standards"          |

### Course and Curriculum Management

| Term                            | Definition                                                                                        | Context               | Example                                                                                                   |
| ------------------------------- | ------------------------------------------------------------------------------------------------- | --------------------- | --------------------------------------------------------------------------------------------------------- |
| **Audit**                       | Taking a course without receiving academic credit or grade                                        | Enrollment option     | "Students may audit courses to learn content without affecting their GPA"                                 |
| **Course Catalog**              | Comprehensive listing of all available courses with descriptions, prerequisites, and requirements | Academic planning     | "The Course Catalog contains detailed descriptions and prerequisite chains for all offered courses"       |
| **Course Code** (Course Number) | Standardized identifier for academic courses following institutional conventions                  | Course identification | "CS101, MATH201, ENGL102 follow department prefix and numbering standards"                                |
| **Course Credit Hours**         | Quantitative measure of academic work representing time commitment and learning outcomes          | Degree requirements   | "Most courses carry 3 credit hours, while labs typically add 1 additional credit hour"                    |
| **Course Prerequisites**        | Required prior coursework or competencies needed before enrollment                                | Academic progression  | "CS201 requires successful completion of CS101 with grade C or better"                                    |
| **Course Corequisites**         | Courses that must be taken simultaneously or have been completed previously                       | Curriculum design     | "Chemistry lab (CHEM101L) is a corequisite for Chemistry lecture (CHEM101)"                               |
| **Course Schedule**             | Specific meeting times, locations, and instructor assignments for course offerings                | Timetabling           | "CS101 Section A meets MWF 9:00-9:50 AM in Computer Lab 101 with Prof. Smith"                             |
| **Course Section**              | Specific offering of a course with defined capacity, schedule, and instructor                     | Enrollment management | "CS101 has three sections in Fall 2024: Section A (50 seats), Section B (45 seats), Section C (40 seats)" |
| **Curriculum**                  | Structured sequence of courses and requirements leading to a degree or certificate                | Program design        | "The Computer Science curriculum requires 120 credit hours including core, major, and elective courses"   |
| **Degree Program**              | Formal academic program leading to a specific degree upon completion                              | Academic offerings    | "Bachelor of Science in Computer Science, Master of Business Administration"                              |

### Enrollment and Registration

| Term                      | Definition                                                                                          | Context                 | Example                                                                                                           |
| ------------------------- | --------------------------------------------------------------------------------------------------- | ----------------------- | ----------------------------------------------------------------------------------------------------------------- |
| **Add/Drop Period**       | Designated time frame when students can modify their course enrollment without penalty              | Registration management | "Students have until the end of the second week to add or drop courses without financial penalty"                 |
| **Course Capacity**       | Maximum number of students allowed to enroll in a specific course section                           | Resource management     | "Course capacity is determined by classroom size, safety requirements, and pedagogical effectiveness"             |
| **Course Waitlist**       | Ordered list of students waiting for enrollment if space becomes available                          | Demand management       | "Students on the waitlist are automatically enrolled when other students drop, following waitlist priority order" |
| **Enrollment Status**     | Student's current course load classification affecting financial aid and benefits                   | Status tracking         | "Full-time (12+ credit hours), Three-quarter time (9-11), Half-time (6-8), Less than half-time (<6)"              |
| **Late Registration**     | Enrollment in courses after the standard registration period with potential penalties               | Exception handling      | "Late registration requires advisor approval and may include additional fees"                                     |
| **Registration Priority** | System for determining order of access to course registration                                       | Fair access management  | "Registration priority based on class level, honors status, and special program participation"                    |
| **Withdrawal**            | Formal process of leaving a course or all courses with specific academic and financial implications | Student services        | "Course withdrawal after the add/drop period results in 'W' grade without GPA impact"                             |

### Grading and Assessment

| Term                          | Definition                                                                                                          | Context                 | Example                                                                                   |
| ----------------------------- | ------------------------------------------------------------------------------------------------------------------- | ----------------------- | ----------------------------------------------------------------------------------------- |
| **Grade Point Average (GPA)** | Numerical representation of academic performance calculated from course grades and credit hours                     | Performance measurement | "Cumulative GPA calculated as total grade points divided by total credit hours attempted" |
| **Grade Points**              | Numerical values assigned to letter grades for GPA calculation                                                      | Standardization         | "A=4.0, A-=3.7, B+=3.3, B=3.0, B-=2.7, C+=2.3, C=2.0, C-=1.7, D+=1.3, D=1.0, F=0.0"       |
| **Grade Scale**               | Standardized system for converting numerical scores to letter grades                                                | Assessment standards    | "90-100%=A, 80-89%=B, 70-79%=C, 60-69%=D, Below 60%=F"                                    |
| **Incomplete Grade (I)**      | Temporary grade assigned when student cannot complete course requirements due to circumstances beyond their control | Exception handling      | "Incomplete grades must be resolved within one academic term or convert to 'F'"           |
| **Pass/Fail Grading**         | Alternative grading system where students receive 'P' (Pass) or 'F' (Fail) instead of letter grades                 | Grading options         | "Pass/Fail courses do not affect GPA calculation but count toward degree requirements"    |
| **Transcript**                | Official academic record showing all courses, grades, and academic standing                                         | Academic documentation  | "Official transcripts required for transfer applications and graduate school admissions"  |

### Faculty and Staff

| Term                        | Definition                                                                             | Context                 | Example                                                                                   |
| --------------------------- | -------------------------------------------------------------------------------------- | ----------------------- | ----------------------------------------------------------------------------------------- |
| **Department Chair**        | Faculty member responsible for administrative leadership of academic department        | Academic governance     | "Department Chair oversees curriculum development, faculty hiring, and budget management" |
| **Faculty Load**            | Total teaching, research, and service responsibilities assigned to faculty member      | Workload management     | "Standard faculty load includes 3 courses per term plus research and service obligations" |
| **Instructor of Record**    | Faculty member officially responsible for teaching a course and assigning final grades | Academic responsibility | "Only the Instructor of Record can submit grade changes and course completion reports"    |
| **Office Hours**            | Scheduled times when faculty are available to meet with students for academic support  | Student services        | "Each faculty member must maintain minimum 3 hours per week of posted office hours"       |
| **Teaching Assistant (TA)** | Graduate student who assists with course instruction under faculty supervision         | Instructional support   | "TAs may lead discussion sections, grade assignments, and hold office hours"              |

## Technical Architecture Terminology

### CQRS and Domain-Driven Design

| Term                    | Definition                                                                            | Context             | Example                                                                                              |
| ----------------------- | ------------------------------------------------------------------------------------- | ------------------- | ---------------------------------------------------------------------------------------------------- |
| **Aggregate**           | Cluster of domain objects treated as single unit for data changes and consistency     | Domain modeling     | `Student` aggregate includes StudentId, PersonalInfo, ContactInfo, and enrollment collection         |
| **Aggregate Root**      | Main entity within aggregate that controls access and maintains consistency           | Domain boundaries   | `Student` is aggregate root; external code cannot directly modify internal enrollment collection     |
| **Bounded Context**     | Explicit boundary within which domain model and ubiquitous language apply             | System architecture | Academic Management has separate contexts: Student Management, Course Catalog, Enrollment Processing |
| **Command**             | Request to perform action that changes system state                                   | CQRS pattern        | `EnrollStudentInCourseCommand` represents intent to enroll student in specific course                |
| **Command Handler**     | Component that processes commands and coordinates business logic execution            | CQRS implementation | `EnrollStudentInCourseCommandHandler` validates business rules and persists enrollment               |
| **Domain Event**        | Something meaningful that happened in domain, published for other components to react | Event-driven design | `StudentEnrolledDomainEvent` fired when enrollment successfully created                              |
| **Domain Service**      | Stateless service containing business logic that doesn't naturally fit within entity  | Domain modeling     | `GPACalculationService` computes student GPA using complex business rules                            |
| **Entity**              | Domain object with unique identity that persists over time                            | Domain modeling     | `Student`, `Course`, `Enrollment` are entities with unique identifiers                               |
| **Query**               | Request for data that doesn't modify system state                                     | CQRS pattern        | `GetStudentTranscriptQuery` retrieves student academic history for display                           |
| **Query Handler**       | Component that processes queries and returns requested data                           | CQRS implementation | `GetStudentTranscriptQueryHandler` fetches and formats transcript data                               |
| **Repository**          | Abstraction for accessing and persisting aggregates                                   | Data access pattern | `IStudentRepository` provides methods to save and retrieve Student aggregates                        |
| **Specification**       | Encapsulated business rule that can be evaluated against domain objects               | Business logic      | `StudentEligibleForEnrollmentSpecification` determines if student can enroll                         |
| **Ubiquitous Language** | Common vocabulary shared between developers and domain experts                        | Communication       | Terms like "Enrollment," "Prerequisites," "Academic Standing" mean same thing to all stakeholders    |
| **Value Object**        | Immutable object that represents descriptive aspect without identity                  | Domain modeling     | `PersonalInfo`, `ContactInfo`, `Grade` are value objects describing entity attributes                |

### .NET and Application Architecture

| Term                          | Definition                                                                            | Context             | Example                                                                               |
| ----------------------------- | ------------------------------------------------------------------------------------- | ------------------- | ------------------------------------------------------------------------------------- |
| **API Controller**            | Class that handles HTTP requests and returns responses in Web API                     | Presentation layer  | `StudentsController` handles HTTP requests for student-related operations             |
| **Application Service**       | Orchestrates domain logic and coordinates between layers                              | Application layer   | `EnrollmentApplicationService` coordinates enrollment workflow using domain services  |
| **Clean Architecture**        | Architectural pattern with dependency inversion and layer separation                  | System design       | Domain → Application → Infrastructure → Presentation layers with dependency inversion |
| **Dependency Injection (DI)** | Design pattern providing dependencies to objects rather than creating them internally | Decoupling          | Services injected into controllers via constructor parameters using DI container      |
| **Entity Framework (EF)**     | Object-relational mapping framework for .NET data access                              | Data persistence    | EF Core maps domain entities to database tables with configuration fluent API         |
| **MediatR**                   | Library implementing mediator pattern for in-process messaging                        | CQRS implementation | Commands and queries sent through `IMediator` to appropriate handlers                 |
| **Middleware**                | Software components in request processing pipeline                                    | HTTP processing     | Authentication, logging, exception handling implemented as middleware components      |
| **Model Builder**             | EF Core API for configuring entity mappings and database schema                       | Data modeling       | `OnModelCreating` method configures entity relationships, constraints, and indexes    |

### Azure Cloud Services

| Term                                  | Definition                                                                 | Context                | Example                                                                                   |
| ------------------------------------- | -------------------------------------------------------------------------- | ---------------------- | ----------------------------------------------------------------------------------------- |
| **App Service**                       | Platform-as-a-Service for hosting web applications and APIs                | Application hosting    | Academia API hosted on Azure App Service with auto-scaling and deployment slots           |
| **Application Insights**              | Application Performance Management service for monitoring and diagnostics  | Observability          | Custom telemetry tracks enrollment success rates and performance metrics                  |
| **Azure Active Directory (Azure AD)** | Cloud-based identity and access management service                         | Authentication         | Students and faculty authenticate using Azure AD with role-based access control           |
| **Azure SQL Database**                | Managed relational database service                                        | Data storage           | Academia database hosted as Azure SQL Database with automatic backups and scaling         |
| **Bicep**                             | Domain-specific language for deploying Azure resources                     | Infrastructure as Code | Bicep templates define complete infrastructure including networking, storage, and compute |
| **Key Vault**                         | Service for securely storing and accessing secrets, keys, and certificates | Security               | Database connection strings and API keys stored in Azure Key Vault                        |
| **Resource Group**                    | Logical container for grouping related Azure resources                     | Resource management    | All Academia resources grouped in `rg-academia-prod` resource group for management        |
| **Service Bus**                       | Messaging service for reliable communication between applications          | Async messaging        | Domain events published to Service Bus topics for event-driven communication              |

### Development and Operations

| Term                             | Definition                                                             | Context                  | Example                                                                           |
| -------------------------------- | ---------------------------------------------------------------------- | ------------------------ | --------------------------------------------------------------------------------- |
| **Blue-Green Deployment**        | Deployment strategy using two identical production environments        | Zero-downtime deployment | New version deployed to Green environment, traffic switched after validation      |
| **Continuous Integration (CI)**  | Practice of frequently integrating code changes with automated testing | Development workflow     | Every commit triggers automated build, test, and code quality checks              |
| **Continuous Deployment (CD)**   | Automated deployment of code changes that pass CI pipeline             | Release automation       | Successful CI builds automatically deployed to staging, production with approvals |
| **Feature Flag**                 | Technique for toggling features on/off without code deployment         | Release management       | New enrollment validation rules controlled by feature flags for gradual rollout   |
| **Health Check**                 | Automated test verifying application and dependencies are functioning  | Monitoring               | `/health` endpoint validates database connectivity, external service availability |
| **Infrastructure as Code (IaC)** | Managing infrastructure through machine-readable definition files      | DevOps practice          | Azure resources defined in Bicep templates, versioned in source control           |
| **Observability**                | Ability to understand system internal state through external outputs   | System monitoring        | Logs, metrics, and traces provide insights into system behavior and performance   |

## System-Specific Entities and Components

### Domain Entities

```csharp
/// <summary>
/// Core domain entities in the Academic Management System
/// </summary>
public static class DomainEntities
{
    /// <summary>
    /// Student: Central aggregate representing enrolled individuals
    /// </summary>
    public const string Student = "Student";

    /// <summary>
    /// Course: Academic offering with content, prerequisites, and capacity
    /// </summary>
    public const string Course = "Course";

    /// <summary>
    /// Enrollment: Relationship between student and course for specific term
    /// </summary>
    public const string Enrollment = "Enrollment";

    /// <summary>
    /// Faculty: Instructors responsible for teaching courses
    /// </summary>
    public const string Faculty = "Faculty";

    /// <summary>
    /// Department: Organizational unit managing related courses and programs
    /// </summary>
    public const string Department = "Department";

    /// <summary>
    /// Program: Degree program with curriculum requirements
    /// </summary>
    public const string Program = "Program";

    /// <summary>
    /// AcademicTerm: Time period when courses are offered
    /// </summary>
    public const string AcademicTerm = "AcademicTerm";
}

/// <summary>
/// Value objects representing descriptive attributes
/// </summary>
public static class ValueObjects
{
    /// <summary>
    /// PersonalInfo: Student's name, birth date, and demographic information
    /// </summary>
    public const string PersonalInfo = "PersonalInfo";

    /// <summary>
    /// ContactInfo: Email, phone, and address information
    /// </summary>
    public const string ContactInfo = "ContactInfo";

    /// <summary>
    /// Grade: Letter grade with associated grade points and percentage
    /// </summary>
    public const string Grade = "Grade";

    /// <summary>
    /// Money: Monetary amount with currency for tuition and fees
    /// </summary>
    public const string Money = "Money";

    /// <summary>
    /// Address: Physical mailing address with validation
    /// </summary>
    public const string Address = "Address";
}
```

### Business Rules and Invariants

| Term                             | Definition                                                                | Context                | Example                                                                                         |
| -------------------------------- | ------------------------------------------------------------------------- | ---------------------- | ----------------------------------------------------------------------------------------------- |
| **Enrollment Capacity Rule**     | Business rule preventing enrollment beyond course capacity limits         | Constraint enforcement | "Course sections cannot exceed maximum capacity defined by classroom and safety requirements"   |
| **Prerequisite Validation Rule** | Business rule ensuring students have completed required prior coursework  | Academic progression   | "Students must complete CS101 with grade C or better before enrolling in CS201"                 |
| **GPA Calculation Rule**         | Business rule defining how grade points are computed and weighted         | Academic standards     | "Only courses with letter grades count toward GPA; Pass/Fail courses excluded from calculation" |
| **Academic Standing Rule**       | Business rule determining student status based on performance metrics     | Status management      | "Students with cumulative GPA below 2.0 placed on academic probation"                           |
| **Enrollment Period Rule**       | Business rule defining when students can register for courses             | Registration control   | "Senior students register first, followed by juniors, sophomores, and freshmen"                 |
| **Grade Change Rule**            | Business rule governing when and how grades can be modified               | Academic integrity     | "Grade changes require instructor approval and department chair authorization after term end"   |
| **Withdrawal Policy Rule**       | Business rule defining academic and financial impact of course withdrawal | Student services       | "Withdrawal before 60% completion results in 'W' grade; after 60% results in 'WF' grade"        |

### System Operations and Workflows

| Term                               | Definition                                              | Context             | Example                                                                                                   |
| ---------------------------------- | ------------------------------------------------------- | ------------------- | --------------------------------------------------------------------------------------------------------- |
| **Enrollment Workflow**            | Multi-step process for registering students in courses  | Business process    | Validate eligibility → Check capacity → Create enrollment → Update records → Send confirmation            |
| **Grade Submission Workflow**      | Process for faculty to submit and modify student grades | Academic operations | Enter grades → Validate completeness → Submit for approval → Process grade changes → Generate transcripts |
| **Transcript Generation Workflow** | Process for creating official academic records          | Student services    | Retrieve academic history → Calculate GPA → Format transcript → Apply official seal → Deliver securely    |
| **Course Scheduling Workflow**     | Process for creating course offerings and timetables    | Academic planning   | Define course needs → Assign instructors → Schedule rooms → Publish schedule → Handle conflicts           |
| **Student Onboarding Workflow**    | Process for admitting and registering new students      | Student lifecycle   | Application review → Admission decision → Orientation → Initial registration → Account setup              |

### Integration Points and External Systems

| Term                                 | Definition                                                      | Context                | Example                                                                           |
| ------------------------------------ | --------------------------------------------------------------- | ---------------------- | --------------------------------------------------------------------------------- |
| **Student Information System (SIS)** | Comprehensive system managing all student academic data         | System integration     | Academic Management System integrates with existing SIS for data synchronization  |
| **Learning Management System (LMS)** | Platform delivering course content and managing online learning | Educational technology | Course enrollments synchronized with LMS for automatic course access              |
| **Financial Aid System**             | External system managing student financial assistance           | Financial integration  | Enrollment changes trigger financial aid recalculation and disbursement updates   |
| **Alumni System**                    | Database tracking graduated students and alumni relations       | Long-term engagement   | Graduate records transferred to alumni system for ongoing relationship management |
| **HR Information System (HRIS)**     | System managing faculty and staff employment data               | Employee integration   | Faculty course assignments synchronized with HR system for workload tracking      |

## Data and Integration Terminology

### Data Management

| Term                             | Definition                                                               | Context                 | Example                                                                                    |
| -------------------------------- | ------------------------------------------------------------------------ | ----------------------- | ------------------------------------------------------------------------------------------ |
| **Data Lake**                    | Centralized repository storing structured and unstructured data at scale | Analytics and reporting | Student behavior data, course materials, and system logs stored for institutional research |
| **Data Warehouse**               | Structured data storage optimized for analysis and reporting             | Business intelligence   | Academic performance data aggregated for trend analysis and institutional reporting        |
| **Master Data Management (MDM)** | Processes ensuring consistency and accuracy of key business data         | Data governance         | Student ID, course codes, and faculty records maintained as authoritative master data      |
| **Real-time Synchronization**    | Immediate data updates across integrated systems                         | System integration      | Enrollment changes immediately reflected in LMS, financial aid, and student portal         |

### API and Integration Patterns

| Term                          | Definition                                                                | Context             | Example                                                                              |
| ----------------------------- | ------------------------------------------------------------------------- | ------------------- | ------------------------------------------------------------------------------------ |
| **RESTful API**               | Web service following REST architectural principles for resource access   | System integration  | `/api/students/{id}/enrollments` endpoint provides student enrollment data           |
| **Event-Driven Architecture** | System design where components communicate through events                 | Loose coupling      | Student enrollment event triggers automatic LMS enrollment and email notification    |
| **Idempotent Operation**      | Operation that produces same result regardless of how many times executed | Reliable processing | Enrollment commands designed to be safely retried without creating duplicate records |
| **Circuit Breaker Pattern**   | Design pattern preventing cascade failures by monitoring service health   | Resilience          | External service calls protected by circuit breaker to prevent system-wide failures  |

## Compliance and Regulatory Terminology

### Educational Privacy (FERPA)

| Term                                | Definition                                                                                  | Context            | Example                                                                                 |
| ----------------------------------- | ------------------------------------------------------------------------------------------- | ------------------ | --------------------------------------------------------------------------------------- |
| **Directory Information**           | Student information that can be disclosed without consent under FERPA                       | Privacy compliance | Name, address, phone, email, enrollment status, degrees received can be shared publicly |
| **Educational Record**              | Records directly related to student maintained by educational institution                   | FERPA scope        | Transcripts, disciplinary records, financial aid records protected under FERPA          |
| **FERPA Consent**                   | Written permission from student to disclose educational records                             | Privacy protection | Parents need signed consent to access adult student's academic records                  |
| **Legitimate Educational Interest** | School official's need to review educational records to fulfill professional responsibility | Access control     | Academic advisors have legitimate interest in student records for advising purposes     |

### Data Protection (GDPR)

| Term                            | Definition                                                        | Context                | Example                                                                            |
| ------------------------------- | ----------------------------------------------------------------- | ---------------------- | ---------------------------------------------------------------------------------- |
| **Data Controller**             | Entity determining purposes and means of personal data processing | GDPR roles             | University acts as data controller for student personal information                |
| **Data Processor**              | Entity processing personal data on behalf of data controller      | GDPR roles             | Cloud service providers process student data as data processors                    |
| **Data Subject Rights**         | Individual rights regarding their personal data under GDPR        | Privacy rights         | Students can request access, correction, deletion, or portability of their data    |
| **Lawful Basis for Processing** | Legal justification required for processing personal data         | Compliance requirement | Student data processed under legitimate interests for educational service delivery |

## Performance and Scalability Terminology

### System Performance

| Term                         | Definition                                                      | Context               | Example                                                                                 |
| ---------------------------- | --------------------------------------------------------------- | --------------------- | --------------------------------------------------------------------------------------- |
| **Response Time**            | Time elapsed between request initiation and response completion | Performance metric    | Enrollment API responses must complete within 200ms for 95th percentile                 |
| **Throughput**               | Number of operations system can handle per unit time            | Capacity planning     | System designed to handle 1,000 concurrent enrollment requests during peak registration |
| **Concurrent Users**         | Number of users simultaneously accessing system                 | Load planning         | Peak concurrent load of 5,000 students during registration periods                      |
| **Database Connection Pool** | Managed set of reusable database connections                    | Resource optimization | Connection pool sized for peak load while minimizing resource waste                     |

### Scalability Patterns

| Term                   | Definition                                             | Context              | Example                                                                          |
| ---------------------- | ------------------------------------------------------ | -------------------- | -------------------------------------------------------------------------------- |
| **Horizontal Scaling** | Adding more servers to handle increased load           | Scale-out approach   | Additional app service instances deployed during high-traffic periods            |
| **Vertical Scaling**   | Increasing power of existing servers                   | Scale-up approach    | Database tier upgraded to higher SKU during peak enrollment periods              |
| **Auto-scaling**       | Automatic adjustment of resources based on demand      | Dynamic scaling      | App Service auto-scales based on CPU utilization and request queue length        |
| **Load Balancing**     | Distributing requests across multiple server instances | Traffic distribution | Azure Load Balancer distributes enrollment requests across app service instances |

## Security and Authentication Terminology

### Identity and Access Management

| Term                                  | Definition                                                                   | Context               | Example                                                                           |
| ------------------------------------- | ---------------------------------------------------------------------------- | --------------------- | --------------------------------------------------------------------------------- |
| **Claims-Based Identity**             | Identity representation using collection of claims about user                | Modern authentication | Student identity includes claims for student ID, program, and enrollment status   |
| **Multi-Factor Authentication (MFA)** | Security process requiring multiple forms of identity verification           | Enhanced security     | Students and faculty required to use MFA for system access                        |
| **Role-Based Access Control (RBAC)**  | Access control method based on user roles within organization                | Authorization         | Students, Faculty, Advisors, and Administrators have different system permissions |
| **Single Sign-On (SSO)**              | Authentication process allowing access to multiple systems with single login | User experience       | Students login once to access LMS, email, academic system, and library resources  |

### Data Security

| Term                                        | Definition                                   | Context                | Example                                                                               |
| ------------------------------------------- | -------------------------------------------- | ---------------------- | ------------------------------------------------------------------------------------- |
| **Data Encryption at Rest**                 | Protection of stored data through encryption | Data protection        | Student records encrypted in database using Azure SQL transparent data encryption     |
| **Data Encryption in Transit**              | Protection of data during transmission       | Communication security | All API communications use TLS 1.3 encryption for data in transit                     |
| **Field-Level Encryption**                  | Selective encryption of specific data fields | Selective protection   | Social Security Numbers encrypted at field level with separate key management         |
| **Personal Identifiable Information (PII)** | Data that can identify specific individual   | Privacy protection     | Student names, addresses, and ID numbers classified as PII requiring special handling |

## Cross-References and Related Terms

### Academic Domain → Technical Implementation

- **Student** → `Student` entity, `StudentId` value object, `IStudentRepository`
- **Course Enrollment** → `EnrollStudentInCourseCommand`, `EnrollmentCreatedDomainEvent`
- **Grade Calculation** → `GPACalculationService`, `GradeCalculationSpecification`
- **Academic Record** → `StudentTranscriptQuery`, `AcademicRecordProjection`

### Business Rules → System Implementation

- **Prerequisite Validation** → `PrerequisiteValidationService`, `CoursePrerequisiteSpecification`
- **Enrollment Capacity** → `CourseCapacityExceededException`, `EnrollmentCapacityRule`
- **Academic Standing** → `AcademicStandingCalculationService`, `StudentStatusUpdateEvent`

### External Systems → Integration Points

- **LMS Integration** → `LMSEnrollmentSyncService`, `CourseAccessGrantedEvent`
- **Financial Aid** → `FinancialAidNotificationService`, `EnrollmentChangedEvent`
- **Student Portal** → `StudentDashboardQuery`, `StudentProgressProjection`

## Usage Guidelines

### Consistent Terminology

- Always use terms as defined in this glossary
- When introducing new terms, add them to appropriate sections
- Reference existing terms rather than creating synonyms
- Use cross-references (→) to link related concepts

### Domain Language

- Prefer domain terms over technical jargon in business discussions
- Use technical terms appropriately in implementation contexts
- Maintain consistency between spoken language and code artifacts
- Ensure stakeholder understanding of both business and technical terms

### Documentation Standards

- Include glossary references in architectural decisions
- Define domain-specific terms in code comments
- Use consistent terminology in user stories and requirements
- Reference glossary entries in API documentation

## Related Documentation References

- [Project Overview](./project-overview.instructions.md) - Domain model and business rules
- [Architecture Design](./architecture-design.instructions.md) - Technical architecture patterns
- [CQRS Implementation](./cqrs-implementation.instructions.md) - Command/query patterns
- [Security Compliance](./security-compliance.instructions.md) - Security and privacy terminology

## Validation Checklist

Before considering the domain glossary complete, verify:

- [ ] All academic domain concepts are clearly defined with context and examples
- [ ] Technical architecture terms include implementation references and code examples
- [ ] CQRS and DDD terminology aligns with system implementation patterns
- [ ] Cross-references connect related terms across business and technical domains
- [ ] System-specific entities match actual domain model implementations
- [ ] Business rules and constraints are documented with their system implementations
- [ ] Integration terminology covers all external system touchpoints
- [ ] Compliance and regulatory terms address FERPA and GDPR requirements
- [ ] Performance and scalability terms align with system architecture decisions
- [ ] Security terminology covers authentication, authorization, and data protection
- [ ] Usage guidelines ensure consistent application across documentation and code
- [ ] All terms include sufficient context for both business and technical stakeholders
