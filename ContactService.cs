// IContactService.cs and ContactService.cs - Business Logic
using PremierElectric.Application.DTOs;
using PremierElectric.Domain.Entities;

namespace PremierElectric.Application.Services
{
    public interface IContactService
    {
        Task<ContactResponseDto> SubmitContactFormAsync(ContactSubmissionDto dto);
        Task<ContactSubmission> GetContactByIdAsync(Guid id);
    }

    public class ContactService : IContactService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactService> _logger;
        private readonly PremierElectricDbContext _dbContext;

        public ContactService(
            IEmailService emailService,
            ILogger<ContactService> logger,
            PremierElectricDbContext dbContext)
        {
            _emailService = emailService;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<ContactResponseDto> SubmitContactFormAsync(ContactSubmissionDto dto)
        {
            var submission = new ContactSubmission
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Subject = dto.Subject,
                Message = dto.Message,
                ServiceCategory = dto.ServiceCategory,
                PreferredContact = dto.PreferredContact,
                SubmittedAt = DateTime.UtcNow,
                Status = ContactStatus.Received
            };

            try
            {
                _dbContext.ContactSubmissions.Add(submission);
                await _dbContext.SaveChangesAsync();

                // Send emails in background
                _ = Task.Run(async () =>
                {
                    await _emailService.SendContactConfirmationAsync(dto, submission.Id);
                    await _emailService.SendAdminNotificationAsync(dto, submission.Id);
                });

                _logger.LogInformation($"Contact submission {submission.Id} created successfully");

                return new ContactResponseDto
                {
                    Success = true,
                    Message = "Your message has been sent successfully. We'll get back to you soon.",
                    TicketId = submission.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error submitting contact form: {ex.Message}");
                return new ContactResponseDto
                {
                    Success = false,
                    Message = "An error occurred while processing your request.",
                    Errors = new Dictionary<string, string> { { "general", ex.Message } }
                };
            }
        }

        public async Task<ContactSubmission> GetContactByIdAsync(Guid id)
        {
            return await _dbContext.ContactSubmissions.FindAsync(id);
        }
    }
}
