using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Tests.Services;

/// <summary>
/// Unit tests for StudentProfileService (Prompt 4 Task 3)
/// </summary>
public class StudentProfileServiceTests : IDisposable
{
    private readonly AcademiaDbContext _context;
    private readonly Mock<ILogger<StudentProfileService>> _loggerMock;
    private readonly StudentProfileService _service;
    private readonly DbContextOptions<AcademiaDbContext> _options;

    public StudentProfileServiceTests()
    {
        // Create in-memory database for testing
        _options = new DbContextOptionsBuilder<AcademiaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AcademiaDbContext(_options, Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>());
        _loggerMock = new Mock<ILogger<StudentProfileService>>();
        _service = new StudentProfileService(_context, _loggerMock.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test department
        var department = new Department
        {
            Name = "Computer Science",
            FullName = "Department of Computer Science",
            HeadEmpNr = null,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Departments.Add(department);

        // Create test degree
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
        _context.Degrees.Add(degree);

        // Create test student
        var student = new Student
        {
            EmpNr = 1001,
            Name = "John Doe",
            StudentId = "ST001",
            PhoneNumber = "555-0123",
            DepartmentName = "Computer Science",
            DegreeCode = "BS-CS",
            PersonalEmail = "john.doe@email.com",
            ProfileCompletionPercentage = 50.0m,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "System"
        };
        _context.Students.Add(student);

        // Create test academic advisor
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
        _context.AcademicAdvisors.Add(advisor);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetCompleteStudentProfileAsync_ValidStudentId_ShouldReturnStudentWithRelatedData()
    {
        // Arrange
        var studentEmpNr = 1001;

        // Act
        var result = await _service.GetCompleteStudentProfileAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.EmpNr);
        Assert.Equal("John Doe", result.Name);
        Assert.NotNull(result.Department);
        Assert.NotNull(result.Degree);
    }

    [Fact]
    public async Task GetCompleteStudentProfileAsync_InvalidStudentId_ShouldReturnNull()
    {
        // Arrange
        var invalidStudentEmpNr = 9999;

        // Act
        var result = await _service.GetCompleteStudentProfileAsync(invalidStudentEmpNr);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStudentProfileAsync_ValidData_ShouldUpdateAndReturnTrue()
    {
        // Arrange
        var studentEmpNr = 1001;
        var updatedProfile = new Student
        {
            PreferredName = "Johnny",
            Gender = "Male",
            DateOfBirth = new DateTime(1995, 5, 15),
            PrimaryAddress = "123 Main St",
            City = "Anytown",
            State = "CA",
            PostalCode = "12345",
            Country = "United States",
            PersonalEmail = "johnny.doe@email.com",
            PersonalInterests = "Programming, Gaming",
            CareerGoals = "Software Development"
        };

        // Act
        var result = await _service.UpdateStudentProfileAsync(studentEmpNr, updatedProfile);

        // Assert
        Assert.True(result);

        // Verify the update
        var updatedStudent = await _context.Students.FindAsync(studentEmpNr);
        Assert.NotNull(updatedStudent);
        Assert.Equal("Johnny", updatedStudent.PreferredName);
        Assert.Equal("Male", updatedStudent.Gender);
        Assert.Equal("Programming, Gaming", updatedStudent.PersonalInterests);
    }

    [Fact]
    public async Task UpdateStudentProfileAsync_InvalidStudentId_ShouldReturnFalse()
    {
        // Arrange
        var invalidStudentEmpNr = 9999;
        var profileData = new Student { PreferredName = "Test" };

        // Act
        var result = await _service.UpdateStudentProfileAsync(invalidStudentEmpNr, profileData);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CalculateProfileCompletionAsync_PartialProfile_ShouldReturnCorrectPercentage()
    {
        // Arrange
        var studentEmpNr = 1001;

        // Act
        var result = await _service.CalculateProfileCompletionAsync(studentEmpNr);

        // Assert
        Assert.True(result > 0);
        Assert.True(result <= 100);
    }

    [Fact]
    public async Task GetProfileCompletionStatusAsync_ValidStudent_ShouldReturnDetailedStatus()
    {
        // Arrange
        var studentEmpNr = 1001;

        // Act
        var result = await _service.GetProfileCompletionStatusAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CompletionPercentage >= 0);
        Assert.NotNull(result.MissingFields);
        Assert.NotNull(result.CompletedFields);
        Assert.NotNull(result.RecommendedFields);
    }

    [Fact]
    public async Task AddEmergencyContactAsync_ValidContact_ShouldAddAndReturnContact()
    {
        // Arrange
        var studentEmpNr = 1001;
        var emergencyContact = new EmergencyContact
        {
            ContactName = "Mary Doe",
            Relationship = "Mother",
            PrimaryPhone = "555-0456",
            Email = "mary.doe@email.com",
            Priority = 1,
            NotifyInEmergency = true,
            FerpaAuthorized = true,
            IsActive = true
        };

        // Act
        var result = await _service.AddEmergencyContactAsync(studentEmpNr, emergencyContact);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal("Mary Doe", result.ContactName);
        Assert.Equal("Mother", result.Relationship);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetEmergencyContactsAsync_StudentWithContacts_ShouldReturnOrderedContacts()
    {
        // Arrange
        var studentEmpNr = 1001;

        // Add test contacts
        var contact1 = new EmergencyContact
        {
            StudentEmpNr = studentEmpNr,
            ContactName = "Contact 1",
            Relationship = "Parent",
            PrimaryPhone = "555-0001",
            Priority = 2,
            IsActive = true
        };
        var contact2 = new EmergencyContact
        {
            StudentEmpNr = studentEmpNr,
            ContactName = "Contact 2",
            Relationship = "Sibling",
            PrimaryPhone = "555-0002",
            Priority = 1,
            IsActive = true
        };

        _context.EmergencyContacts.AddRange(contact1, contact2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetEmergencyContactsAsync(studentEmpNr);

        // Assert
        var contacts = result.ToList();
        Assert.Equal(2, contacts.Count);
        Assert.Equal("Contact 2", contacts[0].ContactName); // Priority 1 should come first
        Assert.Equal("Contact 1", contacts[1].ContactName); // Priority 2 should come second
    }

    [Fact]
    public async Task AssignAdvisorAsync_ValidAssignment_ShouldCreateAssignment()
    {
        // Arrange
        var studentEmpNr = 1001;
        var advisorId = 1;
        var advisorType = AdvisorType.Academic;

        // Act
        var result = await _service.AssignAdvisorAsync(studentEmpNr, advisorId, advisorType, true, "Initial assignment");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(advisorId, result.AdvisorId);
        Assert.Equal(advisorType, result.AdvisorType);
        Assert.True(result.IsPrimary);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetPrimaryAdvisorAsync_StudentWithPrimaryAdvisor_ShouldReturnAdvisor()
    {
        // Arrange
        var studentEmpNr = 1001;
        var advisorId = 1;

        // Create assignment
        await _service.AssignAdvisorAsync(studentEmpNr, advisorId, AdvisorType.Academic, true);

        // Act
        var result = await _service.GetPrimaryAdvisorAsync(studentEmpNr);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(advisorId, result.AdvisorId);
        Assert.True(result.IsPrimary);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task UploadDocumentAsync_ValidDocument_ShouldCreateDocument()
    {
        // Arrange
        var studentEmpNr = 1001;
        var document = new StudentDocument
        {
            DocumentType = StudentDocumentType.ProfilePhoto,
            OriginalFileName = "profile.jpg",
            StoredFileName = "profile_001.jpg",
            FilePath = "/uploads/students/1001/profile_001.jpg",
            MimeType = "image/jpeg",
            FileSizeBytes = 1024,
            IsActive = true
        };

        // Act
        var result = await _service.UploadDocumentAsync(studentEmpNr, document);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(studentEmpNr, result.StudentEmpNr);
        Assert.Equal(StudentDocumentType.ProfilePhoto, result.DocumentType);
        Assert.Equal("profile.jpg", result.OriginalFileName);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetStudentDocumentsAsync_StudentWithDocuments_ShouldReturnDocuments()
    {
        // Arrange
        var studentEmpNr = 1001;
        var document = new StudentDocument
        {
            StudentEmpNr = studentEmpNr,
            DocumentType = StudentDocumentType.Transcript,
            OriginalFileName = "transcript.pdf",
            StoredFileName = "transcript_001.pdf",
            FilePath = "/uploads/students/1001/transcript_001.pdf",
            MimeType = "application/pdf",
            FileSizeBytes = 2048,
            IsActive = true
        };

        _context.StudentDocuments.Add(document);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetStudentDocumentsAsync(studentEmpNr);

        // Assert
        var documents = result.ToList();
        Assert.Single(documents);
        Assert.Equal(StudentDocumentType.Transcript, documents[0].DocumentType);
        Assert.Equal("transcript.pdf", documents[0].OriginalFileName);
    }

    [Fact]
    public async Task VerifyDocumentAsync_ValidDocument_ShouldUpdateVerificationStatus()
    {
        // Arrange
        var studentEmpNr = 1001;
        var document = new StudentDocument
        {
            StudentEmpNr = studentEmpNr,
            DocumentType = StudentDocumentType.Immigration,
            OriginalFileName = "id.jpg",
            StoredFileName = "id_001.jpg",
            FilePath = "/uploads/students/1001/id_001.jpg",
            MimeType = "image/jpeg",
            FileSizeBytes = 512,
            IsActive = true,
            IsVerified = false
        };

        _context.StudentDocuments.Add(document);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.VerifyDocumentAsync(document.Id, "Admin User", "Document verified successfully");

        // Assert
        Assert.True(result);

        // Verify the update
        var updatedDoc = await _context.StudentDocuments.FindAsync(document.Id);
        Assert.NotNull(updatedDoc);
        Assert.True(updatedDoc.IsVerified);
        Assert.Equal("Admin User", updatedDoc.VerifiedBy);
        Assert.NotNull(updatedDoc.VerificationDate);
    }

    [Fact]
    public async Task GetStudentsWithIncompleteProfilesAsync_ShouldReturnStudentsWithLowCompletion()
    {
        // Arrange
        var minimumCompletion = 80m;

        // Act
        var result = await _service.GetStudentsWithIncompleteProfilesAsync(minimumCompletion);

        // Assert
        Assert.NotNull(result.Students);
        Assert.True(result.TotalCount >= 0);

        foreach (var student in result.Students)
        {
            Assert.True(student.ProfileCompletionPercentage < minimumCompletion);
        }
    }

    [Fact]
    public async Task ValidateProfileDataAsync_ValidData_ShouldReturnValid()
    {
        // Arrange
        var profileData = new Student
        {
            PersonalEmail = "test@email.com",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1995, 1, 1)
        };

        // Act
        var result = await _service.ValidateProfileDataAsync(profileData);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateProfileDataAsync_InvalidEmail_ShouldReturnInvalid()
    {
        // Arrange
        var profileData = new Student
        {
            PersonalEmail = "invalid-email",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1995, 1, 1)
        };

        // Act
        var result = await _service.ValidateProfileDataAsync(profileData);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Invalid email format", result.Errors);
    }

    [Fact]
    public async Task ValidateProfileDataAsync_ShortPhoneNumber_ShouldReturnInvalid()
    {
        // Arrange
        var profileData = new Student
        {
            PersonalEmail = "test@email.com",
            PhoneNumber = "123",
            DateOfBirth = new DateTime(1995, 1, 1)
        };

        // Act
        var result = await _service.ValidateProfileDataAsync(profileData);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Phone number must be at least 10 digits", result.Errors);
    }

    [Fact]
    public async Task ValidateProfileDataAsync_FutureDateOfBirth_ShouldReturnInvalid()
    {
        // Arrange
        var profileData = new Student
        {
            PersonalEmail = "test@email.com",
            PhoneNumber = "1234567890",
            DateOfBirth = DateTime.Today.AddDays(1)
        };

        // Act
        var result = await _service.ValidateProfileDataAsync(profileData);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Date of birth cannot be in the future", result.Errors);
    }

    [Fact]
    public async Task UpdateProfilePhotoAsync_ValidPath_ShouldUpdatePhotoPath()
    {
        // Arrange
        var studentEmpNr = 1001;
        var photoPath = "/uploads/students/1001/profile.jpg";

        // Act
        var result = await _service.UpdateProfilePhotoAsync(studentEmpNr, photoPath);

        // Assert
        Assert.True(result);

        // Verify the update
        var student = await _context.Students.FindAsync(studentEmpNr);
        Assert.NotNull(student);
        Assert.Equal(photoPath, student.ProfilePhotoPath);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}