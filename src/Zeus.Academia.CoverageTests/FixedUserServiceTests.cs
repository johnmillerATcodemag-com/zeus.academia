using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Zeus.Academia.Infrastructure.Data;
using Zeus.Academia.Infrastructure.Services;
using Zeus.Academia.Infrastructure.Identity;
using Zeus.Academia.Infrastructure.Entities;

namespace Zeus.Academia.CoverageTests
{
    /// <summary>
    /// Comprehensive tests for UserService covering all major user management operations.
    /// Tests user registration, authentication, profile management, and password operations.
    /// </summary>
    public class FixedUserServiceTests : IDisposable
    {
        private readonly AcademiaDbContext _context;
        private readonly Mock<IPasswordService> _passwordService;
        private readonly Mock<IEmailService> _emailService;
        private readonly Mock<IRoleAssignmentService> _roleAssignmentService;
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public FixedUserServiceTests()
        {
            // Create test database
            var options = new DbContextOptionsBuilder<AcademiaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Create test configuration
            var configDict = new Dictionary<string, string?>
            {
                {"UserSettings:DefaultPasswordExpiration", "90"},
                {"UserSettings:PasswordMinLength", "8"},
                {"UserSettings:RequirePasswordChange", "true"}
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();

            _configuration = config;
            _context = new AcademiaDbContext(options, _configuration);

            // Setup mocks
            _passwordService = new Mock<IPasswordService>();
            _emailService = new Mock<IEmailService>();
            _roleAssignmentService = new Mock<IRoleAssignmentService>();
            _logger = new Mock<ILogger<UserService>>();

            // Setup default mock behaviors
            _passwordService.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");
            _passwordService.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _emailService.Setup(x => x.SendWelcomeEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new EmailSendResult { Success = true });

            // Mock the role hierarchy service
            var roleHierarchyService = new Mock<IRoleHierarchyService>();

            _userService = new UserService(
                _context,
                _passwordService.Object,
                _roleAssignmentService.Object,
                roleHierarchyService.Object,
                _emailService.Object,
                _logger.Object
            );

            SeedTestData();
        }

        [Fact]
        public async Task RegisterUserAsync_ValidRequest_Success()
        {
            // Arrange
            var request = new UserRegistrationRequest
            {
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                SendConfirmationEmail = false
            };

            // Act
            var result = await _userService.RegisterUserAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.User);
            Assert.Equal("testuser", result.User.UserName);
            Assert.Equal("test@example.com", result.User.Email);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task RegisterUserAsync_DuplicateEmail_Fails()
        {
            // Arrange
            var existingUser = new AcademiaUser
            {
                UserName = "existing",
                Email = "existing@example.com",
                PasswordHash = "hashed_password",
                FirstName = "Existing",
                LastName = "User",
                IsActive = true
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var request = new UserRegistrationRequest
            {
                Email = "existing@example.com",
                UserName = "newuser",
                Password = "Password123!",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _userService.RegisterUserAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
            Assert.Contains("A user with this email or username already exists", result.Errors);
        }

        [Fact]
        public async Task RegisterUserAsync_DuplicateUsername_Fails()
        {
            // Arrange
            var existingUser = new AcademiaUser
            {
                UserName = "testuser",
                Email = "existing@example.com",
                PasswordHash = "hashed_password",
                FirstName = "Existing",
                LastName = "User",
                IsActive = true
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var request = new UserRegistrationRequest
            {
                Email = "new@example.com",
                UserName = "testuser",
                Password = "Password123!",
                FirstName = "New",
                LastName = "User"
            };

            // Act
            var result = await _userService.RegisterUserAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.NotEmpty(result.Errors);
            Assert.Contains("A user with this email or username already exists", result.Errors);
        }

        [Fact]
        public async Task ValidateUserAsync_ValidCredentials_Success()
        {
            // Arrange
            var user = new AcademiaUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                EmailConfirmed = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.ValidateUserAsync("testuser", "password123");

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.User);
            Assert.Equal("testuser", result.User.UserName);
        }

        [Fact]
        public async Task ValidateUserAsync_InvalidPassword_Fails()
        {
            // Arrange
            var user = new AcademiaUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                FirstName = "Test",
                LastName = "User",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _passwordService.Setup(x => x.VerifyPassword("wrongpassword", "hashed_password"))
                .Returns(false);

            // Act
            var result = await _userService.ValidateUserAsync("testuser", "wrongpassword");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid username or password", result.FailureReason);
        }

        [Fact]
        public async Task ValidateUserAsync_InactiveUser_Fails()
        {
            // Arrange
            var user = new AcademiaUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                FirstName = "Test",
                LastName = "User",
                IsActive = false
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.ValidateUserAsync("testuser", "password123");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Account is inactive", result.FailureReason);
        }

        [Fact]
        public async Task ValidateUserAsync_NonExistentUser_Fails()
        {
            // Act
            var result = await _userService.ValidateUserAsync("nonexistent", "password123");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid username or password", result.FailureReason);
        }

        [Fact]
        public async Task GetUserProfileAsync_ExistingUser_ReturnsProfile()
        {
            // Arrange
            var academic = new Student
            {
                EmpNr = 12345,
                Name = "John Doe Student",
                StudentId = "ST12345"
            };
            _context.Students.Add(academic);

            var user = new AcademiaUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                Academic = academic,
                AcademicId = academic.EmpNr
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var profile = await _userService.GetUserProfileAsync(user.Id);

            // Assert
            Assert.NotNull(profile);
            Assert.Equal("testuser", profile.UserName);
            Assert.Equal("test@example.com", profile.Email);
            Assert.NotNull(profile.Academic);
            Assert.Equal(12345, profile.Academic.EmpNr);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ValidUpdate_Success()
        {
            // Arrange
            var user = new AcademiaUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                FirstName = "Test",
                LastName = "User",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var updateRequest = new UserProfileUpdateRequest
            {
                FirstName = "Updated",
                LastName = "Name",
                DisplayName = "Updated Name"
            };

            // Act
            var result = await _userService.UpdateUserProfileAsync(user.Id, updateRequest);

            // Assert
            Assert.True(result.Success);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.Equal("Updated", updatedUser?.FirstName);
            Assert.Equal("Name", updatedUser?.LastName);
        }

        [Fact]
        public async Task ChangePasswordAsync_ValidChange_Success()
        {
            // Arrange
            var userId = 123;
            var user = new AcademiaUser
            {
                Id = userId,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "old_hashed_password",
                FirstName = "Test",
                LastName = "User",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _passwordService.Setup(x => x.VerifyPassword("OldPassword123!", "old_hashed_password"))
                .Returns(true);
            _passwordService.Setup(x => x.HashPassword("NewPassword123!"))
                .Returns("new_hashed_password");

            // Act
            var result = await _userService.ChangePasswordAsync(userId, "OldPassword123!", "NewPassword123!");

            // Assert
            Assert.True(result.Success, $"Password change failed: {string.Join(", ", result.Errors)}");

            var updatedUser = await _context.Users.FindAsync(userId);
            Assert.Equal("new_hashed_password", updatedUser?.PasswordHash);
        }

        [Fact]
        public async Task ChangePasswordAsync_InvalidOldPassword_Fails()
        {
            // Arrange
            var user = new AcademiaUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "old_hashed_password",
                FirstName = "Test",
                LastName = "User",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _passwordService.Setup(x => x.VerifyPassword("wrongpassword", "old_hashed_password"))
                .Returns(false);

            // Act
            var result = await _userService.ChangePasswordAsync(user.Id, "wrongpassword", "newpassword");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Current password is incorrect", result.Errors);
        }

        [Fact]
        public async Task UserExistsAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var user = new AcademiaUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                FirstName = "Test",
                LastName = "User",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var exists = await _userService.UserExistsAsync("testuser");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task UserExistsAsync_NonExistentUser_ReturnsFalse()
        {
            // Act
            var exists = await _userService.UserExistsAsync("nonexistent");

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task CreateUserForAcademicAsync_ValidAcademic_Success()
        {
            // Arrange
            var academic = new Professor
            {
                EmpNr = 67890,
                Name = "Dr. Jane Smith",
                DepartmentName = "Computer Science"
            };
            _context.Professors.Add(academic);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.CreateUserForAcademicAsync(67890, "jane.smith@example.com", "TempPass123!");

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.User);
            Assert.Equal(67890, result.User.AcademicId);
            Assert.Equal("jane.smith@example.com", result.User.Email);
        }

        [Fact]
        public async Task CreateUserForAcademicAsync_NonExistentAcademic_Fails()
        {
            // Act
            var result = await _userService.CreateUserForAcademicAsync(99999, "nonexistent@example.com", "TempPass123!");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Academic entity not found", result.Errors);
        }

        private void SeedTestData()
        {
            // Add some basic test data if needed
            var testSubject = new Subject
            {
                Code = "TEST101",
                Title = "Test Subject",
                CreditHours = 3,
                IsActive = true
            };

            _context.Subjects.Add(testSubject);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}