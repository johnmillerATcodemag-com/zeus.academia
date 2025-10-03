using Xunit;
using Zeus.Academia.Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace Zeus.Academia.Tests.Entities;

/// <summary>
/// Unit tests for faculty profile management entities (Prompt 5 Task 2).
/// Tests FacultyProfile, FacultyDocument, FacultyPublication, and OfficeAssignment entities.
/// </summary>
public class FacultyProfileManagementEntityTests
{
    #region FacultyProfile Tests

    [Fact]
    public void FacultyProfile_CreatesWithDefaultValues()
    {
        // Arrange & Act
        var profile = new FacultyProfile();

        // Assert
        Assert.Equal(0, profile.AcademicEmpNr);
        Assert.Null(profile.ProfessionalTitle);
        Assert.Null(profile.PreferredName);
        Assert.Null(profile.ProfessionalEmail);
        Assert.True(profile.IsPublicProfile);
        Assert.Null(profile.WebsiteUrl);
        Assert.Null(profile.ResearchInterests);
    }

    [Fact]
    public void FacultyProfile_CanSetBasicProperties()
    {
        // Arrange & Act
        var profile = new FacultyProfile
        {
            AcademicEmpNr = 12345,
            ProfessionalTitle = "Dr.",
            PreferredName = "John Doe"
        };

        // Assert
        Assert.Equal(12345, profile.AcademicEmpNr);
        Assert.Equal("Dr.", profile.ProfessionalTitle);
        Assert.Equal("John Doe", profile.PreferredName);
    }

    #endregion

    #region FacultyDocument Tests

    [Fact]
    public void FacultyDocument_CreatesWithDefaultValues()
    {
        // Arrange & Act
        var document = new FacultyDocument();

        // Assert
        Assert.Equal(0, document.AcademicEmpNr);
        Assert.Equal(string.Empty, document.DocumentType);
        Assert.Equal(string.Empty, document.Title);
        Assert.Equal(1, document.Version);
        Assert.True(document.IsCurrentVersion);
        Assert.False(document.IsPublic);
        Assert.False(document.IsApproved);
    }

    [Fact]
    public void FacultyDocument_CanSetBasicProperties()
    {
        // Arrange & Act
        var document = new FacultyDocument
        {
            AcademicEmpNr = 12345,
            DocumentType = "CV",
            Title = "Dr. John Doe CV",
            FilePath = "/documents/cv.pdf"
        };

        // Assert
        Assert.Equal(12345, document.AcademicEmpNr);
        Assert.Equal("CV", document.DocumentType);
        Assert.Equal("Dr. John Doe CV", document.Title);
        Assert.Equal("/documents/cv.pdf", document.FilePath);
    }

    [Fact]
    public void FacultyDocument_AccessibilityFlags_WorkCorrectly()
    {
        // Arrange
        var publicDocument = new FacultyDocument
        {
            IsPublic = true,
            IsApproved = true
        };

        var privateDocument = new FacultyDocument
        {
            IsPublic = false,
            IsApproved = true
        };

        var unapprovedDocument = new FacultyDocument
        {
            IsPublic = true,
            IsApproved = false
        };

        // Act & Assert
        Assert.True(publicDocument.IsPublic);
        Assert.True(publicDocument.IsApproved);
        Assert.False(privateDocument.IsPublic);
        Assert.True(privateDocument.IsApproved);
        Assert.True(unapprovedDocument.IsPublic);
        Assert.False(unapprovedDocument.IsApproved);
    }

    #endregion

    #region FacultyPublication Tests

    [Fact]
    public void FacultyPublication_CreatesWithDefaultValues()
    {
        // Arrange & Act
        var publication = new FacultyPublication();

        // Assert
        Assert.Equal(0, publication.AcademicId);
        Assert.Equal(string.Empty, publication.Title);
        Assert.Equal(string.Empty, publication.PublicationType);
        Assert.Equal(0, publication.PublicationYear);
        Assert.False(publication.IsPeerReviewed);
        Assert.False(publication.IsOpenAccess);
        Assert.Equal(0, publication.CitationCount);
    }

