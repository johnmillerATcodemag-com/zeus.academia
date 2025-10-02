using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Entities;
using Zeus.Academia.Infrastructure.Enums;
using Zeus.Academia.Infrastructure.Services.Interfaces;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service for managing student profiles and related functionality
/// </summary>
public class StudentProfileService : IStudentProfileService
{
    private readonly AcademiaDbContext _context;
    private readonly ILogger<StudentProfileService> _logger;

    public StudentProfileService(AcademiaDbContext context, ILogger<StudentProfileService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Student?> GetCompleteStudentProfileAsync(int studentEmpNr)
    {
        _logger.LogDebug("Getting complete profile for student {StudentEmpNr}", studentEmpNr);

        return await _context.Students
            .Include(s => s.Department)
            .Include(s => s.Degree)
            .Include(s => s.EmergencyContacts.Where(ec => ec.IsActive))
            .Include(s => s.Documents.Where(d => d.IsActive))
            .Include(s => s.AdvisorAssignments.Where(aa => aa.IsActive))
                .ThenInclude(aa => aa.Advisor)
            .FirstOrDefaultAsync(s => s.EmpNr == studentEmpNr);
    }

    public async Task<bool> UpdateStudentProfileAsync(int studentEmpNr, Student profileData)
    {
        _logger.LogInformation("Updating profile for student {StudentEmpNr}", studentEmpNr);

        var existingStudent = await _context.Students.FindAsync(studentEmpNr);
        if (existingStudent == null)
        {
            _logger.LogWarning("Student {StudentEmpNr} not found", studentEmpNr);
            return false;
        }

        // Validate profile data
        var validationResult = await ValidateProfileDataAsync(profileData);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Profile validation failed for student {StudentEmpNr}: {Errors}",
                studentEmpNr, string.Join(", ", validationResult.Errors));
            return false;
        }

        // Update profile fields
        existingStudent.PreferredName = profileData.PreferredName;
        existingStudent.Gender = profileData.Gender;
        existingStudent.DateOfBirth = profileData.DateOfBirth;
        existingStudent.PrimaryAddress = profileData.PrimaryAddress;
        existingStudent.City = profileData.City;
        existingStudent.State = profileData.State;
        existingStudent.PostalCode = profileData.PostalCode;
        existingStudent.Country = profileData.Country;
        existingStudent.PersonalEmail = profileData.PersonalEmail;
        existingStudent.EmergencyPhone = profileData.EmergencyPhone;
        existingStudent.CitizenshipStatus = profileData.CitizenshipStatus;
        existingStudent.Ethnicity = profileData.Ethnicity;
        existingStudent.HasDietaryRestrictions = profileData.HasDietaryRestrictions;
        existingStudent.DietaryRestrictions = profileData.DietaryRestrictions;
        existingStudent.RequiresAccommodations = profileData.RequiresAccommodations;
        existingStudent.PreferredContactMethod = profileData.PreferredContactMethod;
        existingStudent.PersonalInterests = profileData.PersonalInterests;
        existingStudent.CareerGoals = profileData.CareerGoals;
        existingStudent.ProfileLastUpdated = DateTime.UtcNow;
        existingStudent.ModifiedBy = "System";

