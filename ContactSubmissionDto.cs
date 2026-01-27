// ContactSubmissionDto.cs - Data Transfer Object
namespace PremierElectric.Application.DTOs
{
    public class ContactSubmissionDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string ServiceCategory { get; set; }
        public string PreferredContact { get; set; }
    }

    public class ContactResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? TicketId { get; set; }
        public Dictionary<string, string> Errors { get; set; }
    }
}