    [Fact]
    public void FacultyPublication_RequiredFieldsValidation_Succeeds()
    {
        // Arrange
        var publication = new FacultyPublication
        {
            AcademicId = 12345,
            Title = "Research on AI in Education",
            PublicationType = "Journal Article",
            PublicationYear = 2024
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(publication);
        bool isValid = Validator.TryValidateObject(publication, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    #endregion

    #region OfficeAssignment Tests

    [Fact]
    public void OfficeAssignment_CreatesWithDefaultValues()
    {
        // Arrange & Act
        var assignment = new OfficeAssignment();

        // Assert
        Assert.Equal(0, assignment.AcademicId);
        Assert.Equal(string.Empty, assignment.BuildingName);
        Assert.Equal(string.Empty, assignment.RoomNumber);
        Assert.False(assignment.HasWindowView);
        Assert.False(assignment.IsSharedOffice);
        Assert.False(assignment.HasConferenceCapability);
        Assert.False(assignment.IsAccessible);
        Assert.Equal("Active", assignment.AssignmentStatus);
    }

    [Fact]
    public void OfficeAssignment_RequiredFieldsValidation_Succeeds()
    {
        // Arrange
        var assignment = new OfficeAssignment
        {
            AcademicId = 12345,
            BuildingName = "Science Building",
            RoomNumber = "204A",
            AssignmentStartDate = DateTime.Today
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(assignment);
        bool isValid = Validator.TryValidateObject(assignment, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void OfficeAssignment_FullOfficeLocation_ConcatenatesCorrectly()
    {
        // Arrange
        var assignment = new OfficeAssignment
        {
            BuildingName = "Science Building",
            RoomNumber = "204A"
        };

        // Act
        var fullLocation = assignment.FullOfficeLocation;

        // Assert
        Assert.Equal("Science Building 204A", fullLocation);
    }

    [Fact]
    public void OfficeAssignment_IsCurrentAssignment_CalculatesCorrectly()
    {
        // Arrange
        var activeAssignment = new OfficeAssignment
        {
            AssignmentStatus = "Active",
            AssignmentStartDate = DateTime.Today.AddDays(-30)
        };

        var inactiveAssignment = new OfficeAssignment
        {
            AssignmentStatus = "Inactive",
            AssignmentStartDate = DateTime.Today.AddDays(-30)
        };

        // Act & Assert
        Assert.True(activeAssignment.IsCurrentAssignment);
        Assert.False(inactiveAssignment.IsCurrentAssignment);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void FacultyProfileManagement_EntityRelationships_AreConsistent()
    {
        // Arrange
        var academicId = 12345;

        var profile = new FacultyProfile
        {
            AcademicEmpNr = academicId,
            IsPublicProfile = true
        };

        var document = new FacultyDocument
        {
            AcademicEmpNr = academicId,
            DocumentType = "CV",
            Title = "Dr. John Doe CV",
            IsPublic = true,
            IsApproved = true
        };

        var publication = new FacultyPublication
        {
            AcademicId = academicId,
            Title = "Research Excellence in Academia",
            PublicationType = "Journal Article",
            PublicationYear = 2024
        };

        var office = new OfficeAssignment
        {
            AcademicId = academicId,
            BuildingName = "Academic Hall",
            RoomNumber = "301B",
            AssignmentStartDate = DateTime.Today,
            AssignmentStatus = "Active"
        };

        // Act & Assert
        Assert.Equal(academicId, profile.AcademicEmpNr);
        Assert.Equal(academicId, document.AcademicEmpNr);
        Assert.Equal(academicId, publication.AcademicId);
        Assert.Equal(academicId, office.AcademicId);

        // Verify business logic consistency
        Assert.True(profile.IsPublicProfile);
        Assert.True(document.IsPublic);
        Assert.True(document.IsApproved);
        Assert.True(office.IsCurrentAssignment);
    }

    #endregion
}