// IEmailService.cs and EmailService.cs - Email Sending
using System.Net;
using System.Net.Mail;
using PremierElectric.Application.DTOs;

namespace PremierElectric.Application.Services
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
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    var subject = "We Received Your Message - Premiere Electric";
                    var body = GenerateCustomerEmailBody(contact, ticketId);

                    var mailMessage = new MailMessage(senderEmail, contact.Email, subject, body)
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
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];
                var adminEmail = _configuration["EmailSettings:AdminEmail"];

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    var subject = $"New Contact Form Submission - {contact.Subject}";
                    var body = GenerateAdminEmailBody(contact, ticketId);

                    var mailMessage = new MailMessage(senderEmail, adminEmail, subject, body)
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
    }
}
