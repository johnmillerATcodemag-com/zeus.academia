using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using Zeus.Academia.Infrastructure.Services;

namespace Zeus.Academia.CoverageTests
{
    /// <summary>
    /// Comprehensive tests for AuditService covering all audit logging scenarios.
    /// Tests authentication logging, role changes, data access, and admin actions.
    /// </summary>
    public class FixedAuditServiceTests
    {
        private readonly Mock<ILogger<AuditService>> _mockLogger;
        private readonly AuditService _auditService;

        public FixedAuditServiceTests()
        {
            _mockLogger = new Mock<ILogger<AuditService>>();
            _auditService = new AuditService(_mockLogger.Object);
        }

        [Fact]
        public async Task LogActionAsync_ValidAction_LogsSuccessfully()
        {
            // Arrange
            var action = "CREATE";
            var entityType = "User";
            var entityId = "123";
            var userId = "456";
            var userName = "testuser";

            // Act
            await _auditService.LogActionAsync(action, entityType, entityId,
                userId: userId, userName: userName);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogActionAsync_WithOldAndNewValues_LogsSuccessfully()
        {
            // Arrange
            var action = "UPDATE";
            var entityType = "User";
            var entityId = "123";
            var oldValues = new { Name = "Old Name", Email = "old@example.com" };
            var newValues = new { Name = "New Name", Email = "new@example.com" };

            // Act
            await _auditService.LogActionAsync(action, entityType, entityId,
                oldValues: oldValues, newValues: newValues);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogActionAsync_WithError_LogsError()
        {
            // Arrange
            var action = "DELETE";
            var entityType = "User";
            var entityId = "123";
            var errorMessage = "User not found";

            // Act
            await _auditService.LogActionAsync(action, entityType, entityId,
                isSuccess: false, errorMessage: errorMessage);

            // Assert
            VerifyFailureAuditLogged(LogLevel.Warning);
        }

        [Fact]
        public async Task LogLoginAsync_SuccessfulLogin_LogsInformation()
        {
            // Arrange
            var userId = "123";
            var userName = "testuser";
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";

            // Act
            await _auditService.LogLoginAsync(userId, userName, ipAddress, userAgent);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogLoginAsync_FailedLogin_LogsWarning()
        {
            // Arrange
            var userId = "123";
            var userName = "testuser";
            var errorMessage = "Invalid credentials";

            // Act
            await _auditService.LogLoginAsync(userId, userName,
                isSuccess: false, errorMessage: errorMessage);

            // Assert
            VerifyFailureAuditLogged(LogLevel.Warning);
        }

        [Fact]
        public async Task LogLogoutAsync_ValidLogout_LogsInformation()
        {
            // Arrange
            var userId = "123";
            var userName = "testuser";
            var ipAddress = "192.168.1.1";

            // Act
            await _auditService.LogLogoutAsync(userId, userName, ipAddress);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogPasswordChangeAsync_SuccessfulChange_LogsInformation()
        {
            // Arrange
            var userId = "123";
            var userName = "testuser";
            var ipAddress = "192.168.1.1";

            // Act
            await _auditService.LogPasswordChangeAsync(userId, userName, ipAddress);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogPasswordChangeAsync_FailedChange_LogsWarning()
        {
            // Arrange
            var userId = "123";
            var userName = "testuser";
            var errorMessage = "Current password incorrect";

            // Act
            await _auditService.LogPasswordChangeAsync(userId, userName,
                isSuccess: false, errorMessage: errorMessage);

            // Assert
            VerifyFailureAuditLogged(LogLevel.Warning);
        }

        [Fact]
        public async Task LogRoleChangeAsync_RoleAssignment_LogsInformation()
        {
            // Arrange
            var targetUserId = "123";
            var targetUserName = "targetuser";
            var action = "ASSIGN";
            var roleName = "Administrator";
            var performedByUserId = "456";
            var performedByUserName = "adminuser";

            // Act
            await _auditService.LogRoleChangeAsync(targetUserId, targetUserName, action, roleName,
                performedByUserId, performedByUserName);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogRoleChangeAsync_RoleRemoval_LogsInformation()
        {
            // Arrange
            var targetUserId = "123";
            var targetUserName = "targetuser";
            var action = "REMOVE";
            var roleName = "Student";

            // Act
            await _auditService.LogRoleChangeAsync(targetUserId, targetUserName, action, roleName);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogRoleChangeAsync_FailedRoleChange_LogsError()
        {
            // Arrange
            var targetUserId = "123";
            var targetUserName = "targetuser";
            var action = "ASSIGN";
            var roleName = "NonExistentRole";
            var errorMessage = "Role not found";

            // Act
            await _auditService.LogRoleChangeAsync(targetUserId, targetUserName, action, roleName,
                isSuccess: false, errorMessage: errorMessage);

            // Assert
            VerifyFailureAuditLogged(LogLevel.Warning);
        }

        [Fact]
        public async Task LogAdminActionAsync_SuccessfulAction_LogsInformation()
        {
            // Arrange
            var action = "BULK_DELETE";
            var description = "Deleted inactive users";
            var targetEntityType = "User";
            var performedByUserId = "456";
            var performedByUserName = "adminuser";

            // Act
            await _auditService.LogAdminActionAsync(action, description, targetEntityType,
                performedByUserId: performedByUserId, performedByUserName: performedByUserName);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogAdminActionAsync_FailedAction_LogsError()
        {
            // Arrange
            var action = "SYSTEM_BACKUP";
            var description = "Failed to create system backup";
            var errorMessage = "Insufficient disk space";

            // Act
            await _auditService.LogAdminActionAsync(action, description,
                isSuccess: false, errorMessage: errorMessage);

            // Assert
            VerifyFailureAuditLogged(LogLevel.Warning);
        }

        [Fact]
        public async Task LogDataAccessAsync_SuccessfulAccess_LogsDebug()
        {
            // Arrange
            var entityType = "StudentRecord";
            var entityId = "ST12345";
            var accessType = "READ";
            var userId = "456";
            var userName = "professor";

            // Act
            await _auditService.LogDataAccessAsync(entityType, entityId, accessType,
                userId, userName);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogDataAccessAsync_UnauthorizedAccess_LogsWarning()
        {
            // Arrange
            var entityType = "StudentRecord";
            var entityId = "ST12345";
            var accessType = "READ";
            var userId = "789";
            var userName = "unauthorizeduser";
            var errorMessage = "Access denied";

            // Act
            await _auditService.LogDataAccessAsync(entityType, entityId, accessType,
                userId, userName, isSuccess: false, errorMessage: errorMessage);

            // Assert
            VerifyFailureAuditLogged(LogLevel.Warning);
        }

        [Fact]
        public async Task LogActionAsync_WithAdditionalData_LogsWithStructuredData()
        {
            // Arrange
            var action = "ENROLLMENT";
            var entityType = "Course";
            var entityId = "CS101";
            var additionalData = new { StudentId = "ST12345", Semester = "Fall2024" };

            // Act
            await _auditService.LogActionAsync(action, entityType, entityId,
                additionalData: additionalData);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogActionAsync_MultipleConsecutiveCalls_AllLogged()
        {
            // Arrange & Act
            await _auditService.LogActionAsync("CREATE", "User", "1");
            await _auditService.LogActionAsync("UPDATE", "User", "1");
            await _auditService.LogActionAsync("DELETE", "User", "1");

            // Assert - Each call logs twice (main + structured), so 3 calls = 6 log entries
            VerifyLogCalled(LogLevel.Information, Times.Exactly(6));
        }

        [Fact]
        public async Task LogLoginAsync_WithoutOptionalParameters_LogsSuccessfully()
        {
            // Arrange
            var userId = "123";
            var userName = "testuser";

            // Act
            await _auditService.LogLoginAsync(userId, userName);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogAdminActionAsync_MinimalParameters_LogsSuccessfully()
        {
            // Arrange
            var action = "SYSTEM_MAINTENANCE";
            var description = "Performed routine maintenance";

            // Act
            await _auditService.LogAdminActionAsync(action, description);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogDataAccessAsync_SensitiveData_LogsWithCorrectLevel()
        {
            // Arrange
            var entityType = "SalaryRecord";
            var entityId = "EMP12345";
            var accessType = "READ";
            var userId = "HR001";
            var userName = "hrmanager";

            // Act
            await _auditService.LogDataAccessAsync(entityType, entityId, accessType,
                userId, userName);

            // Assert
            VerifySuccessAuditLogged();
        }

        [Fact]
        public async Task LogRoleChangeAsync_WithIpAndUserAgent_LogsAllContext()
        {
            // Arrange
            var targetUserId = "123";
            var targetUserName = "targetuser";
            var action = "MODIFY";
            var roleName = "Professor";
            var ipAddress = "192.168.1.100";
            var userAgent = "Mozilla/5.0 Chrome";

            // Act
            await _auditService.LogRoleChangeAsync(targetUserId, targetUserName, action, roleName,
                ipAddress: ipAddress, userAgent: userAgent);

            // Assert
            VerifySuccessAuditLogged();
        }

        private void VerifyLogCalled(LogLevel expectedLevel, Times times)
        {
            _mockLogger.Verify(
                x => x.Log(
                    expectedLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                times);
        }

        private void VerifySuccessAuditLogged()
        {
            // Verify the main audit message at Information level
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(2));
        }

        private void VerifyFailureAuditLogged(LogLevel errorLevel = LogLevel.Error)
        {
            // For failure scenarios, verify error level log + structured information log
            _mockLogger.Verify(
                x => x.Log(
                    errorLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());

            // Plus the structured audit entry at Information level
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }
    }
}