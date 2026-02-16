// IEmailService.cs and EmailService.cs - Email Sending
using System.Net;
using System.Net.Mail;
using PremierElectric.Api.DTOs;

namespace PremierElectric.Api.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactConfirmationAsync(ContactSubmissionDto contact, Guid ticketId);
        Task<bool> SendAdminNotificationAsync(ContactSubmissionDto contact, Guid ticketId);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendContactConfirmationAsync(ContactSubmissionDto contact, Guid ticketId)
        {
            try
            {
                if (!TryGetEmailSettings(out var settings, out var error))
                {
                    _logger.LogWarning("Email sending disabled or misconfigured: {Error}", error);
                    return false;
                }

                using (var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort))
                {
                    client.EnableSsl = settings.EnableSsl;
                    client.Credentials = new NetworkCredential(settings.SenderEmail, settings.SenderPassword);

                    var subject = "We Received Your Message - Premiere Electric";
                    var body = GenerateCustomerEmailBody(contact, ticketId);

                    var mailMessage = new MailMessage(settings.SenderEmail, contact.Email, subject, body)
                    {
                        IsBodyHtml = true
                    };

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Confirmation email sent to {contact.Email} with ticket {ticketId}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send confirmation email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendAdminNotificationAsync(ContactSubmissionDto contact, Guid ticketId)
        {
            try
            {
                if (!TryGetEmailSettings(out var settings, out var error))
                {
                    _logger.LogWarning("Email sending disabled or misconfigured: {Error}", error);
                    return false;
                }

                using (var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort))
                {
                    client.EnableSsl = settings.EnableSsl;
                    client.Credentials = new NetworkCredential(settings.SenderEmail, settings.SenderPassword);

                    var subject = $"New Contact Form Submission - {contact.Subject}";
                    var body = GenerateAdminEmailBody(contact, ticketId);

                    var mailMessage = new MailMessage(settings.SenderEmail, settings.AdminEmail, subject, body)
                    {
                        IsBodyHtml = true
                    };

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Admin notification sent for ticket {ticketId}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send admin notification: {ex.Message}");
                return false;
            }
        }

        private string GenerateCustomerEmailBody(ContactSubmissionDto contact, Guid ticketId)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ color: #1a3a52; margin-bottom: 20px; }}
                        .ticket {{ background: #f0f0f0; padding: 15px; border-radius: 5px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Thank You for Contacting Premiere Electric</h2>
                        </div>
                        <p>Dear {contact.FullName},</p>
                        <p>We have received your message and appreciate your interest in Premiere Electric. Our team will review your inquiry and get back to you shortly.</p>
                        
                        <div class='ticket'>
                            <strong>Your Ticket ID:</strong> {ticketId}<br/>
                            <p><em>Please keep this ID for your records and reference when we contact you.</em></p>
                        </div>
                        
                        <h3>Your Inquiry Details:</h3>
                        <p><strong>Subject:</strong> {contact.Subject}</p>
                        <p><strong>Preferred Contact:</strong> {(string.IsNullOrEmpty(contact.PreferredContact) ? "Any" : contact.PreferredContact)}</p>
                        
                        <p>We typically respond within 24-48 business hours. If your request is urgent, please call us directly at (555) 123-4567.</p>
                        
                        <p>Best regards,<br/>Premiere Electric Team</p>
                    </div>
                </body>
                </html>";
        }

        private string GenerateAdminEmailBody(ContactSubmissionDto contact, Guid ticketId)
        {
            return $@"
                <html>
                <body>
                    <h2>New Contact Form Submission</h2>
                    <p><strong>Ticket ID:</strong> {ticketId}</p>
                    <p><strong>Name:</strong> {contact.FullName}</p>
                    <p><strong>Email:</strong> {contact.Email}</p>
                    <p><strong>Phone:</strong> {contact.PhoneNumber ?? "Not provided"}</p>
                    <p><strong>Subject:</strong> {contact.Subject}</p>
                    <p><strong>Service Category:</strong> {contact.ServiceCategory ?? "Not specified"}</p>
                    <p><strong>Preferred Contact:</strong> {contact.PreferredContact ?? "Not specified"}</p>
                    <h3>Message:</h3>
                    <p>{contact.Message}</p>
                    <p><em>Submitted at: {DateTime.UtcNow:F}</em></p>
                </body>
                </html>";
        }

        private bool TryGetEmailSettings(out EmailSettings settings, out string error)
        {
            settings = new EmailSettings
            {
                Enabled = _configuration.GetValue<bool>("EmailSettings:Enabled"),
                EnableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl", true),
                SmtpServer = _configuration["EmailSettings:SmtpServer"],
                SenderEmail = _configuration["EmailSettings:SenderEmail"],
                SenderPassword = _configuration["EmailSettings:SenderPassword"],
                AdminEmail = _configuration["EmailSettings:AdminEmail"]
            };

            if (!settings.Enabled)
            {
                error = "EmailSettings:Enabled is false";
                return false;
            }

            if (!int.TryParse(_configuration["EmailSettings:SmtpPort"], out var smtpPort))
            {
                error = "EmailSettings:SmtpPort is missing or invalid";
                return false;
            }

            settings.SmtpPort = smtpPort;

            if (string.IsNullOrWhiteSpace(settings.SmtpServer) ||
                string.IsNullOrWhiteSpace(settings.SenderEmail) ||
                string.IsNullOrWhiteSpace(settings.SenderPassword) ||
                string.IsNullOrWhiteSpace(settings.AdminEmail))
            {
                error = "EmailSettings are incomplete";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private sealed class EmailSettings
        {
            public bool Enabled { get; set; }
            public bool EnableSsl { get; set; } = true;
            public string SmtpServer { get; set; } = string.Empty;
            public int SmtpPort { get; set; }
            public string SenderEmail { get; set; } = string.Empty;
            public string SenderPassword { get; set; } = string.Empty;
            public string AdminEmail { get; set; } = string.Empty;
        }
    }
}