        // Recalculate profile completion
        var completionPercentage = await CalculateProfileCompletionAsync(studentEmpNr);
        existingStudent.ProfileCompletionPercentage = completionPercentage;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully updated profile for student {StudentEmpNr}", studentEmpNr);
        return true;
    }

    public async Task<decimal> CalculateProfileCompletionAsync(int studentEmpNr)
    {
        _logger.LogDebug("Calculating profile completion for student {StudentEmpNr}", studentEmpNr);

        var student = await GetCompleteStudentProfileAsync(studentEmpNr);
        if (student == null) return 0;

        var totalFields = 0;
        var completedFields = 0;

        // Core required fields
        totalFields += 10;
        if (!string.IsNullOrWhiteSpace(student.Name)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.StudentId)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.PersonalEmail)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.PhoneNumber)) completedFields++;
        if (student.DateOfBirth.HasValue) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.PrimaryAddress)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.City)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.State)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.PostalCode)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.Country)) completedFields++;

        // Optional but recommended fields
        totalFields += 5;
        if (!string.IsNullOrWhiteSpace(student.PreferredName)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.Gender)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.CitizenshipStatus)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.PersonalInterests)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.CareerGoals)) completedFields++;

        // Emergency contact requirement
        totalFields += 1;
        if (student.EmergencyContacts.Any(ec => ec.IsActive)) completedFields++;

        // Profile photo
        totalFields += 1;
        if (!string.IsNullOrWhiteSpace(student.ProfilePhotoPath)) completedFields++;

        // Advisor assignment
        totalFields += 1;
        if (student.AdvisorAssignments.Any(aa => aa.IsActive)) completedFields++;

        var percentage = totalFields > 0 ? Math.Round((decimal)completedFields / totalFields * 100, 1) : 0;

        _logger.LogDebug("Profile completion for student {StudentEmpNr}: {Percentage}% ({CompletedFields}/{TotalFields})",
            studentEmpNr, percentage, completedFields, totalFields);

        return percentage;
    }

    public async Task<ProfileCompletionStatus> GetProfileCompletionStatusAsync(int studentEmpNr)
    {
        _logger.LogDebug("Getting profile completion status for student {StudentEmpNr}", studentEmpNr);

        var student = await GetCompleteStudentProfileAsync(studentEmpNr);
        if (student == null)
        {
            return new ProfileCompletionStatus { CompletionPercentage = 0 };
        }

        var status = new ProfileCompletionStatus();
        var missingFields = new List<string>();
        var completedFields = new List<string>();

        // Check core fields
        CheckField(student.Name, "Full Name", completedFields, missingFields);
        CheckField(student.StudentId, "Student ID", completedFields, missingFields);
        CheckField(student.PersonalEmail, "Personal Email", completedFields, missingFields);
        CheckField(student.PhoneNumber, "Phone Number", completedFields, missingFields);
        CheckField(student.DateOfBirth?.ToString(), "Date of Birth", completedFields, missingFields);
        CheckField(student.PrimaryAddress, "Primary Address", completedFields, missingFields);
        CheckField(student.City, "City", completedFields, missingFields);
        CheckField(student.State, "State", completedFields, missingFields);
        CheckField(student.PostalCode, "Postal Code", completedFields, missingFields);
        CheckField(student.Country, "Country", completedFields, missingFields);

        // Check optional fields
        if (!string.IsNullOrWhiteSpace(student.PreferredName)) completedFields.Add("Preferred Name");
        if (!string.IsNullOrWhiteSpace(student.Gender)) completedFields.Add("Gender");
        if (!string.IsNullOrWhiteSpace(student.CitizenshipStatus)) completedFields.Add("Citizenship Status");
        if (!string.IsNullOrWhiteSpace(student.PersonalInterests)) completedFields.Add("Personal Interests");
        if (!string.IsNullOrWhiteSpace(student.CareerGoals)) completedFields.Add("Career Goals");

        // Check related data
        status.HasEmergencyContact = student.EmergencyContacts.Any(ec => ec.IsActive);
        if (!status.HasEmergencyContact) missingFields.Add("Emergency Contact");
        else completedFields.Add("Emergency Contact");

        status.HasProfilePhoto = !string.IsNullOrWhiteSpace(student.ProfilePhotoPath);
        if (!status.HasProfilePhoto) missingFields.Add("Profile Photo");
        else completedFields.Add("Profile Photo");

        status.HasAdvisor = student.AdvisorAssignments.Any(aa => aa.IsActive);
        if (!status.HasAdvisor) missingFields.Add("Academic Advisor");
        else completedFields.Add("Academic Advisor");

        // Check documents
        var allDocuments = student.Documents.Where(d => d.IsActive).ToList();
        status.SubmittedDocumentCount = allDocuments.Count;
        status.RequiredDocumentCount = allDocuments.Count(d => d.IsRequired);

        status.CompletionPercentage = await CalculateProfileCompletionAsync(studentEmpNr);
        status.MissingFields = missingFields;
        status.CompletedFields = completedFields;
        status.RecommendedFields = new List<string> { "Preferred Name", "Personal Interests", "Career Goals" };

        return status;
    }

    public async Task<bool> UpdateProfilePhotoAsync(int studentEmpNr, string photoPath)
    {
        _logger.LogInformation("Updating profile photo for student {StudentEmpNr}", studentEmpNr);

        var student = await _context.Students.FindAsync(studentEmpNr);
        if (student == null) return false;

        student.ProfilePhotoPath = photoPath;
        student.ProfileLastUpdated = DateTime.UtcNow;
        student.ModifiedBy = "System";

        // Recalculate completion percentage
        student.ProfileCompletionPercentage = await CalculateProfileCompletionAsync(studentEmpNr);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<EmergencyContact> AddEmergencyContactAsync(int studentEmpNr, EmergencyContact emergencyContact)
    {
        _logger.LogInformation("Adding emergency contact for student {StudentEmpNr}", studentEmpNr);

        emergencyContact.StudentEmpNr = studentEmpNr;
        emergencyContact.CreatedBy = "System";
        emergencyContact.ModifiedBy = "System";

        _context.EmergencyContacts.Add(emergencyContact);
        await _context.SaveChangesAsync();

        // Update profile completion
        await UpdateProfileCompletionAsync(studentEmpNr);

        return emergencyContact;
    }

    public async Task<bool> UpdateEmergencyContactAsync(int contactId, EmergencyContact emergencyContact)
    {
        _logger.LogInformation("Updating emergency contact {ContactId}", contactId);

        var existing = await _context.EmergencyContacts.FindAsync(contactId);
        if (existing == null) return false;

        existing.ContactName = emergencyContact.ContactName;
        existing.Relationship = emergencyContact.Relationship;
        existing.PrimaryPhone = emergencyContact.PrimaryPhone;
        existing.SecondaryPhone = emergencyContact.SecondaryPhone;
        existing.Email = emergencyContact.Email;
        existing.Address = emergencyContact.Address;
        existing.City = emergencyContact.City;
        existing.State = emergencyContact.State;
        existing.PostalCode = emergencyContact.PostalCode;
        existing.Country = emergencyContact.Country;
        existing.Priority = emergencyContact.Priority;
        existing.NotifyInEmergency = emergencyContact.NotifyInEmergency;
        existing.NotifyForAcademicIssues = emergencyContact.NotifyForAcademicIssues;
        existing.NotifyForFinancialMatters = emergencyContact.NotifyForFinancialMatters;
        existing.FerpaAuthorized = emergencyContact.FerpaAuthorized;
        existing.PreferredContactMethod = emergencyContact.PreferredContactMethod;
        existing.Notes = emergencyContact.Notes;
        existing.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveEmergencyContactAsync(int contactId)
    {
        _logger.LogInformation("Removing emergency contact {ContactId}", contactId);

        var contact = await _context.EmergencyContacts.FindAsync(contactId);
        if (contact == null) return false;

        contact.IsActive = false;
        contact.ModifiedBy = "System";

        await _context.SaveChangesAsync();

        // Update profile completion
        await UpdateProfileCompletionAsync(contact.StudentEmpNr);

        return true;
    }

    public async Task<IEnumerable<EmergencyContact>> GetEmergencyContactsAsync(int studentEmpNr)
    {
        return await _context.EmergencyContacts
            .Where(ec => ec.StudentEmpNr == studentEmpNr && ec.IsActive)
            .OrderBy(ec => ec.Priority)
            .ToListAsync();
    }

    public async Task<StudentAdvisorAssignment> AssignAdvisorAsync(int studentEmpNr, int advisorId, AdvisorType advisorType, bool isPrimary = true, string? reason = null)
    {
        _logger.LogInformation("Assigning advisor {AdvisorId} to student {StudentEmpNr}", advisorId, studentEmpNr);

        // If this is a primary advisor, deactivate other primary advisors
        if (isPrimary)
        {
            var existingPrimary = await _context.StudentAdvisorAssignments
                .Where(saa => saa.StudentEmpNr == studentEmpNr && saa.IsPrimary && saa.IsActive)
                .ToListAsync();

            foreach (var assignment in existingPrimary)
            {
                assignment.IsActive = false;
                assignment.EndDate = DateTime.UtcNow;
                assignment.EndReason = "Replaced by new primary advisor";
                assignment.ModifiedBy = "System";
            }
        }

        var newAssignment = new StudentAdvisorAssignment
        {
            StudentEmpNr = studentEmpNr,
            AdvisorId = advisorId,
            AdvisorType = advisorType,
            IsPrimary = isPrimary,
            AssignmentReason = reason,
            AssignedBy = "System",
            CreatedBy = "System",
            ModifiedBy = "System"
        };

        _context.StudentAdvisorAssignments.Add(newAssignment);
        await _context.SaveChangesAsync();

        // Update profile completion
        await UpdateProfileCompletionAsync(studentEmpNr);

        return newAssignment;
    }

    public async Task<bool> RemoveAdvisorAssignmentAsync(int assignmentId, string? endReason = null)
    {
        _logger.LogInformation("Removing advisor assignment {AssignmentId}", assignmentId);

        var assignment = await _context.StudentAdvisorAssignments.FindAsync(assignmentId);
        if (assignment == null) return false;

        assignment.IsActive = false;
        assignment.EndDate = DateTime.UtcNow;
        assignment.EndReason = endReason ?? "Assignment ended";
        assignment.ModifiedBy = "System";

        await _context.SaveChangesAsync();

        // Update profile completion
        await UpdateProfileCompletionAsync(assignment.StudentEmpNr);

        return true;
    }

    public async Task<IEnumerable<StudentAdvisorAssignment>> GetCurrentAdvisorAssignmentsAsync(int studentEmpNr)
    {
        return await _context.StudentAdvisorAssignments
            .Include(saa => saa.Advisor)
            .Where(saa => saa.StudentEmpNr == studentEmpNr && saa.IsActive)
            .OrderByDescending(saa => saa.IsPrimary)
            .ThenBy(saa => saa.AssignmentDate)
            .ToListAsync();
    }

    public async Task<StudentAdvisorAssignment?> GetPrimaryAdvisorAsync(int studentEmpNr)
    {
        return await _context.StudentAdvisorAssignments
            .Include(saa => saa.Advisor)
            .FirstOrDefaultAsync(saa => saa.StudentEmpNr == studentEmpNr && saa.IsPrimary && saa.IsActive);
    }

    public async Task<StudentDocument> UploadDocumentAsync(int studentEmpNr, StudentDocument document)
    {
        _logger.LogInformation("Uploading document for student {StudentEmpNr}", studentEmpNr);

        document.StudentEmpNr = studentEmpNr;
        document.CreatedBy = "System";
        document.ModifiedBy = "System";

        _context.StudentDocuments.Add(document);
        await _context.SaveChangesAsync();

        // Update profile completion
        await UpdateProfileCompletionAsync(studentEmpNr);

        return document;
    }

    public async Task<IEnumerable<StudentDocument>> GetStudentDocumentsAsync(int studentEmpNr, StudentDocumentType? documentType = null)
    {
        var query = _context.StudentDocuments
            .Where(sd => sd.StudentEmpNr == studentEmpNr && sd.IsActive);

        if (documentType.HasValue)
        {
            query = query.Where(sd => sd.DocumentType == documentType.Value);
        }

        return await query
            .OrderByDescending(sd => sd.UploadDate)
            .ToListAsync();
    }

    public async Task<bool> VerifyDocumentAsync(int documentId, string verifiedBy, string? notes = null)
    {
        _logger.LogInformation("Verifying document {DocumentId}", documentId);

        var document = await _context.StudentDocuments.FindAsync(documentId);
        if (document == null) return false;

        document.IsVerified = true;
        document.VerifiedBy = verifiedBy;
        document.VerificationDate = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(notes))
        {
            document.Notes = notes;
        }
        document.ModifiedBy = "System";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDocumentAsync(int documentId)
    {
        _logger.LogInformation("Deleting document {DocumentId}", documentId);

        var document = await _context.StudentDocuments.FindAsync(documentId);
        if (document == null) return false;

        document.IsActive = false;
        document.ModifiedBy = "System";

        await _context.SaveChangesAsync();

        // Update profile completion
        await UpdateProfileCompletionAsync(document.StudentEmpNr);

        return true;
    }

    public async Task<(IEnumerable<Student> Students, int TotalCount)> GetStudentsWithIncompleteProfilesAsync(
        decimal minimumCompletionPercentage = 80m, int pageNumber = 1, int pageSize = 10)
    {
        _logger.LogDebug("Getting students with incomplete profiles (< {MinCompletion}%)", minimumCompletionPercentage);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Students
            .Where(s => s.IsActive && s.ProfileCompletionPercentage < minimumCompletionPercentage);

        var totalCount = await query.CountAsync();

        var students = await query
            .OrderBy(s => s.ProfileCompletionPercentage)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }

    public Task<ProfileValidationResult> ValidateProfileDataAsync(Student profileData)
    {
        var result = new ProfileValidationResult { IsValid = true };

        // Validate email format
        if (!string.IsNullOrWhiteSpace(profileData.PersonalEmail) && !profileData.PersonalEmail.Contains("@"))
        {
            result.Errors.Add("Invalid email format");
            result.IsValid = false;
        }

        // Validate phone number format (basic)
        if (!string.IsNullOrWhiteSpace(profileData.PhoneNumber) && profileData.PhoneNumber.Length < 10)
        {
            result.Errors.Add("Phone number must be at least 10 digits");
            result.IsValid = false;
        }

        // Validate date of birth
        if (profileData.DateOfBirth.HasValue)
        {
            if (profileData.DateOfBirth > DateTime.Today)
            {
                result.Errors.Add("Date of birth cannot be in the future");
                result.IsValid = false;
            }
            else if (profileData.DateOfBirth < DateTime.Today.AddYears(-120))
            {
                result.Errors.Add("Invalid date of birth");
                result.IsValid = false;
            }
        }

        // Add warnings for missing recommended fields
        if (string.IsNullOrWhiteSpace(profileData.PersonalEmail))
        {
            result.Warnings.Add("Personal email is recommended for better communication");
        }

        if (!profileData.DateOfBirth.HasValue)
        {
            result.Warnings.Add("Date of birth is recommended for age verification and student services");
        }

        return Task.FromResult(result);
    }

    private async Task UpdateProfileCompletionAsync(int studentEmpNr)
    {
        var completionPercentage = await CalculateProfileCompletionAsync(studentEmpNr);
        var student = await _context.Students.FindAsync(studentEmpNr);
        if (student != null)
        {
            student.ProfileCompletionPercentage = completionPercentage;
            student.ProfileLastUpdated = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private static void CheckField(string? value, string fieldName, List<string> completedFields, List<string> missingFields)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            missingFields.Add(fieldName);
        }
        else
        {
            completedFields.Add(fieldName);
        }
    }
}