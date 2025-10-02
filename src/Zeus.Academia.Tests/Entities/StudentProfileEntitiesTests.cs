using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;

namespace Zeus.Academia.Tests.Entities;

/// <summary>
/// Integration tests for Student Profile Management entities (Prompt 4 Task 3)
/// </summary>
public class StudentProfileEntitiesTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly DbContextOptions<AcademiaDbContext> _options;

    public StudentProfileEntitiesTests()
    {
        _options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AcademiaDbContext(_options, Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>());
        
        // Seed required data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var department = new Department
        {
            Name = "Computer Science",
            FullName = "Department of Computer Science",
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        
        var degree = new Degree
        {
            Code = "BS-CS",
            Title = "Bachelor of Science in Computer Science",
            Level = "Bachelor",
            TotalCreditHours = 120,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        _context.Departments.Add(department);
        _context.Degrees.Add(degree);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Student_WithExtendedProfileFields_ShouldSaveAndRetrieveCorrectly()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 1001,
            Name = "John Doe",
            StudentId = "ST001",
            PhoneNumber = "555-0123",
            DepartmentName = "Computer Science",
            DegreeCode = "BS-CS",
            
            // Extended profile fields
            PreferredName = "Johnny",
            Gender = "Male",
            DateOfBirth = new DateTime(1995, 5, 15),
            PrimaryAddress = "123 Main Street",
            City = "Anytown",
            State = "CA",
            PostalCode = "12345",
            Country = "United States",
            PersonalEmail = "johnny.doe@email.com",
            EmergencyPhone = "555-9911",
            CitizenshipStatus = "US Citizen",
            Ethnicity = "Caucasian",
            HasDietaryRestrictions = true,
            DietaryRestrictions = "Vegetarian",
            RequiresAccommodations = false,
            PreferredContactMethod = ContactMethod.Email,
            PersonalInterests = "Programming, Gaming, Reading",
            CareerGoals = "Software Engineer at a tech company",
            ProfilePhotoPath = "/uploads/students/1001/profile.jpg",
            ProfileCompletionPercentage = 85.5m,
            ProfileLastUpdated = DateTime.UtcNow,
            
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Act
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Assert
        var savedStudent = await _context.Students
            .FirstOrDefaultAsync(s => s.EmpNr == 1001);

        Assert.NotNull(savedStudent);
        Assert.Equal("Johnny", savedStudent.PreferredName);
        Assert.Equal("Male", savedStudent.Gender);
        Assert.Equal(new DateTime(1995, 5, 15), savedStudent.DateOfBirth);
        Assert.Equal("123 Main Street", savedStudent.PrimaryAddress);
        Assert.Equal("Anytown", savedStudent.City);
        Assert.Equal("CA", savedStudent.State);
        Assert.Equal("12345", savedStudent.PostalCode);
        Assert.Equal("United States", savedStudent.Country);
        Assert.Equal("johnny.doe@email.com", savedStudent.PersonalEmail);
        Assert.Equal("555-9911", savedStudent.EmergencyPhone);
        Assert.Equal("US Citizen", savedStudent.CitizenshipStatus);
        Assert.Equal("Caucasian", savedStudent.Ethnicity);
        Assert.True(savedStudent.HasDietaryRestrictions);
        Assert.Equal("Vegetarian", savedStudent.DietaryRestrictions);
        Assert.False(savedStudent.RequiresAccommodations);
        Assert.Equal(ContactMethod.Email, savedStudent.PreferredContactMethod);
        Assert.Equal("Programming, Gaming, Reading", savedStudent.PersonalInterests);
        Assert.Equal("Software Engineer at a tech company", savedStudent.CareerGoals);
        Assert.Equal("/uploads/students/1001/profile.jpg", savedStudent.ProfilePhotoPath);
        Assert.Equal(85.5m, savedStudent.ProfileCompletionPercentage);
    }

    [Fact]
    public async Task EmergencyContact_WithAllFields_ShouldSaveAndRetrieveCorrectly()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 1001,
            Name = "John Doe",
            StudentId = "ST001",
            PhoneNumber = "555-0123",
            DepartmentName = "Computer Science",
            DegreeCode = "BS-CS",
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);

        var emergencyContact = new EmergencyContact
        {
            StudentEmpNr = 1001,
            ContactName = "Mary Doe",
            Relationship = "Mother",
            PrimaryPhone = "555-0456",
            SecondaryPhone = "555-0789",
            Email = "mary.doe@email.com",
            Address = "456 Oak Street",
            City = "Hometown",
            State = "CA",
            PostalCode = "54321",
            Country = "United States",
            Priority = 1,
            NotifyInEmergency = true,
            NotifyForAcademicIssues = true,
            NotifyForFinancialMatters = false,
            FerpaAuthorized = true,
            PreferredContactMethod = ContactMethod.Phone,
            Notes = "Primary emergency contact",
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Act
        _context.EmergencyContacts.Add(emergencyContact);
        await _context.SaveChangesAsync();

        // Assert
        var savedContact = await _context.EmergencyContacts
            .Include(ec => ec.Student)
            .FirstOrDefaultAsync(ec => ec.StudentEmpNr == 1001);

        Assert.NotNull(savedContact);
        Assert.Equal("Mary Doe", savedContact.ContactName);
        Assert.Equal("Mother", savedContact.Relationship);
        Assert.Equal("555-0456", savedContact.PrimaryPhone);
        Assert.Equal("555-0789", savedContact.SecondaryPhone);
        Assert.Equal("mary.doe@email.com", savedContact.Email);
        Assert.Equal("456 Oak Street", savedContact.Address);
        Assert.Equal("Hometown", savedContact.City);
        Assert.Equal("CA", savedContact.State);
        Assert.Equal("54321", savedContact.PostalCode);
        Assert.Equal("United States", savedContact.Country);
        Assert.Equal(1, savedContact.Priority);
        Assert.True(savedContact.NotifyInEmergency);
        Assert.True(savedContact.NotifyForAcademicIssues);
        Assert.False(savedContact.NotifyForFinancialMatters);
        Assert.True(savedContact.FerpaAuthorized);
        Assert.Equal(ContactMethod.Phone, savedContact.PreferredContactMethod);
        Assert.Equal("Primary emergency contact", savedContact.Notes);
        Assert.True(savedContact.IsActive);
        Assert.NotNull(savedContact.Student);
        Assert.Equal("John Doe", savedContact.Student.Name);
    }

    [Fact]
    public async Task StudentDocument_WithAllFields_ShouldSaveAndRetrieveCorrectly()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 1001,
            Name = "John Doe",
            StudentId = "ST001",
            PhoneNumber = "555-0123",
            DepartmentName = "Computer Science",
            DegreeCode = "BS-CS",
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);

        var document = new StudentDocument
        {
            StudentEmpNr = 1001,
            DocumentType = StudentDocumentType.ProfilePhoto,
            OriginalFileName = "profile_photo.jpg",
            StoredFileName = "1001_profile_20231002_001.jpg",
            FilePath = "/uploads/students/1001/photos/1001_profile_20231002_001.jpg",
            MimeType = "image/jpeg",
            FileSizeBytes = 2048576, // 2MB
            Description = "Student profile photo for ID card",
            AccessLevel = DocumentAccessLevel.StudentAndDepartment,
            IsRequired = false,
            IsVerified = true,
            VerifiedBy = "Admin User",
            VerificationDate = DateTime.UtcNow,
            ExpirationDate = null,
            Notes = "High quality photo suitable for official documents",
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Act
        _context.StudentDocuments.Add(document);
        await _context.SaveChangesAsync();

        // Assert
        var savedDocument = await _context.StudentDocuments
            .Include(sd => sd.Student)
            .FirstOrDefaultAsync(sd => sd.StudentEmpNr == 1001);

        Assert.NotNull(savedDocument);
        Assert.Equal(StudentDocumentType.ProfilePhoto, savedDocument.DocumentType);
        Assert.Equal("profile_photo.jpg", savedDocument.OriginalFileName);
        Assert.Equal("1001_profile_20231002_001.jpg", savedDocument.StoredFileName);
        Assert.Equal("/uploads/students/1001/photos/1001_profile_20231002_001.jpg", savedDocument.FilePath);
        Assert.Equal("image/jpeg", savedDocument.MimeType);
        Assert.Equal(2048576, savedDocument.FileSizeBytes);
        Assert.Equal("Student profile photo for ID card", savedDocument.Description);
        Assert.Equal(DocumentAccessLevel.StudentAndDepartment, savedDocument.AccessLevel);
        Assert.False(savedDocument.IsRequired);
        Assert.True(savedDocument.IsVerified);
        Assert.Equal("Admin User", savedDocument.VerifiedBy);
        Assert.NotNull(savedDocument.VerificationDate);
        Assert.Null(savedDocument.ExpirationDate);
        Assert.Equal("High quality photo suitable for official documents", savedDocument.Notes);
        Assert.True(savedDocument.IsActive);
        Assert.NotNull(savedDocument.Student);
        Assert.Equal("John Doe", savedDocument.Student.Name);
    }

    [Fact]
    public async Task AcademicAdvisor_WithAllFields_ShouldSaveAndRetrieveCorrectly()
    {
        // Arrange
        var advisor = new AcademicAdvisor
        {
            FacultyEmpNr = 2001,
            AdvisorName = "Dr. Jane Smith",
            Title = "Professor",
            Email = "jane.smith@university.edu",
            PhoneNumber = "555-0321",
            OfficeLocation = "Science Building, Room 301",
            DepartmentName = "Computer Science",
            MaxStudentLoad = 25,
            Specializations = "Software Engineering, Data Science, Machine Learning",
            OfficeHours = "Monday 2-4 PM, Wednesday 10-12 PM",
            PreferredContactMethod = ContactMethod.Email,
            Notes = "Specializes in graduate student mentoring",
            IsActive = true,
            IsAcceptingNewStudents = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Act
        _context.AcademicAdvisors.Add(advisor);
        await _context.SaveChangesAsync();

        // Assert
        var savedAdvisor = await _context.AcademicAdvisors
            .Include(aa => aa.Department)
            .FirstOrDefaultAsync(aa => aa.FacultyEmpNr == 2001);

        Assert.NotNull(savedAdvisor);
        Assert.Equal(2001, savedAdvisor.FacultyEmpNr);
        Assert.Equal("Dr. Jane Smith", savedAdvisor.AdvisorName);
        Assert.Equal("Professor", savedAdvisor.Title);
        Assert.Equal("jane.smith@university.edu", savedAdvisor.Email);
        Assert.Equal("555-0321", savedAdvisor.PhoneNumber);
        Assert.Equal("Science Building, Room 301", savedAdvisor.OfficeLocation);
        Assert.Equal("Computer Science", savedAdvisor.DepartmentName);
        Assert.Equal(25, savedAdvisor.MaxStudentLoad);
        Assert.Equal("Software Engineering, Data Science, Machine Learning", savedAdvisor.Specializations);
        Assert.Equal("Monday 2-4 PM, Wednesday 10-12 PM", savedAdvisor.OfficeHours);
        Assert.Equal(ContactMethod.Email, savedAdvisor.PreferredContactMethod);
        Assert.Equal("Specializes in graduate student mentoring", savedAdvisor.Notes);
        Assert.True(savedAdvisor.IsActive);
        Assert.True(savedAdvisor.IsAcceptingNewStudents);
        Assert.NotNull(savedAdvisor.Department);
        Assert.Equal("Computer Science", savedAdvisor.Department.Name);
    }

    [Fact]
    public async Task StudentAdvisorAssignment_WithAllFields_ShouldSaveAndRetrieveCorrectly()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 1001,
            Name = "John Doe",
            StudentId = "ST001",
            PhoneNumber = "555-0123",
            DepartmentName = "Computer Science",
            DegreeCode = "BS-CS",
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        var advisor = new AcademicAdvisor
        {
            Id = 1,
            FacultyEmpNr = 2001,
            AdvisorName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu",
            DepartmentName = "Computer Science",
            MaxStudentLoad = 25,
            IsActive = true,
            IsAcceptingNewStudents = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        _context.Students.Add(student);
        _context.AcademicAdvisors.Add(advisor);
        await _context.SaveChangesAsync();

        var assignment = new StudentAdvisorAssignment
        {
            StudentEmpNr = 1001,
            AdvisorId = 1,
            AdvisorType = AdvisorType.Academic,
            IsPrimary = true,
            AssignmentDate = DateTime.UtcNow,
            AssignmentReason = "Initial advisor assignment for new student",
            AssignedBy = "Academic Coordinator",
            Notes = "Student shows strong interest in software engineering",
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        // Act
        _context.StudentAdvisorAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Assert
        var savedAssignment = await _context.StudentAdvisorAssignments
            .Include(saa => saa.Student)
            .Include(saa => saa.Advisor)
            .FirstOrDefaultAsync(saa => saa.StudentEmpNr == 1001 && saa.AdvisorId == 1);

        Assert.NotNull(savedAssignment);
        Assert.Equal(1001, savedAssignment.StudentEmpNr);
        Assert.Equal(1, savedAssignment.AdvisorId);
        Assert.Equal(AdvisorType.Academic, savedAssignment.AdvisorType);
        Assert.True(savedAssignment.IsPrimary);
        Assert.Equal("Initial advisor assignment for new student", savedAssignment.AssignmentReason);
        Assert.Equal("Academic Coordinator", savedAssignment.AssignedBy);
        Assert.Equal("Student shows strong interest in software engineering", savedAssignment.Notes);
        Assert.True(savedAssignment.IsActive);
        Assert.Null(savedAssignment.EndDate);
        Assert.Null(savedAssignment.EndReason);
        Assert.NotNull(savedAssignment.Student);
        Assert.Equal("John Doe", savedAssignment.Student.Name);
        Assert.NotNull(savedAssignment.Advisor);
        Assert.Equal("Dr. Jane Smith", savedAssignment.Advisor.AdvisorName);
    }

    [Fact]
    public async Task Student_WithNavigationProperties_ShouldLoadRelatedEntities()
    {
        // Arrange
        var student = new Student
        {
            EmpNr = 1001,
            Name = "John Doe",
            StudentId = "ST001",
            PhoneNumber = "555-0123",
            DepartmentName = "Computer Science",
            DegreeCode = "BS-CS",
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        var emergencyContact = new EmergencyContact
        {
            StudentEmpNr = 1001,
            ContactName = "Mary Doe",
            Relationship = "Mother",
            PrimaryPhone = "555-0456",
            Priority = 1,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        var document = new StudentDocument
        {
            StudentEmpNr = 1001,
            DocumentType = StudentDocumentType.ProfilePhoto,
            OriginalFileName = "profile.jpg",
            StoredFileName = "1001_profile.jpg",
            FilePath = "/uploads/1001_profile.jpg",
            MimeType = "image/jpeg",
            FileSizeBytes = 1024,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        var advisor = new AcademicAdvisor
        {
            Id = 1,
            FacultyEmpNr = 2001,
            AdvisorName = "Dr. Jane Smith",
            Email = "jane.smith@university.edu",
            DepartmentName = "Computer Science",
            IsActive = true,
            IsAcceptingNewStudents = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        var assignment = new StudentAdvisorAssignment
        {
            StudentEmpNr = 1001,
            AdvisorId = 1,
            AdvisorType = AdvisorType.Academic,
            IsPrimary = true,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        _context.Students.Add(student);
        _context.EmergencyContacts.Add(emergencyContact);
        _context.StudentDocuments.Add(document);
        _context.AcademicAdvisors.Add(advisor);
        _context.StudentAdvisorAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Act
        var loadedStudent = await _context.Students
            .Include(s => s.Department)
            .Include(s => s.Degree)
            .Include(s => s.EmergencyContacts.Where(ec => ec.IsActive))
            .Include(s => s.Documents.Where(d => d.IsActive))
            .Include(s => s.AdvisorAssignments.Where(aa => aa.IsActive))
                .ThenInclude(aa => aa.Advisor)
            .FirstOrDefaultAsync(s => s.EmpNr == 1001);

        // Assert
        Assert.NotNull(loadedStudent);
        Assert.Equal("John Doe", loadedStudent.Name);
        
        // Check department and degree relationships
        Assert.NotNull(loadedStudent.Department);
        Assert.Equal("Computer Science", loadedStudent.Department.Name);
        Assert.NotNull(loadedStudent.Degree);
        Assert.Equal("BS-CS", loadedStudent.Degree.Code);
        
        // Check emergency contacts
        Assert.Single(loadedStudent.EmergencyContacts);
        Assert.Equal("Mary Doe", loadedStudent.EmergencyContacts.First().ContactName);
        
        // Check documents
        Assert.Single(loadedStudent.Documents);
        Assert.Equal(StudentDocumentType.ProfilePhoto, loadedStudent.Documents.First().DocumentType);
        
        // Check advisor assignments
        Assert.Single(loadedStudent.AdvisorAssignments);
        var advisorAssignment = loadedStudent.AdvisorAssignments.First();
        Assert.True(advisorAssignment.IsPrimary);
        Assert.NotNull(advisorAssignment.Advisor);
        Assert.Equal("Dr. Jane Smith", advisorAssignment.Advisor.AdvisorName);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
