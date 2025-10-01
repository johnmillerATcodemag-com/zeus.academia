using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Service interface for sending emails including confirmation, password reset, and notification emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send email confirmation email to user
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="userName">User display name</param>
    /// <param name="confirmationToken">Email confirmation token</param>
    /// <returns>Email sending result</returns>
    Task<EmailSendResult> SendEmailConfirmationAsync(string email, string userName, string confirmationToken);

    /// <summary>
    /// Send password reset email to user
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="userName">User display name</param>
    /// <param name="resetToken">Password reset token</param>
    /// <returns>Email sending result</returns>
    Task<EmailSendResult> SendPasswordResetAsync(string email, string userName, string resetToken);

    /// <summary>
    /// Send welcome email to new user
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="userName">User display name</param>
    /// <param name="temporaryPassword">Temporary password (optional)</param>
    /// <returns>Email sending result</returns>
    Task<EmailSendResult> SendWelcomeEmailAsync(string email, string userName, string? temporaryPassword = null);

    /// <summary>
    /// Send account status change notification
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="userName">User display name</param>
    /// <param name="statusChange">Description of status change</param>
    /// <returns>Email sending result</returns>
    Task<EmailSendResult> SendAccountStatusChangeAsync(string email, string userName, string statusChange);

    /// <summary>
    /// Send role assignment notification
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="userName">User display name</param>
    /// <param name="roleName">Role name that was assigned</param>
    /// <param name="departmentContext">Department context (optional)</param>
    /// <returns>Email sending result</returns>
    Task<EmailSendResult> SendRoleAssignmentAsync(string toEmail, string userName, string roleName, string? departmentContext = null);


    /// <summary>
    /// Send general notification email
    /// </summary>
    /// <param name="email">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="message">Email message content</param>
    /// <param name="isHtml">Whether the message is HTML formatted</param>
    /// <returns>Email sending result</returns>
    Task<EmailSendResult> SendNotificationAsync(string email, string subject, string message, bool isHtml = false);
}

/// <summary>
/// Email sending result
/// </summary>
public class EmailSendResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? MessageId { get; set; }
}

/// <summary>
/// Email service implementation (simplified for demonstration)
/// In production, integrate with services like SendGrid, AWS SES, or Azure Communication Services
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _baseUrl;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _fromEmail = configuration["Email:FromEmail"] ?? "noreply@zeusacademia.edu";
        _fromName = configuration["Email:FromName"] ?? "Zeus Academia System";
        _baseUrl = configuration["App:BaseUrl"] ?? "https://localhost:5001";
    }

    public async Task<EmailSendResult> SendEmailConfirmationAsync(string email, string userName, string confirmationToken)
    {
        var subject = "Confirm Your Email Address - Zeus Academia";
        var confirmationUrl = $"{_baseUrl}/auth/confirm-email?token={confirmationToken}";

        var message = $@"
Dear {userName},

Welcome to Zeus Academia! Please confirm your email address by clicking the link below:

{confirmationUrl}

This link will expire in 24 hours.

If you did not create this account, please ignore this email.

Best regards,
Zeus Academia System";

        return await SendEmailAsync(email, subject, message);
    }

    public async Task<EmailSendResult> SendPasswordResetAsync(string email, string userName, string resetToken)
    {
        var subject = "Password Reset Request - Zeus Academia";
        var resetUrl = $"{_baseUrl}/auth/reset-password?token={resetToken}";

        var message = $@"
Dear {userName},

You have requested to reset your password for your Zeus Academia account.

Please click the link below to reset your password:

{resetUrl}

This link will expire in 1 hour.

If you did not request this password reset, please ignore this email and contact support if you have concerns.

Best regards,
Zeus Academia System";

        return await SendEmailAsync(email, subject, message);
    }

    public async Task<EmailSendResult> SendWelcomeEmailAsync(string email, string userName, string? temporaryPassword = null)
    {
        var subject = "Welcome to Zeus Academia!";

        var passwordInfo = temporaryPassword != null
            ? $@"
Your temporary password is: {temporaryPassword}

Please log in and change your password as soon as possible."
            : "Please use the password you created during registration.";

        var message = $@"
Dear {userName},

Welcome to Zeus Academia! Your account has been successfully created.

{passwordInfo}

You can log in at: {_baseUrl}/auth/login

Best regards,
Zeus Academia System";

        return await SendEmailAsync(email, subject, message);
    }

    public async Task<EmailSendResult> SendAccountStatusChangeAsync(string email, string userName, string statusChange)
    {
        var subject = "Account Status Change - Zeus Academia";

        var message = $@"
Dear {userName},

Your Zeus Academia account status has been changed:

{statusChange}

If you have questions about this change, please contact the system administrator.

Best regards,
Zeus Academia System";

        return await SendEmailAsync(email, subject, message);
    }

    public async Task<EmailSendResult> SendRoleAssignmentAsync(string email, string userName, string roleName, string? departmentContext = null)
    {
        var subject = "New Role Assignment - Zeus Academia";

        var departmentInfo = departmentContext != null
            ? $" in the {departmentContext} department"
            : "";

        var message = $@"
Dear {userName},

You have been assigned a new role in Zeus Academia:

Role: {roleName}{departmentInfo}

This role assignment grants you additional permissions in the system. Please log in to access your new capabilities.

Best regards,
Zeus Academia System";

        return await SendEmailAsync(email, subject, message);
    }

    public async Task<EmailSendResult> SendNotificationAsync(string email, string subject, string message, bool isHtml = false)
    {
        return await SendEmailAsync(email, subject, message, isHtml);
    }

    private async Task<EmailSendResult> SendEmailAsync(string toEmail, string subject, string message, bool isHtml = false)
    {
        try
        {
            // In a real implementation, integrate with an email service provider
            // For demonstration, we'll just log the email
            _logger.LogInformation("Sending email to {Email} with subject '{Subject}'", toEmail, subject);
            _logger.LogDebug("Email content: {Message}", message);

            // Simulate async email sending
            await Task.Delay(100);

            // In production, return actual message ID from email service
            var messageId = Guid.NewGuid().ToString();

            return new EmailSendResult
            {
                Success = true,
                MessageId = messageId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", toEmail);
            return new EmailSendResult
            {
                Success = false,
                Error = "Failed to send email"
            };
        }
    }
}